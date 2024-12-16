@allowed([
  'prod'
  'preview'
])
param environment string = 'preview'

param location string = resourceGroup().location

@minLength(3)
param appName string

param workspaceId string

param alertEmailAdresses array

param errorAlertName string

param errorAlertShortName string

param criticalAlertName string

param criticalAlertShortName string

param availabilityAlertShortName string

param customDomain string = ''

@secure()
param sqlAdminPassword string

@secure()
param brevoApiKey string

var environmentSettings = {
  prod: {
  }
  preview: {
    appServicePlan: {
      sku: {
        name: 'B1'
      }
    }
    sqlDatabase: {
      sku: {
        name: 'Basic'
      }
      properties: {}
    }
    appService: {
      UseCase: {
        name: 'CoreManagerOptions:UseCase'
        value: 'default'
      }
      JwtTokenAuthority: {
        name: 'CoreManagerOptions:JwtTokenAuthority'
        value: 'https://xxx.auth0.com/'
      }
      JwtTokenAudience: {
        name: 'CoreManagerOptions:JwtTokenAudience'
        value: 'Poort8-Dataspace-CoreManager'
      }
      RegisterNewUsersDisabled: {
        name: 'FeatureManagement:RegisterNewUsersDisabled'
        value: true
      }
      AROrgAndRGPagesDisabled: {
        name: 'FeatureManagement:AROrgAndRGPagesDisabled'
        value: false
      }
      ApiDisabled: {
        name: 'FeatureManagement:ApiDisabled'
        value: false
      }
      IshareEnabled: {
        name: 'FeatureManagement:IshareEnabled'
        value: false
      }
      ClientId: {
        name: 'IshareCoreOptions:ClientId'
        value: 'ClientId'
      }
      SatelliteId: {
        name: 'IshareCoreOptions:SatelliteId'
        value: 'SatelliteId'
      }
      SatelliteUrl: {
        name: 'IshareCoreOptions:SatelliteUrl'
        value: 'https://SatelliteUrl.nl'
      }
      ManageEntitiesApi: {
        name: 'Roles:ManageEntitiesApi'
        value: 'your@email.nl'
      }
      CanSetPolicyIssuer: {
        name: 'Roles:CanSetPolicyIssuer'
        value: 'your@email.nl'
      }
    }
  }
}

resource appServicePlan 'Microsoft.Web/serverfarms@2022-09-01' = {
  name: '${appName}-asp'
  location: location
  sku: environmentSettings[environment].appServicePlan.sku
}

resource applicationInsights 'Microsoft.Insights/components@2020-02-02' = {
  name: '${appName}-ai'
  location: location
  kind: 'web'
  properties: {
    Application_Type: 'web'
    Request_Source: 'rest'
    IngestionMode: 'LogAnalytics'
    WorkspaceResourceId: workspaceId
  }
}

resource applicationInsightsAudit 'Microsoft.Insights/components@2020-02-02' = {
  name: '${appName}-audit'
  location: location
  kind: 'web'
  properties: {
    Application_Type: 'web'
    Request_Source: 'rest'
    IngestionMode: 'LogAnalytics'
    WorkspaceResourceId: workspaceId
  }
}

module errorAlert '../../deploy/dataspaceAlertModule.bicep' = {
  name: 'ErrorAlert'
  params: {
    environment: environment
    location: location
    alertName: errorAlertName
    actionGroupShortName: errorAlertShortName
    emailAdresses: alertEmailAdresses
    evaluationFrequency: 'P1D'
    query: 'AppTraces\n| where Message has "P8.err"\n| project TimeGenerated, Message, AppRoleName'
    severity: 1
    workspaceId: workspaceId
  }
}

module criticalAlert '../../deploy/dataspaceAlertModule.bicep' = {
  name: 'CriticalAlert'
  params: {
    environment: environment
    location: location
    alertName: criticalAlertName
    actionGroupShortName: criticalAlertShortName
    emailAdresses: alertEmailAdresses
    evaluationFrequency: 'PT5M'
    query: 'AppTraces\n| where Message has "P8.crit"'
    severity: 0
    workspaceId: workspaceId
  }
}

resource actionGroup 'Microsoft.Insights/actionGroups@2023-01-01' = {
  name: '${appName}-action-group'
  location: 'global'
  properties: {
    enabled: true
    groupShortName: availabilityAlertShortName
    emailReceivers: [for emailAdress in alertEmailAdresses: {
      name: emailAdress
      emailAddress: emailAdress
      useCommonAlertSchema: true
    }]
  }
}

resource availabilityTest 'Microsoft.Insights/webtests@2022-06-15' = {
  name: '${appName}-availability-test'
  location: location
  tags: {
    'hidden-link:${applicationInsights.id}': 'Resource'
  }
  properties: {
    Configuration: {
      WebTest: '<WebTest         Name="${toLower(appName)}-health-test"         Id="${guid('seed')}"         Enabled="True"         CssProjectStructure=""         CssIteration=""         Timeout="120"         WorkItemIds=""         xmlns="http://microsoft.com/schemas/VisualStudio/TeamTest/2010"         Description=""         CredentialUserName=""         CredentialPassword=""         PreAuthenticate="True"         Proxy="default"         StopOnError="False"         RecordedResultFile=""         ResultsLocale="">        <Items>        <Request         Method="GET"         Guid="${guid('seed')}"         Version="1.1"         Url="https://${appService.properties.defaultHostName}/health"         ThinkTime="0"         Timeout="120"         ParseDependentRequests="False"         FollowRedirects="True"         RecordResult="True"         Cache="False"         ResponseTimeGoal="0"         Encoding="utf-8"         ExpectedHttpStatusCode="200"         ExpectedResponseUrl=""         ReportingName=""         IgnoreHttpStatusCode="False" />        </Items>        <ValidationRules>        <ValidationRule         Classname="Microsoft.VisualStudio.TestTools.WebTesting.Rules.ValidationRuleFindText, Microsoft.VisualStudio.QualityTools.WebTestFramework, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"         DisplayName="Find Text"         Description="Verifies the existence of the specified text in the response."         Level="High"         ExectuionOrder="BeforeDependents">        <RuleParameters>        <RuleParameter Name="FindText" Value="Healthy" />        <RuleParameter Name="IgnoreCase" Value="False" />        <RuleParameter Name="UseRegularExpression" Value="False" />        <RuleParameter Name="PassIfTextFound" Value="True" />        </RuleParameters>        </ValidationRule>        </ValidationRules>        </WebTest>'
    }
    Enabled: true
    Kind: 'ping'
    RetryEnabled: true
    Frequency: 300
    Timeout: 120
    Locations: [
      { Id: 'emea-fr-pra-edge'}
      { Id: 'emea-gb-db3-azr' }
      { Id: 'emea-se-sto-edge' }
      { Id: 'emea-nl-ams-azr' }
      { Id: 'emea-ru-msa-edge' }
    ]
    Name: '${appName}-health-test'
    SyntheticMonitorId: '${appName}-availability-test'
  }
}

resource pingAlertRule 'Microsoft.Insights/metricAlerts@2018-03-01' = {
  name: '${appName}-avail-alert'
  location: 'global'
  properties: {
    actions: [
      {
        actionGroupId: actionGroup.id
      }
    ]
    criteria: {
      'odata.type': 'Microsoft.Azure.Monitor.WebtestLocationAvailabilityCriteria'
      webTestId: availabilityTest.id
      componentId: applicationInsights.id
      failedLocationCount: 5
    }
    enabled: true
    evaluationFrequency: 'PT5M'
    scopes: [
      availabilityTest.id
      applicationInsights.id
    ]
    severity: 1
    windowSize: 'PT5M'
  }
}

resource sqlServer 'Microsoft.Sql/servers@2023-05-01-preview' = {
  name: '${toLower(appName)}-sql-server'
  location: location
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    administratorLogin: 'YourUserName'
    administratorLoginPassword: sqlAdminPassword
    minimalTlsVersion: '1.2'
  }
}

resource sqlAllowAllWindowsAzureIps 'Microsoft.Sql/servers/firewallRules@2023-05-01-preview' = {
  name: 'AllowAllWindowsAzureIps'
  parent: sqlServer
  properties: {
    startIpAddress: '0.0.0.0'
    endIpAddress: '0.0.0.0'
  }
}

resource sqlDB 'Microsoft.Sql/servers/databases@2023-05-01-preview' = {
  parent: sqlServer
  name: toLower(appName)
  location: location
  sku: environmentSettings[environment].sqlDatabase.sku
  properties: environmentSettings[environment].sqlDatabase.properties
}

resource appService 'Microsoft.Web/sites@2022-09-01' = {
  name: appName
  location: location
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    serverFarmId: appServicePlan.id
    vnetRouteAllEnabled: true
    httpsOnly: true
    clientAffinityEnabled: true
    siteConfig: {
      appSettings: [
        {
          name: 'APPINSIGHTS_INSTRUMENTATIONKEY'
          value: applicationInsights.properties.InstrumentationKey
        }
        {
          name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
          value: applicationInsights.properties.ConnectionString
        }
        {
          name: 'AUDIT_APPLICATIONINSIGHTS_CONNECTION_STRING'
          value: applicationInsightsAudit.properties.ConnectionString
        }
        {
          name: 'ApplicationInsightsAgent_EXTENSION_VERSION'
          value: '~2'
        }
        {
          name: 'XDT_MicrosoftApplicationInsights_Mode'
          value: 'Recommended'
        }
        {
          name: 'WEBSITE_TIME_ZONE'
          value: 'W. Europe Standard Time'
        }
        {
          name: 'WEBSITE_LOAD_USER_PROFILE'
          value: '1'
        }
        {
          name: 'OrganizationRegistry:ConnectionString'
          value: 'Data Source=tcp:${sqlServer.properties.fullyQualifiedDomainName},1433;Initial Catalog=${sqlDB.name};User Id=${sqlServer.properties.administratorLogin}@${sqlServer.properties.fullyQualifiedDomainName};Password=${sqlAdminPassword};'
        }
        {
          name: 'AuthorizationRegistry:ConnectionString'
          value: 'Data Source=tcp:${sqlServer.properties.fullyQualifiedDomainName},1433;Initial Catalog=${sqlDB.name};User Id=${sqlServer.properties.administratorLogin}@${sqlServer.properties.fullyQualifiedDomainName};Password=${sqlAdminPassword};'
        }
        {
          name: 'Identity:ConnectionString'
          value: 'Data Source=tcp:${sqlServer.properties.fullyQualifiedDomainName},1433;Initial Catalog=${sqlDB.name};User Id=${sqlServer.properties.administratorLogin}@${sqlServer.properties.fullyQualifiedDomainName};Password=${sqlAdminPassword};'
        }
        environmentSettings[environment].appService.RegisterNewUsersDisabled
        environmentSettings[environment].appService.AROrgAndRGPagesDisabled
        environmentSettings[environment].appService.ApiDisabled
        environmentSettings[environment].appService.IshareEnabled
        environmentSettings[environment].appService.ClientId
        environmentSettings[environment].appService.SatelliteId
        environmentSettings[environment].appService.SatelliteUrl
        environmentSettings[environment].appService.UseCase
        environmentSettings[environment].appService.ManageEntitiesApi
        environmentSettings[environment].appService.CanSetPolicyIssuer
        environmentSettings[environment].appService.JwtTokenAuthority
        environmentSettings[environment].appService.JwtTokenAudience
        {
          name: 'BrevoApiKey'
          value: brevoApiKey
        }
      ]
      phpVersion: 'OFF'
      netFrameworkVersion: 'v9.0'
      alwaysOn: true
      webSocketsEnabled: true
      ipSecurityRestrictionsDefaultAction: 'Allow'
      ipSecurityRestrictions: [
        {
          ipAddress: 'Any'
          action: 'Allow'
          priority: 2147483647
          name: 'Allow all'
          description: 'Allow all access'
        }
      ]
    }
  }
}

module customDomainModule 'customDomainModule.bicep' = if (length(customDomain) > 0) {
  name: 'CustomDomainModule'
  params: {
    location: location
    appName: appService.name
    customDomain: customDomain
    appServicePlanId: appServicePlan.id
  }
}

output appServiceName string = appService.name

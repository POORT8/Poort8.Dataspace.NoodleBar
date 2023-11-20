@allowed([
  'prod'
  'preview'
])
param environment string = 'preview'

param location string = resourceGroup().location

var isProd = environment == 'prod'
var nameAddition = (isProd ? '' : '-${environment}')

var appName = 'Poort8-Dataspace-CoreManager${nameAddition}'

var environmentSettings = {
  prod: {
    appServicePlan: {
      sku: {
        name: 'B2'
      }
    }
  }
  preview: {
    appServicePlan: {
      sku: {
        name: 'B1'
      }
    }
  }
}

resource appServicePlan 'Microsoft.Web/serverfarms@2022-09-01' = {
  name: '${appName}-asp'
  location: location
  sku: environmentSettings[environment].appServicePlan.sku
}

module workspace '../../deploy/dataspaceWorkspaceModule.bicep' = {
  name: 'WorkspaceModule'
  params: {
    environment: environment
    location: location
  }
}

resource applicationInsights 'Microsoft.Insights/components@2020-02-02' = {
  name: '${appName}-ai'
  location: location
  kind: 'web'
  properties: {
    Application_Type: 'web'
    Request_Source: 'rest'
    IngestionMode: 'LogAnalytics'
    WorkspaceResourceId: workspace.outputs.workspaceId
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
    WorkspaceResourceId: workspace.outputs.workspaceId
  }
}

module errorAlert '../../deploy/dataspaceAlertModule.bicep' = {
  name: 'ErrorAlert'
  params: {
    environment: environment
    location: location
    alertName: 'Dataspace-CoreManager-Error'
    actionGroupShortName: 'DCME${nameAddition}'
    emailAdresses: [
      'merijn@poort8.nl'
      'cunes@poort8.nl'
    ]
    evaluationFrequency: 'P1D'
    query: 'AppTraces\n| where Message has "P8.err"\n| project TimeGenerated, Message, AppRoleName'
    severity: 1
    workspaceId: workspace.outputs.workspaceId
  }
}

module criticalAlert '../../deploy/dataspaceAlertModule.bicep' = {
  name: 'CriticalAlert'
  params: {
    environment: environment
    location: location
    alertName: 'Dataspace-CoreManager-Critical'
    actionGroupShortName: 'DCMC${nameAddition}'
    emailAdresses: [
      'merijn@poort8.nl'
      'cunes@poort8.nl'
    ]
    evaluationFrequency: 'PT5M'
    query: 'AppTraces\n| where Message has "P8.crit"'
    severity: 0
    workspaceId: workspace.outputs.workspaceId
  }
}

resource actionGroup 'Microsoft.Insights/actionGroups@2023-01-01' = {
  name: '${appName}-action-group'
  location: 'global'
  properties: {
    enabled: true
    groupShortName: 'DCM${nameAddition}'
    emailReceivers: [
      {
        name: 'cunes@poort8.nl'
        emailAddress: 'cunes@poort8.nl'
        useCommonAlertSchema: true
      }
      {
        name: 'merijn@poort8.nl'
        emailAddress: 'merijn@poort8.nl'
        useCommonAlertSchema: true
      }
    ]
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
      WebTest: '<WebTest         Name="${toLower(appName)}-health-test"         Id="${guid('seed')}"         Enabled="True"         CssProjectStructure=""         CssIteration=""         Timeout="120"         WorkItemIds=""         xmlns="http://microsoft.com/schemas/VisualStudio/TeamTest/2010"         Description=""         CredentialUserName=""         CredentialPassword=""         PreAuthenticate="True"         Proxy="default"         StopOnError="False"         RecordedResultFile=""         ResultsLocale="">        <Items>        <Request         Method="GET"         Guid="${guid('seed')}"         Version="1.1"         Url="https://${appService.properties.defaultHostName}/health"         ThinkTime="0"         Timeout="120"         ParseDependentRequests="False"         FollowRedirects="True"         RecordResult="True"         Cache="False"         ResponseTimeGoal="0"         Encoding="utf-8"         ExpectedHttpStatusCode="200"         ExpectedResponseUrl=""         ReportingName=""         IgnoreHttpStatusCode="False" />        </Items>        <ValidationRules>        <ValidationRule         Classname="Microsoft.VisualStudio.TestTools.WebTesting.Rules.ValidationRuleFindText, Microsoft.VisualStudio.QualityTools.WebTestFramework, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"         DisplayName="Find Text"         Description="Verifies the existence of the specified text in the response."         Level="High"         ExectuionOrder="BeforeDependents">        <RuleParameters>        <RuleParameter Name="FindText" Value="Hello from Dataspace Core Manager" />        <RuleParameter Name="IgnoreCase" Value="False" />        <RuleParameter Name="UseRegularExpression" Value="False" />        <RuleParameter Name="PassIfTextFound" Value="True" />        </RuleParameters>        </ValidationRule>        </ValidationRules>        </WebTest>'
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

resource appService 'Microsoft.Web/sites@2022-09-01' = {
  name: appName
  location: location
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
      ]
      phpVersion: 'OFF'
      netFrameworkVersion: 'v8.0'
      alwaysOn: true
      webSocketsEnabled: true
    }
  }
}

resource ipRestriction 'Microsoft.Web/sites/config@2022-03-01' = {
  name: 'web'
  parent: appService
  properties: {
    netFrameworkVersion: 'v8.0'
    ipSecurityRestrictions: [
      {
        ipAddress: 'ApplicationInsightsAvailability'
        action: 'Allow'
        tag: 'ServiceTag'
        priority: 1
        name: 'ApplicationInsightsAvailability'
      }
      {
        ipAddress: '193.172.20.145/32'
        action: 'Allow'
        priority: 2
        name: 'Kantoor IP'
      }
      {
        ipAddress: '83.128.161.194/32'
        action: 'Allow'
        priority: 3
        name: 'Cunes IP'
      }
      {
        ipAddress: '143.177.169.7/32'
        action: 'Allow'
        priority: 4
        name: 'Merijn IP'
      }
      {
        ipAddress: 'Any'
        action: 'Deny'
        priority: 2147483647
        name: 'Deny all'
        description: 'Deny all access'
      }
    ]
  }
}

output appServiceName string = appService.name

@allowed([
  'prod'
  'preview'
])
param environment string = 'preview'

param location string = resourceGroup().location

var isProd = environment == 'prod'
var nameAddition = (isProd ? '' : '-${environment}')

var environmentAppName = 'Poort8-Dataspace-CoreManager${nameAddition}'

resource keyVault 'Microsoft.KeyVault/vaults@2022-07-01' existing = {
  name: 'YourVaultName'
  scope: resourceGroup('ResourceGroupGuid', 'ResourceGroupName')
}

module workspace '../../deploy/dataspaceWorkspaceModule.bicep' = {
  name: 'WorkspaceModule'
  params: {
    environment: environment
    location: location
    appName: environmentAppName
  }
}

module appService 'appServiceModule.bicep' = {
  name: 'AppServiceModule'
  params: {
    environment: environment
    location: location
    appName: environmentAppName
    workspaceId: workspace.outputs.workspaceId
    alertEmailAdresses: [
      'your@email.nl'
    ]
    errorAlertName: 'Dataspace-CoreManager-Error'
    errorAlertShortName: 'DCME${nameAddition}'
    criticalAlertName: 'Dataspace-CoreManager-Critical'
    criticalAlertShortName: 'DCMC${nameAddition}'
    availabilityAlertShortName: 'DCM${nameAddition}'
    sqlAdminPassword: keyVault.getSecret('Poort8DataspaceSqlAdminPassword-${environment}')
    brevoApiKey: keyVault.getSecret('BrevoApiKey')
  }
}

output appServiceName string = appService.outputs.appServiceName

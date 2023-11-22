@allowed([
  'prod'
  'preview'
])
param environment string = 'preview'

param location string = resourceGroup().location

module appService 'appServiceModule.bicep' = {
  name: 'AppServiceModule'
  params: {
    environment: environment
    location: location
    appName: 'Poort8-Dataspace-CoreManager'
    alertEmailAdresses: [
      'merijn@poort8.nl'
      'cunes@poort8.nl'
    ]
    errorAlertName: 'Dataspace-CoreManager-Error'
    errorAlertShortName: 'DCME'
    criticalAlertName: 'Dataspace-CoreManager-Critical'
    criticalAlertShortName: 'DCMC'
    availabilityAlertShortName: 'DCM'
    ipWhiteList: [
      {
        ipAddress: '193.172.20.145'
        name: 'Kantoor IP'
      }
      {
        ipAddress: '83.128.161.194'
        name: 'Cunes IP'
      }
      {
        ipAddress: '143.177.169.7'
        name: 'Merijn IP'
      }
    ]
  }
}

output appServiceName string = appService.outputs.appServiceName

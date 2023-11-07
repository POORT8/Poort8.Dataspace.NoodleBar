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
  }
}

output appServiceName string = appService.outputs.appServiceName

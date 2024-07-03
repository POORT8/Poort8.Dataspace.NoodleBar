param appName string
param customDomain string
param appServicePlanId string
param location string

resource appService 'Microsoft.Web/sites@2022-09-01' existing = {
  name: appName
}

resource appServiceHostNameBinding 'Microsoft.Web/sites/hostNameBindings@2022-03-01' = {
  parent: appService
  name: customDomain
}

resource sslCertificate 'Microsoft.Web/certificates@2022-03-01' = {
  name: '${customDomain}-${appName}'
  location: location
  dependsOn: [
    appServiceHostNameBinding
  ]
  properties: {
    serverFarmId: appServicePlanId
    canonicalName: customDomain
  }
}

module sslBinding 'sslBindingModule.bicep' = {
  name: 'SSLBindingModule'
  params: {
    appName: appService.name
    customDomain: appServiceHostNameBinding.name
    certificateThumbprint: sslCertificate.properties.thumbprint
  }
}

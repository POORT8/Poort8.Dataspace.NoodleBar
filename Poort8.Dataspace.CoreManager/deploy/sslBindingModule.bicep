param appName string
param customDomain string
param certificateThumbprint string

resource sslBindingEnable 'Microsoft.Web/sites/hostNameBindings@2020-06-01' = {
  name: '${appName}/${customDomain}'
  properties: {
    sslState: 'SniEnabled'
    thumbprint: certificateThumbprint
  }
}

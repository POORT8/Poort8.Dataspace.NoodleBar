@allowed([
  'prod'
  'preview'
])
param environment string = 'preview'

param location string = resourceGroup().location

@minLength(3)
param appName string

var environmentSettings = {
  prod: {
    retentionInDays: 730
  }
  preview: {
    retentionInDays: 90
  }
}

resource workspace 'Microsoft.OperationalInsights/workspaces@2022-10-01' = {
  name: '${appName}-ws'
  location: location
}

resource appTracesTable 'Microsoft.OperationalInsights/workspaces/tables@2022-10-01' = {
  parent: workspace
  name: 'AppTraces'
  properties: {
    totalRetentionInDays: environmentSettings[environment].retentionInDays
    retentionInDays: environmentSettings[environment].retentionInDays
  }
}

output workspaceId string = workspace.id

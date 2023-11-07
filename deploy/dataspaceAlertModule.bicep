@allowed([
  'prod'
  'preview'
])
param environment string = 'preview'

param location string = resourceGroup().location

param alertName string

@minLength(1)
param emailAdresses array

@maxLength(12)
param actionGroupShortName string

@minValue(0)
@maxValue(4)
param severity int

param evaluationFrequency string

param workspaceId string

param query string

var isProd = environment == 'prod'
var nameAddition = (isProd ? '' : '-${environment}')

resource actionGroup 'Microsoft.Insights/actionGroups@2023-01-01' = {
  name: '${alertName}${nameAddition}-action-group'
  location: 'Global'
  properties: {
    enabled: true
    groupShortName: actionGroupShortName
    emailReceivers: [for email in emailAdresses: {
      name: email
      emailAddress: email
      useCommonAlertSchema: false
    }]
  }
}

resource alert 'Microsoft.Insights/scheduledQueryRules@2022-08-01-preview' = {
  name: '${alertName}${nameAddition}'
  location: location
  properties: {
    severity: severity
    enabled: true
    evaluationFrequency: evaluationFrequency
    scopes: [
      workspaceId
    ]
    targetResourceTypes: [
      'Microsoft.OperationalInsights/workspaces'
    ]
    windowSize: evaluationFrequency
    criteria: {
      allOf: [
        {
          query: query
          timeAggregation: 'Count'
          dimensions: []
          operator: 'GreaterThan'
          threshold: 0
          failingPeriods: {
            numberOfEvaluationPeriods: 1
            minFailingPeriodsToAlert: 1
          }
        }
      ]
    }
    actions: {
      actionGroups: [
        actionGroup.id
      ]
    }
    autoMitigate: false
  }
}

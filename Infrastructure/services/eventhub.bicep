param env string
param location string
param serverNumber string

resource eventHub 'Microsoft.EventHub/namespaces@2025-05-01-preview' = {
  name: 'tradingking-${env}-${location}-${serverNumber}'
  location: location
  sku: {
    name: 'Basic'
    capacity: 1
  }

  /* resource authRules 'authorizationRules' = {
    name: 'RootManageSharedAccessKey'
    properties: {
      rights: [
        'Listen'
        'Manage'
        'Send'
      ]
    }
  } */

  resource sqltrigger 'eventhubs' = {
    name: 'sqltrigger'
    properties: {
      messageRetentionInDays: 1
      partitionCount: 1
      retentionDescription: {
        cleanupPolicy: 'Delete'
        retentionTimeInHours: 1
      }
    }
  }
}

resource asd 'Microsoft.EventHub/namespaces/authorizationRules@2025-05-01-preview' = {
  parent: eventHub
  name: 'RootManageSharedAccessKey'
  properties: {
    rights: [
      'Listen'
      'Manage'
      'Send'
    ]
  }
}

output ruleName string = asd.name

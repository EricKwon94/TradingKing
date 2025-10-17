param env string
param location string
param serverNumber string

resource serviceBus 'Microsoft.ServiceBus/namespaces@2025-05-01-preview' = {
  name: 'tradingking-${env}-${location}-${serverNumber}'
  location: location
  sku: {
    name: 'Basic'
  }

  resource authRules 'authorizationRules' = {
    name: 'RootManageSharedAccessKey'
    properties: {
      rights: [
        'Listen'
        'Manage'
        'Send'
      ]
    }
  }

  resource order 'queues' = {
    name: 'order'
    properties: {
      defaultMessageTimeToLive: 'PT1H'
    }
  }

  resource rank 'queues' = {
    name: 'rank'
    properties: {
      defaultMessageTimeToLive: 'PT1H'
    }
  }
}

output cs string = serviceBus::authRules.listkeys().primaryConnectionString

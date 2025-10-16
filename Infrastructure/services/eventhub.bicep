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
  /* properties: {
    publicNetworkAccess: 'Enabled'
    zoneRedundant: true
    kafkaEnabled: true
  } */

  /* resource auth 'authorizationRules' = {
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
  }
}

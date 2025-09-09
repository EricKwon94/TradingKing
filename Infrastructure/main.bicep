targetScope = 'subscription'

param env string
param location string
param serverNumber string

resource rgApp 'Microsoft.Resources/resourceGroups@2025-03-01' = {
  name: 'tradingking-${env}-${location}-${serverNumber}'
  location: location
}

module app 'services/_main.bicep' = {
  name: 'dp-app'
  scope: rgApp
  params: {
    env: env
    location: location
    serverNumber: serverNumber
  }
}

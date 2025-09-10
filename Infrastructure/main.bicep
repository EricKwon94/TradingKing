targetScope = 'subscription'

param env string
param location string
param serverNumber string
param registryUrl string
param registryUserName string

@secure()
param issKey string
@secure()
param registryPassword string

/*
resource rgShared 'Microsoft.Resources/resourceGroups@2025-04-01' = {
  name: 'tradingking-shared-${location}'
  location: location
}

module shared 'services/_shared.bicep' = {
  name: 'dp-shared'
  scope: rgShared
}
*/

resource rgApp 'Microsoft.Resources/resourceGroups@2025-04-01' = {
  name: 'tradingking-${env}-${location}-${serverNumber}'
  location: location
}

module app 'services/_main.bicep' = {
  name: 'dp-app'
  scope: rgApp
  params: {
    env: env
    serverNumber: serverNumber
    issKey: issKey
    registryUrl: registryUrl
    registryUserName: registryUserName
    registryPassword: registryPassword
  }
}

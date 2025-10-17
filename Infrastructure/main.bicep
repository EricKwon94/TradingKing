targetScope = 'subscription'

param env string
param location string
param serverNumber string

param dockerWebServerImageName string
param dockerRankServerImageName string
param dockerFuncServerImageName string
param dockerUrl string
param dockerUserName string

@secure()
param issKey string
@secure()
param dockerPassword string
@secure()
param sqlsrvId string
@secure()
param sqlsrvPwd string


resource rgShared 'Microsoft.Resources/resourceGroups@2025-04-01' = {
  name: 'tradingking-shared-${location}'
  location: location
}

module shared 'services/_shared.bicep' = {
  name: 'dp-shared'
  scope: rgShared
  params:{
    
  }
}


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
    dockerWebServerImageName: dockerWebServerImageName
    dockerRankServerImageName: dockerRankServerImageName
    dockerFuncServerImageName: dockerFuncServerImageName
    dockerUrl: dockerUrl
    dockerUserName: dockerUserName
    dockerPassword: dockerPassword
    sqlsrvId: sqlsrvId
    sqlsrvPwd: sqlsrvPwd
  }
}

param env string
param serverNumber string
param location string = resourceGroup().location

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

module appService 'app-service.bicep' = {
  name: 'dp-appService'
  params: {
    env: env
    location: location
    serverNumber: serverNumber
    issKey: issKey
    dockerWebServerImageName: dockerWebServerImageName
    dockerUrl: dockerUrl
    dockerUserName: dockerUserName
    dockerPassword: dockerPassword
  }
}

module signalR 'signalr.bicep' = {
  name: 'dp-signalR'
  params: {
    env: env
    location: location
    serverNumber: serverNumber
  }
}

module sqlServer 'sql-server.bicep' = {
  name: 'dp-sqlServer'
  params: {
    env: env
    location: location
    serverNumber: serverNumber
    sqlsrvId: sqlsrvId
    sqlsrvPwd: sqlsrvPwd
  }
}

module serviceLinker 'service-linker.bicep' = {
  name: 'dp-serviceLinker'
  params: {
    appServiceName: appService.outputs.appServiceName
    signalrId: signalR.outputs.id
    sqldbId: sqlServer.outputs.sqldbId
    sqlsrvId: sqlsrvId
    sqlsrvPwd: sqlsrvPwd
  }
}

/*
module sa 'storage.bicep' = {
  name: 'dp-storage'
  params: {
    env: env
    location: location
    serverNumber: serverNumber
    deploymentStorageContainerName: deploymentStorageContainerName
  }
}
*/

module eventHub 'eventhub.bicep' = {
  name: 'dp-eventHub'
  params: {
    env: env
    location: location
    serverNumber: serverNumber
  }
}

module ca 'container-apps.bicep' = {
  name: 'dp-ca'
  params: {
    env: env
    location: location
    serverNumber: serverNumber
    eventHubCs: eventHub.outputs.cs
    sqlsrvdn: sqlServer.outputs.domainName
    sqlsrvId: sqlsrvId
    sqlsrvPwd: sqlsrvPwd
    dockerRankServerImageName: dockerRankServerImageName
    dockerFuncServerImageName: dockerFuncServerImageName
    dockerUrl: dockerUrl
    dockerUserName: dockerUserName
    dockerPassword: dockerPassword
  }
}


param env string
param serverNumber string
param location string = resourceGroup().location
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

param resourceToken string = toLower(uniqueString(subscription().id, location))
param appName string = 'func-${resourceToken}'
var deploymentStorageContainerName = 'app-package-${take(appName, 32)}-${take(resourceToken, 7)}'

module appService 'app-service.bicep' = {
  name: 'dp-appService'
  params: {
    env: env
    location: location
    serverNumber: serverNumber
    issKey: issKey
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

module sa 'storage.bicep' = {
  name: 'dp-storage'
  params: {
    env: env
    location: location
    serverNumber: serverNumber
    deploymentStorageContainerName: deploymentStorageContainerName
  }
}

module func 'functions.bicep' = {
  name: 'dp-functions'
  params: {
    env: env
    location: location
    serverNumber: serverNumber
    deploymentStorageContainerName: deploymentStorageContainerName
    blob: sa.outputs.blob
    sqlsrvdn: sqlServer.outputs.domainName
    sqlsrvId: sqlsrvId
    sqlsrvPwd: sqlsrvPwd
    saName: sa.outputs.name
  }
}

param env string
param serverNumber string
param location string = resourceGroup().location
param registryUrl string
param registryUserName string

@secure()
param issKey string
@secure()
param registryPassword string
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
    registryUrl: registryUrl
    registryUserName: registryUserName
    registryPassword: registryPassword
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
    sqldbId: sqlServer.outputs.sqldbId
    sqlsrvId: sqlsrvId
    sqlsrvPwd: sqlsrvPwd
  }
}

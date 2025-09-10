param env string
param serverNumber string
param location string = resourceGroup().location
param registryUrl string
param registryUserName string

@secure()
param issKey string
@secure()
param registryPassword string

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

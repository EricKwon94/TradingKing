param env string
param location string
param serverNumber string
param deploymentStorageContainerName string
param blob string

resource appServicePlan2 'Microsoft.Web/serverfarms@2024-11-01' = {
  name: 'functk-${env}-${location}-${serverNumber}'
  location: location
  kind: 'functionapp'
  sku: {
    tier: 'FlexConsumption'
    name: 'FC1'
  }
  properties: {
    reserved: true
    maximumElasticWorkerCount: 1
  }
}

resource site2 'Microsoft.Web/sites@2024-11-01' = {
  name: 'functk-${env}-${location}-${serverNumber}'
  location: location
  kind: 'functionapp,linux'
  properties: {
    serverFarmId: appServicePlan2.id
    enabled: true
    reserved: true
    httpsOnly: true
    publicNetworkAccess: 'Enabled'
    keyVaultReferenceIdentity: 'SystemAssigned'
    functionAppConfig: {
      deployment: {
        storage: {
          type: 'blobContainer'
          value: '${blob}${deploymentStorageContainerName}'
          authentication: {
            type: 'storageaccountconnectionstring'
            storageAccountConnectionStringName: 'DEPLOYMENT_STORAGE_CONNECTION_STRING'
          }
        }
      }
      runtime: {
        name: 'dotnet-isolated'
        version: '9.0'
      }
      scaleAndConcurrency: {
        maximumInstanceCount: 100
        instanceMemoryMB: 2048
      }
    }
  }
}

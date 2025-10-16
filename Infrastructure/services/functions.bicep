param env string
param location string
param serverNumber string
param deploymentStorageContainerName string
param blob string
param saName string
param sqlsrvdn string

@secure()
param sqlsrvId string
@secure()
param sqlsrvPwd string

resource sa 'Microsoft.Storage/storageAccounts@2025-01-01' existing = {
  name: saName
}

var s string ='DefaultEndpointsProtocol=https;AccountName=${sa.name};AccountKey=${listKeys(sa.id, sa.apiVersion).keys[0].value};EndpointSuffix=${environment().suffixes.storage}'

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
    containerSize: 1536
    siteConfig: {
      functionAppScaleLimit: 100
    }
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

  resource config 'config' = {
    name: 'appsettings'
    properties: {
      AzureWebJobsStorage: s
      DEPLOYMENT_STORAGE_CONNECTION_STRING: s
      TradingKing: 'Data Source=${sqlsrvdn};Initial Catalog=TradingKing;User ID=${sqlsrvId};Password=${sqlsrvPwd};'
    }
  }
}

param env string
param location string
param serverNumber string
param deploymentStorageContainerName string
param blob string
param saName string
param eventHubRuleName string
param sqlsrvdn string

@secure()
param sqlsrvId string
@secure()
param sqlsrvPwd string

resource sa 'Microsoft.Storage/storageAccounts@2025-01-01' existing = {
  name: saName
}

resource eventHubRule 'Microsoft.EventHub/namespaces/eventhubs/authorizationRules@2025-05-01-preview' existing = {
  name: eventHubRuleName
}

var saCS string = 'DefaultEndpointsProtocol=https;AccountName=${sa.name};AccountKey=${listKeys(sa.id, sa.apiVersion).keys[0].value};EndpointSuffix=${environment().suffixes.storage}'
var eventHubCs string = eventHubRule.listkeys().primaryConnectionString

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

  resource configWeb 'config' = {
    name: 'web'
    properties: {
      cors: {
        allowedOrigins: [
          'https://portal.azure.com'
        ]
        supportCredentials: false
      }
    }
  }

  resource config 'config' = {
    name: 'appsettings'
    properties: {
      AzureWebJobsStorage: saCS
      DEPLOYMENT_STORAGE_CONNECTION_STRING: saCS
      SqlHubName: 'SqlTrigger'
      TimerHubName: 'TimerTrigger'
      EventHub: eventHubCs
    }
  }

  resource configCs 'config' = {
    name: 'connectionstrings'
    properties: {
      TradingKing: {
        value: 'Data Source=${sqlsrvdn};Initial Catalog=TradingKing;User ID=${sqlsrvId};Password=${sqlsrvPwd};'
        type: 'SQLAzure'
      }
    }
  }
}

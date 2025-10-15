param env string
param serverNumber string
param location string = resourceGroup().location

resource cr 'Microsoft.ContainerRegistry/registries@2025-04-01' = {
  name: 'tradingking${location}'
  location: 'koreacentral'
  sku: {
    name: 'Basic'
  }
  properties:{
    adminUserEnabled: true
  }
}

/*
resource sa 'Microsoft.Storage/storageAccounts@2025-01-01' = {
  name: 'tradingking${location}'
  location: location
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'FileStorage'
  properties: {
    accessTier: 'Hot'
    encryption: {
      requireInfrastructureEncryption: false
      services: {
        file: {
          keyType: 'Account'
          enabled: true
        }
      }
      keySource: 'Microsoft.Storage'
    }
  }

  resource safs 'fileServices@2025-01-01' = {
    name: 'default'

    resource safss 'shares@2025-01-01' = {
      name: 'seqacistorage'
      properties: {
        accessTier: 'TransactionOptimized'
        shareQuota: 102400
        enabledProtocols: 'SMB'
      }
    }
  }
}
*/

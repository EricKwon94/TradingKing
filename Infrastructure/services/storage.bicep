param location string = resourceGroup().location
param storageAccounts_tradingking_name string = 'tradingking'

resource sa 'Microsoft.Storage/storageAccounts@2025-01-01' = {
  name: storageAccounts_tradingking_name
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

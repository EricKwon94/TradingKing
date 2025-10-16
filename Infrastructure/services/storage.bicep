param env string
param location string
param serverNumber string
param deploymentStorageContainerName string

resource sa 'Microsoft.Storage/storageAccounts@2025-01-01' = {
  name: 'tki${env}${location}${serverNumber}'
  location: location
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
  properties: {
    accessTier: 'Hot'
    publicNetworkAccess: 'Enabled'
    allowSharedKeyAccess: true
    largeFileSharesState: 'Enabled'
    minimumTlsVersion: 'TLS1_2'
    supportsHttpsTrafficOnly: true
    networkAcls: {
      bypass: 'AzureServices'
      defaultAction: 'Allow'
    }
    encryption: {
      requireInfrastructureEncryption: false
      services: {
        file: {
          keyType: 'Account'
          enabled: true
        }
        blob: {
          keyType: 'Account'
          enabled: true
        }
      }
      keySource: 'Microsoft.Storage'
    }
  }

  resource blob 'blobServices' = {
    name: 'default'
    properties: {
      containerDeleteRetentionPolicy: {
        enabled: true
        days: 7
      }
      deleteRetentionPolicy: {
        allowPermanentDelete: false
        enabled: true
        days: 7
      }
    }

    resource container 'containers' = {
      name: deploymentStorageContainerName
      properties: {
        publicAccess: 'None'
      }
    }
  }

  resource file 'fileServices' = {
    name: 'default'
    properties: {
      shareDeleteRetentionPolicy: {
        enabled: true
        days: 7
      }
    }
  }

  resource q 'queueServices' = {
    name: 'default'
  }

  resource t 'tableServices' = {
    name: 'default'
  }
  /*
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
    */
}

output blob string = sa.properties.primaryEndpoints.blob
output name string = sa.name

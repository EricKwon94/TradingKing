param env string
param location string
param serverNumber string

@secure()
param sqlsrvId string
@secure()
param sqlsrvPwd string

resource sqlServer 'Microsoft.Sql/servers@2024-11-01-preview' = {
  name: 'tradingking-${env}-${location}-${serverNumber}'
  location: location
  properties: {
    administratorLogin: sqlsrvId
    administratorLoginPassword: sqlsrvPwd
    publicNetworkAccess: 'Enabled'
    minimalTlsVersion: '1.2'
  }

  resource automaticTuning 'automaticTuning' existing = {
    name: 'current'
  }

  resource firewallRules 'firewallRules' = {
    name: 'firewallRules'
    properties: {
      endIpAddress: '0.0.0.0'
      startIpAddress: '0.0.0.0'
    }
  }
}

resource sqlDB 'Microsoft.Sql/servers/databases@2024-11-01-preview' = {
  name: 'TradingKing'
  parent: sqlServer
  location: location
  sku: {
    name: 'Basic'
  }
  properties: {
    requestedBackupStorageRedundancy: 'Local'
  }
}

output sqldbId string = sqlDB.id

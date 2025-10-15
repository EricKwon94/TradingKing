param appServiceName string
param signalrId string
param sqldbId string
@secure()
param sqlsrvId string
@secure()
param sqlsrvPwd string

resource appService 'Microsoft.Web/sites@2024-11-01' existing = {
  name: appServiceName
}

resource serviceLinkerSignalR 'Microsoft.ServiceLinker/linkers@2024-07-01-preview' = {
  name: 'signalr_3687d'
  scope: appService
  properties: {
    clientType: 'dotnet'
    targetService: {
      type: 'AzureResource'
      id: signalrId
    }
    authInfo: {
      authType: 'secret'
      secretInfo: {
        secretType: 'rawValue'
      }
    }
    configurationInfo: {
      customizedKeys: {
        AZURE_SIGNALR_CONNECTIONSTRING: 'AZURE__SIGNALR__CONNECTIONSTRING'
      }
    }
  }
}

resource serviceLinkerSql 'Microsoft.ServiceLinker/linkers@2024-07-01-preview' = {
  name: 'sql_7fa05'
  scope: appService
  properties: {
    clientType: 'dotnet-connectionString'
    targetService: {
      type: 'AzureResource'
      id: sqldbId
    }
    authInfo: {
      authType: 'secret'
      name: sqlsrvId
      secretInfo: {
        secretType: 'rawValue'
        value: sqlsrvPwd
      }
    }
    configurationInfo: {
      customizedKeys: {
        AZURE_SQL_CONNECTIONSTRING: 'TradingKing'
      }
    }
  }
}

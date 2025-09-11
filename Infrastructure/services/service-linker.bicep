param appServiceName string
param sqldbId string
@secure()
param sqlsrvId string
@secure()
param sqlsrvPwd string

resource appService 'Microsoft.Web/sites@2024-11-01' existing = {
  name: appServiceName
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

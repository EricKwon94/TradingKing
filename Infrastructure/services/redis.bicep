param env string
param location string
param serverNumber string

resource redis 'Microsoft.Cache/redis@2024-11-01' = {
  name: 'tradingking-${env}-${location}-${serverNumber}'
  location: location
  properties:{    
    disableAccessKeyAuthentication: false
    publicNetworkAccess: 'Enabled'
    enableNonSslPort: false
    sku: {
      name: 'Basic'
      family: 'C'
      capacity: 0
    }
  }
}

var v string = '${redis.properties.hostName}:6380,password=${redis.listKeys().primaryKey},ssl=True,abortConnect=False'
output cs string = v
output id string = redis.id

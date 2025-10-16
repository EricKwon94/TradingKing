//param env string
//param serverNumber string
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

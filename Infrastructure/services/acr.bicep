
param location string = resourceGroup().location

resource acrResource 'Microsoft.ContainerRegistry/registries@2025-04-01' = {
  name: 'tradingkingshared${location}'
  location: location
  sku: {
    name: 'Basic'
  }
  properties: {
    adminUserEnabled: true
  }
}

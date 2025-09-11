param env string
param serverNumber string
param location string = resourceGroup().location

module storage 'storage.bicep' = {
  name: 'dp-storage'
  params: {
    location: location
  }
}

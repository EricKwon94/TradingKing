param env string
param location string
param serverNumber string

resource signalR 'Microsoft.SignalRService/signalR@2025-01-01-preview' = {
  name: 'tradingking-${env}-${location}-${serverNumber}'
  location: location  
  sku: {
    capacity: 1
    name: 'Free_F1'
  }
  properties: {
    features: [
      {
        flag: 'ServiceMode'
        value: 'Default'
      }
    ]
  }
}

output id string = signalR.id


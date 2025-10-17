param env string
param location string
param serverNumber string

param eventHubCs string
param sqlsrvdn string

param dockerRankServerImageName string
param dockerFuncServerImageName string
param dockerUrl string

@secure()
param sqlsrvId string
@secure()
param sqlsrvPwd string

resource cae 'Microsoft.App/managedEnvironments@2025-02-02-preview' = {
  name: 'tradingking-${env}-${location}-${serverNumber}'
  location: location
  properties: {
    publicNetworkAccess: 'Enabled'
    workloadProfiles: [
      {
        workloadProfileType: 'Consumption'
        name: 'Consumption'
        enableFips: false
      }
    ]
  }
}

resource caFunc 'Microsoft.App/containerApps@2025-02-02-preview' = {
  name: 'tkfunc-${env}-${location}-${serverNumber}'
  location: location
  kind: 'functionapp'
  properties: {
    managedEnvironmentId: cae.id
    workloadProfileName: 'Consumption'
    configuration: {
      activeRevisionsMode: 'Single'
      registries: [
        {
          server: dockerUrl
          identity: 'system-environment'
        }
      ]
    }
    template: {
      containers: [
        {
          name: cae.name
          image: '${dockerUrl}/${dockerFuncServerImageName}:latest'
          imageType: 'ContainerImage'
          env:[
            {
              name: 'SqlHubName'
              value: 'sqltrigger'
            }
            {
              name: 'TimerHubName'
              value: 'timertrigger'
            }
            {
              name: 'EventHub'
              value: eventHubCs
            }
            {
              name: 'ConnectionStrings__TradingKing'
              value: 'Data Source=${sqlsrvdn};Initial Catalog=TradingKing;User ID=${sqlsrvId};Password=${sqlsrvPwd};'
            }
          ]
          resources: {
            cpu: json('0.25')
            memory: '0.5Gi'
          }
        }
      ]
    }
  }
}
/*
resource caRanking 'Microsoft.App/containerApps@2025-02-02-preview' = {
  name: 'tradingking-${env}-${location}-${serverNumber}'
  location: location
  kind: 'containerapps'
  properties:{
    managedEnvironmentId: cae.id
    workloadProfileName: 'Consumption'
  }
}
*/

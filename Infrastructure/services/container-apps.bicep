param env string
param location string
param serverNumber string

param serviceBusCs string
param sqlsrvdn string

param dockerRankServerImageName string
param dockerFuncServerImageName string
param dockerUrl string
param dockerUserName string

@secure()
param sqlsrvId string
@secure()
param sqlsrvPwd string
@secure()
param dockerPassword string

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
    environmentId: cae.id
    workloadProfileName: 'Consumption'
    configuration: {
      activeRevisionsMode: 'Single'
      secrets:[
        {
          name: 'docker-password'
          value: dockerPassword
        }
      ]
      registries: [
        {
          server: dockerUrl
          username: dockerUserName
          passwordSecretRef:'docker-password'
        }
      ]
    }
    template: {
      containers: [
        {
          name: 'tkfunc'      
          image: '${dockerUrl}/${dockerFuncServerImageName}:latest'
          imageType: 'ContainerImage'
          env:[
            {
              name: 'OrderQueueName'
              value: 'order'
            }
            {
              name: 'RankQueueName'
              value: 'rank'
            }
            {
              name: 'ServiceBus'
              value: serviceBusCs
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

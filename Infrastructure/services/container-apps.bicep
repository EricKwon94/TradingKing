param env string
param location string
param serverNumber string

param dockerRankServerImageName string
param dockerFuncServerImageName string
param dockerUrl string
param dockerUserName string

@secure()
param redisCs string
@secure()
param serviceBusCs string
@secure()
param sqlsrvCs string
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
      secrets: [
        {
          name: 'docker-password'
          value: dockerPassword
        }
        {
          name: 'service-bus'
          value: serviceBusCs
        }
        {
          name: 'sqlsrv'
          value: sqlsrvCs
        }
      ]
      registries: [
        {
          server: dockerUrl
          username: dockerUserName
          passwordSecretRef: 'docker-password'
        }
      ]
    }
    template: {
      containers: [
        {
          name: 'tkfunc'
          image: '${dockerUrl}/${dockerFuncServerImageName}:latest'
          imageType: 'ContainerImage'
          env: [
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
              secretRef: 'service-bus'
            }
            {
              name: 'ConnectionStrings__TradingKing'
              secretRef: 'sqlsrv'
            }
          ]
          resources: {
            cpu: json('0.5')
            memory: '1.0Gi'
          }
        }
      ]
    }
  }
}

resource caRanking 'Microsoft.App/containerApps@2025-02-02-preview' = {
  name: 'tkrank-${env}-${location}-${serverNumber}'
  location: location
  kind: 'containerapps'
  properties: {
    environmentId: cae.id
    workloadProfileName: 'Consumption'
    configuration: {
      activeRevisionsMode: 'Single'
      secrets: [
        {
          name: 'docker-password'
          value: dockerPassword
        }
        {
          name: 'service-bus'
          value: serviceBusCs
        }
        {
          name: 'sqlsrv'
          value: sqlsrvCs
        }
        {
          name: 'redis'
          value: redisCs
        }
      ]
      registries: [
        {
          server: dockerUrl
          username: dockerUserName
          passwordSecretRef: 'docker-password'
        }
      ]
    }
    template: {
      containers: [
        {
          name: 'tkfunc'
          image: '${dockerUrl}/${dockerRankServerImageName}:latest'
          imageType: 'ContainerImage'
          env: [
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
              secretRef: 'service-bus'
            }
            {
              name: 'ConnectionStrings__TradingKing'
              secretRef: 'sqlsrv'
            }
            {
              name: 'redis'
              secretRef: 'redis'
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

param env string
param location string
param serverNumber string

param dockerWebServerImageName string
param dockerUrl string
param dockerUserName string

@secure()
param redisCs string
@secure()
param issKey string
@secure()
param dockerPassword string

resource appServicePlan 'Microsoft.Web/serverfarms@2024-11-01' = {
  name: 'tradingking-${env}-${location}-${serverNumber}'
  location: location
  properties: {
    reserved: true
  }
  sku: {
    name: 'F1'
  }
  kind: 'linux'
}

resource appService 'Microsoft.Web/sites@2024-11-01' = {
  name: 'tradingking-${env}-${location}-${serverNumber}'
  location: location
  properties: {
    serverFarmId: appServicePlan.id
    httpsOnly: true
    siteConfig: {
      minTlsVersion: '1.2'
      linuxFxVersion: 'sitecontainers'
      appSettings: [
        //{
          //name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
          //value: insightsConnectionString
        //}
        {
          name: 'ISS_KEY'
          value: issKey
        }
        {
          name: 'ASPNETCORE_ENVIRONMENT'
          value: env == 'dev' ? 'Development' : 'Production'
        }
        {
          name: 'DOCKER_ENABLE_CI'
          value: 'true'
        }
        {
          name: 'REDIS'
          value: redisCs
        }
      ]
    }
  }

  resource scm 'basicPublishingCredentialsPolicies' = {
    name: 'scm'
    properties: {
      allow: true
    }
  }

  resource siteContainer 'sitecontainers' = {
    name: 'main'
    properties: {
      image: '${dockerUrl}/${dockerWebServerImageName}:latest'
      targetPort: '8080'
      isMain: true
      startUpCommand: ''
      authType: 'UserCredentials'
      userName: dockerUserName
      passwordSecret: dockerPassword
    }
  }
}

output appServiceName string = appService.name

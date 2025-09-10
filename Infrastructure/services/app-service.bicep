param env string
param location string
param serverNumber string
param registryUrl string
param registryUserName string

@secure()
param issKey string
@secure()
param registryPassword string

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
      minTlsVersion: '1.3'
      linuxFxVersion: 'sitecontainers'
      appSettings: [
        {
          //name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
          //value: insightsConnectionString
        }
        {
          name: 'ISS_KEY'
          value: issKey
        }
        {
          name: 'ASPNETCORE_ENVIRONMENT'
          value: env == 'dev' ? 'Development' : 'Production'
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
      image: registryUrl
      targetPort: '8080'
      isMain: true
      startUpCommand: ''
      authType: 'UserCredentials'
      userName: registryUserName
      passwordSecret: registryPassword
    }
  }
}

output appServiceName string = appService.name

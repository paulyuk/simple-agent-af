targetScope = 'subscription'

@minLength(1)
@maxLength(64)
@description('Name of the the environment which is used to generate a short unique hash used in all resources.')
param environmentName string

@minLength(1)
@description('Primary location for all resources & Flex Consumption Function App')
@metadata({
  azd: {
    type: 'location'
  }
})
param location string = 'eastus2'

param vnetEnabled bool = false
param apiServiceName string = ''
param apiUserAssignedIdentityName string = ''
param applicationInsightsName string = ''
param appServicePlanName string = ''
param logAnalyticsName string = ''
param resourceGroupName string = ''
param storageAccountName string = ''
@description('Optional: Your Azure AD user/service principal object ID for development access. Leave empty for production. Local development should use "azd up" which sets this automatically.')
param principalId string = ''

@description('Friendly name for your Azure AI resource')
param aiProjectFriendlyName string = 'Simple AI Agent Project'

@description('Description of your Azure AI resource displayed in AI studio')
param aiProjectDescription string = 'This is a simple AI agent project for Azure Functions.'

@description('Enable Azure AI Search for vector store and search capabilities')
param enableAzureSearch bool = false

@description('Name of the Azure AI Search account')
param aiSearchName string = 'agent-ai-search'

@description('Enable Cosmos DB for agent thread storage')
param enableCosmosDb bool = false

@description('Name for capabilityHost.')
param accountCapabilityHostName string = 'caphostacc'

@description('Name for capabilityHost.')
param projectCapabilityHostName string = 'caphostproj'

@description('Name of the Azure AI Services account')
param aiServicesName string = 'agent-ai-services'

@description('Model name for deployment')
param modelName string = 'gpt-5-mini'

@description('Model format for deployment')
param modelFormat string = 'OpenAI'

@description('Model version for deployment')
param modelVersion string = '2025-08-07'

@description('Model deployment SKU name')
param modelSkuName string = 'GlobalStandard'

@description('Model deployment capacity')
param modelCapacity int = 50

@description('Name for the model deployment in Azure AI Services')
param modelDeploymentName string = 'chat'

@description('Name of the Cosmos DB account for agent thread storage')
param cosmosDbName string = 'agent-ai-cosmos'

@description('The AI Service Account full ARM Resource ID. This is an optional field, and if not provided, the resource will be created.')
param aiServiceAccountResourceId string = ''

@description('The Ai Search Service full ARM Resource ID. This is an optional field, and if not provided, the resource will be created.')
param aiSearchServiceResourceId string = ''

@description('The Ai Storage Account full ARM Resource ID. This is an optional field, and if not provided, the resource will be created.')
param aiStorageAccountResourceId string = ''

@description('The Cosmos DB Account full ARM Resource ID. This is an optional field, and if not provided, the resource will be created.')
param aiCosmosDbAccountResourceId string = ''

var abbrs = loadJsonContent('./abbreviations.json')
var resourceToken = toLower(uniqueString(subscription().id, environmentName, location))
var tags = { 'azd-env-name': environmentName }
var functionAppName = !empty(apiServiceName) ? apiServiceName : '${abbrs.webSitesFunctions}api-${resourceToken}'
var deploymentStorageContainerName = 'app-package-${take(functionAppName, 32)}-${take(toLower(uniqueString(functionAppName, resourceToken)), 7)}'

// Create a short, unique suffix, that will be unique to each resource group
var uniqueSuffix = toLower(uniqueString(subscription().id, environmentName, location))
var projectName = toLower('${environmentName}${uniqueSuffix}')

// Organize resources in a resource group
resource rg 'Microsoft.Resources/resourceGroups@2021-04-01' = {
  name: !empty(resourceGroupName) ? resourceGroupName : '${abbrs.resourcesResourceGroups}${environmentName}'
  location: location
  tags: tags
}

// User assigned managed identity to be used by the function app to reach storage and other dependencies
module apiUserAssignedIdentity 'br/public:avm/res/managed-identity/user-assigned-identity:0.4.1' = {
  name: 'apiUserAssignedIdentity'
  scope: rg
  params: {
    location: location
    tags: tags
    name: !empty(apiUserAssignedIdentityName) ? apiUserAssignedIdentityName : '${abbrs.managedIdentityUserAssignedIdentities}api-${resourceToken}'
  }
}

// Create an App Service Plan to group applications under the same payment plan and SKU
module appServicePlan 'br/public:avm/res/web/serverfarm:0.1.1' = {
  name: 'appserviceplan'
  scope: rg
  params: {
    name: !empty(appServicePlanName) ? appServicePlanName : '${abbrs.webServerFarms}${resourceToken}'
    location: location
    tags: tags
    kind: 'FunctionApp'
    sku: {
      tier: 'FlexConsumption'
      name: 'FC1'
    }
    reserved: true
  }
}

// Storage for Azure Functions
module storage 'br/public:avm/res/storage/storage-account:0.14.3' = {
  name: 'storage'
  scope: rg
  params: {
    name: !empty(storageAccountName) ? storageAccountName : '${abbrs.storageStorageAccounts}${resourceToken}'
    location: location
    tags: tags
    kind: 'StorageV2'
    skuName: 'Standard_LRS'
    allowBlobPublicAccess: false
    allowSharedKeyAccess: false
    publicNetworkAccess: 'Enabled'
    networkAcls: {
      bypass: 'AzureServices'
      defaultAction: 'Allow'
    }
    blobServices: {
      containers: [
        { name: deploymentStorageContainerName }
      ]
    }
  }
}

// Monitor application with Azure Monitor
module monitoring 'br/public:avm/ptn/azd/monitoring:0.1.0' = {
  name: 'monitoring'
  scope: rg
  params: {
    logAnalyticsName: !empty(logAnalyticsName) ? logAnalyticsName : '${abbrs.operationalInsightsWorkspaces}${resourceToken}'
    applicationInsightsName: !empty(applicationInsightsName) ? applicationInsightsName : '${abbrs.insightsComponents}${resourceToken}'
    applicationInsightsDashboardName: ''
    location: location
    tags: tags
  }
}

// Flex Consumption Function App
module api './app/api.bicep' = {
  name: 'api'
  scope: rg
  params: {
    name: functionAppName
    location: location
    tags: tags
    applicationInsightsName: monitoring.outputs.applicationInsightsName
    appServicePlanId: appServicePlan.outputs.resourceId
    runtimeName: 'dotnet-isolated'
    runtimeVersion: '8.0'
    storageAccountName: storage.outputs.name
    deploymentStorageContainerName: deploymentStorageContainerName
    identityId: apiUserAssignedIdentity.outputs.resourceId
    identityClientId: apiUserAssignedIdentity.outputs.clientId
    enableBlob: true
    enableQueue: true
    appSettings: {
      AZURE_CLIENT_ID: apiUserAssignedIdentity.outputs.clientId
      AZURE_AI_PROJECT_ENDPOINT: aiProject.outputs.projectEndpoint
      AZURE_AI_MODEL_DEPLOYMENT_NAME: modelDeploymentName
      AZURE_OPENAI_ENDPOINT: 'https://${aiDependencies.outputs.aiServicesName}.openai.azure.com/'
      AZURE_OPENAI_DEPLOYMENT_NAME: modelDeploymentName
    }
  }
}

// AI Dependencies (AI Services, Storage, Search, Cosmos DB)
module aiDependencies './agent/standard-dependent-resources.bicep' = {
  name: 'dependencies${projectName}'
  scope: rg
  params: {
    location: location
    tags: tags
    storageName: 'ai${abbrs.storageStorageAccounts}${resourceToken}'
    aiServicesName: '${aiServicesName}${uniqueSuffix}'
    aiSearchName: '${aiSearchName}${uniqueSuffix}'
    cosmosDbName: '${cosmosDbName}${uniqueSuffix}'
    enableAzureSearch: enableAzureSearch
    enableCosmosDb: enableCosmosDb
    modelName: modelName
    modelFormat: modelFormat
    modelVersion: modelVersion
    modelSkuName: modelSkuName
    modelCapacity: modelCapacity
    modelDeploymentName: modelDeploymentName
    modelLocation: location
    aiServiceAccountResourceId: aiServiceAccountResourceId
    aiSearchServiceResourceId: aiSearchServiceResourceId
    aiStorageAccountResourceId: aiStorageAccountResourceId
    aiCosmosDbAccountResourceId: aiCosmosDbAccountResourceId
  }
}

// AI Project
module aiProject './agent/standard-ai-project.bicep' = {
  name: 'project${projectName}'
  scope: rg
  params: {
    location: location
    tags: tags
    aiServicesAccountName: aiDependencies.outputs.aiServicesName
    aiProjectName: projectName
    aiProjectFriendlyName: aiProjectFriendlyName
    aiProjectDescription: aiProjectDescription
    enableAzureSearch: enableAzureSearch
    enableCosmosDb: enableCosmosDb
    cosmosDbAccountName: aiDependencies.outputs.cosmosDbAccountName
    cosmosDbAccountSubscriptionId: aiDependencies.outputs.cosmosDbAccountSubscriptionId
    cosmosDbAccountResourceGroupName: aiDependencies.outputs.cosmosDbAccountResourceGroupName
    storageAccountName: aiDependencies.outputs.storageAccountName
    storageAccountSubscriptionId: aiDependencies.outputs.storageAccountSubscriptionId
    storageAccountResourceGroupName: aiDependencies.outputs.storageAccountResourceGroupName
    aiSearchName: aiDependencies.outputs.aiSearchName
    aiSearchSubscriptionId: aiDependencies.outputs.aiSearchServiceSubscriptionId
    aiSearchResourceGroupName: aiDependencies.outputs.aiSearchServiceResourceGroupName
  }
}

// AI Project Role Assignments
module projectRoleAssignments './agent/standard-ai-project-role-assignments.bicep' = {
  name: 'rbac${projectName}'
  scope: rg
  params: {
    aiProjectPrincipalId: aiProject.outputs.aiProjectPrincipalId
    userPrincipalId: principalId
    allowUserIdentityPrincipal: !empty(principalId)
    aiServicesName: aiDependencies.outputs.aiServicesName
    aiSearchName: aiDependencies.outputs.aiSearchName
    aiCosmosDbName: aiDependencies.outputs.cosmosDbAccountName
    aiStorageAccountName: aiDependencies.outputs.storageAccountName
    integrationStorageAccountName: storage.outputs.name
    functionAppManagedIdentityPrincipalId: apiUserAssignedIdentity.outputs.principalId
    allowFunctionAppIdentityPrincipal: true
    enableAzureSearch: enableAzureSearch
    enableCosmosDb: enableCosmosDb
  }
}

module aiProjectCapabilityHost './agent/standard-ai-project-capability-host.bicep' = if (enableAzureSearch && enableCosmosDb) {
  name: 'caphost${projectName}'
  scope: rg
  params: {
    aiServicesAccountName: aiDependencies.outputs.aiServicesName
    projectName: aiProject.outputs.aiProjectName
    aiSearchConnection: aiProject.outputs.aiSearchConnection
    azureStorageConnection: aiProject.outputs.azureStorageConnection
    cosmosDbConnection: aiProject.outputs.cosmosDbConnection
    accountCapHost: '${accountCapabilityHostName}${uniqueSuffix}'
    projectCapHost: '${projectCapabilityHostName}${uniqueSuffix}'
    enableAzureSearch: enableAzureSearch
    enableCosmosDb: enableCosmosDb
  }
  dependsOn: [ projectRoleAssignments ]
}

module postCapabilityHostCreationRoleAssignments './agent/post-capability-host-role-assignments.bicep' = if (enableAzureSearch && enableCosmosDb) {
  name: 'postcap${projectName}'
  scope: rg
  params: {
    aiProjectPrincipalId: aiProject.outputs.aiProjectPrincipalId
    aiProjectWorkspaceId: aiProject.outputs.projectWorkspaceId
    aiStorageAccountName: aiDependencies.outputs.storageAccountName
    cosmosDbAccountName: aiDependencies.outputs.cosmosDbAccountName
    enableCosmosDb: enableCosmosDb
  }
  dependsOn: [ aiProjectCapabilityHost ]
}

// Define the configuration object locally to pass to the modules
var storageEndpointConfig = {
  enableBlob: true
  enableQueue: true
  enableTable: false
  enableFiles: false
  allowUserIdentityPrincipal: !empty(principalId)
}

// Consolidated Role Assignments
module rbac 'app/rbac.bicep' = {
  name: 'rbacAssignments'
  scope: rg
  params: {
    storageAccountName: storage.outputs.name
    appInsightsName: monitoring.outputs.applicationInsightsName
    managedIdentityPrincipalId: apiUserAssignedIdentity.outputs.principalId
    userIdentityPrincipalId: principalId
    enableBlob: storageEndpointConfig.enableBlob
    enableQueue: storageEndpointConfig.enableQueue
    enableTable: storageEndpointConfig.enableTable
    allowUserIdentityPrincipal: storageEndpointConfig.allowUserIdentityPrincipal
  }
}

// App outputs
output APPLICATIONINSIGHTS_CONNECTION_STRING string = monitoring.outputs.applicationInsightsConnectionString
output AZURE_LOCATION string = location
output AZURE_TENANT_ID string = tenant().tenantId
output SERVICE_API_NAME string = api.outputs.SERVICE_API_NAME
output SERVICE_API_URI string = 'https://${api.outputs.SERVICE_API_NAME}.azurewebsites.net'
output AZURE_FUNCTION_APP_NAME string = api.outputs.SERVICE_API_NAME
output RESOURCE_GROUP string = rg.name
output STORAGE_ACCOUNT_NAME string = storage.outputs.name
output AI_SERVICES_NAME string = aiDependencies.outputs.aiServicesName

// AI Foundry outputs
output PROJECT_ENDPOINT string = aiProject.outputs.projectEndpoint
output MODEL_DEPLOYMENT_NAME string = modelDeploymentName
output AZURE_OPENAI_ENDPOINT string = 'https://${aiDependencies.outputs.aiServicesName}.openai.azure.com/'
output AZURE_OPENAI_DEPLOYMENT_NAME string = modelDeploymentName
output AZURE_CLIENT_ID string = apiUserAssignedIdentity.outputs.clientId
output STORAGE_CONNECTION__queueServiceUri string = 'https://${storage.outputs.name}.queue.${environment().suffixes.storage}'

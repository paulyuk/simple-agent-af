# Deployment Guide

This document explains the deployment options for the Simple Agent Framework.

## Deployment Options

### Option 1: Azure Developer CLI (azd) - Recommended

This is the easiest way to deploy the application with all required resources.

```bash
# Install azd if you haven't already
# See: https://learn.microsoft.com/azure/developer/azure-developer-cli/install-azd

# Clone the repository
git clone https://github.com/paulyuk/simple-agent-af.git
cd simple-agent-af

# Login to Azure
azd auth login

# Deploy everything
azd up
```

**What it does:**
- Provisions all Azure resources using the Bicep templates in `/infra`
- Deploys the .NET application code
- Configures all necessary environment variables
- Sets up role assignments and managed identities
- Runs post-provision scripts to set up local development environment

**Resources created:**
- Azure Functions app (Flex Consumption plan)
- Azure AI Services with GPT-4.1-mini model deployment
- Storage Account (for functions and AI workspace)
- Application Insights and Log Analytics
- All necessary managed identities and role assignments
- Optional: Azure AI Search (disabled by default, enable with `enableAzureSearch=true`)
- Optional: Cosmos DB for agent thread storage (disabled by default, enable with `enableCosmosDb=true`)

### Option 2: Deploy to Azure Button (One-Click Deployment)

Use the "Deploy to Azure" button for quick deployments without installing additional tools.

[![Deploy to Azure](https://aka.ms/deploytoazurebutton)](https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2Fpaulyuk%2Fsimple-agent-af%2Fmain%2Finfra%2Fdeploybutton%2Fazuredeploy.json)

See [infra/deploybutton/README.md](infra/deploybutton/README.md) for more details.

**Note:** The ARM template is auto-generated from the Bicep files when changes are pushed to the main branch.

### Option 3: Manual Bicep Deployment

If you prefer to deploy using Azure CLI directly:

```bash
# Login to Azure
az login

# Set your subscription
az account set --subscription <your-subscription-id>

# Create a resource group
az group create --name rg-simple-agent --location eastus

# Deploy the Bicep template
az deployment sub create \
  --location eastus \
  --template-file infra/main.bicep \
  --parameters environmentName=simple-agent location=eastus
```

## Configuration

### Required Parameters

- **environmentName**: A unique name for your environment (used to generate resource names)
- **location**: The Azure region where resources will be deployed

### Optional Parameters

All other parameters have secure defaults, but you can override them:

- **modelName**: Default is "gpt-4.1-mini"
- **modelVersion**: Default is "2024-07-18"
- **modelSkuName**: Default is "GlobalStandard"
- **modelCapacity**: Default is 50
- **enableAzureSearch**: Default is `false` (set to `true` to enable Azure AI Search for vector store)
- **enableCosmosDb**: Default is `false` (set to `true` to enable Cosmos DB for agent thread storage)

See [infra/main.bicep](infra/main.bicep) for the full list of parameters.

## After Deployment

### For Local Development

After deploying with `azd up`:

1. The post-provision script automatically creates a `local.settings.json` file
2. Run the application locally:
   ```bash
   dotnet run
   ```

### Get Configuration Values

To get the deployed configuration:

```bash
# Using azd
azd env get-values

# Or using Azure CLI
az functionapp config appsettings list \
  --name <your-function-app-name> \
  --resource-group <your-resource-group>
```

## Updating the Deployment

To update an existing deployment:

```bash
# Using azd
azd up

# Or using Azure CLI
az deployment sub create \
  --location eastus \
  --template-file infra/main.bicep \
  --parameters @infra/main.parameters.json
```

## Cleaning Up

To delete all resources:

```bash
# Using azd
azd down

# Or using Azure CLI
az group delete --name <your-resource-group> --yes
```

## Troubleshooting

### Common Issues

1. **Insufficient permissions**: Ensure you have Owner or Contributor + User Access Administrator roles
2. **Resource name conflicts**: Use a unique environmentName to avoid naming conflicts
3. **Region availability**: Some regions may not support all required services (Flex Consumption, AI services)

### Logs and Monitoring

- Application Insights: Monitor your function app performance and logs
- Log Analytics: Query logs across all resources
- Function App logs: `az functionapp logs tail --name <app-name> --resource-group <rg-name>`

## Architecture

The deployment creates the following architecture:

```
Azure Subscription
└── Resource Group
    ├── Azure Functions (Flex Consumption)
    ├── Azure AI Services (with GPT-4o-mini)
    ├── Azure AI Search
    ├── Cosmos DB
    ├── Storage Account
    ├── Application Insights
    ├── Log Analytics Workspace
    └── Managed Identities
```

All resources are connected using managed identities and role-based access control (RBAC) for secure, passwordless authentication.

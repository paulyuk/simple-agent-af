# Deploy to Azure Button

This folder contains the compiled ARM template for one-click deployment to Azure.

> **Note**: The ARM template will be automatically generated from the Bicep files when changes are pushed to the main branch. The GitHub Actions workflow compiles `infra/main.bicep` to `infra/deploybutton/azuredeploy.json`.

## Deploy to Azure

Once the ARM template is generated, you can deploy using this button:

[![Deploy to Azure](https://aka.ms/deploytoazurebutton)](https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2Fpaulyuk%2Fsimple-agent-af%2Fmain%2Finfra%2Fdeploybutton%2Fazuredeploy.json)

## Manual Compilation

If you want to manually compile the Bicep to ARM template:

```bash
cd infra
az bicep build --file main.bicep --outfile deploybutton/azuredeploy.json
```

Or using the Bicep CLI directly:

```bash
cd infra
bicep build main.bicep --outfile deploybutton/azuredeploy.json
```

## Parameters

When deploying, you will be asked to provide:

- **environmentName**: A unique name for your environment (used to generate resource names)
- **location**: The Azure region where resources will be deployed

All other parameters use secure defaults optimized for the Simple Agent Framework:
- Model: GPT-4o-mini (2024-07-18)
- Model SKU: GlobalStandard
- Model Capacity: 50

## What Gets Deployed

- Azure Functions app (Flex Consumption plan with .NET 9)
- Azure AI Services with GPT-4o-mini deployment
- Azure AI Search
- Cosmos DB (for agent state/thread storage)
- Storage Account (for function app and AI workspace)
- Application Insights and Log Analytics (for monitoring)
- All necessary role assignments and managed identities

# Deploy to Azure Button

This folder contains the compiled ARM template for one-click deployment to Azure.

[![Deploy to Azure](https://aka.ms/deploytoazurebutton)](https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2Fpaulyuk%2Fsimple-agent-af%2Fmain%2Finfra%2Fdeploybutton%2Fazuredeploy.json)

## Parameters

When deploying, you will be asked to provide:

- **environmentName**: A unique name for your environment (used to generate resource names)
- **location**: The Azure region where resources will be deployed

All other parameters use secure defaults optimized for the Simple Agent Framework:
- Model: GPT-4.1-mini (2025-04-14)
- Model SKU: GlobalStandard
- Model Capacity: 50

## What Gets Deployed

- Azure Functions app (Flex Consumption plan)
- Azure AI Services with GPT-4.1-mini deployment
- Azure AI Search
- Cosmos DB (for agent state)
- Storage Account
- Application Insights and Log Analytics
- All necessary role assignments and managed identities

# Simple Agent Framework Quickstart (.NET 10)

A simple AI agent application using Azure OpenAI with the Azure.AI.OpenAI SDK and Microsoft Agent Framework 2.0.

## Description

This application demonstrates how to create a simple AI agent using the Azure OpenAI SDK configured for Azure OpenAI endpoints. The agent is configured with robot directives and provides an interactive conversational loop.

<img width="450" height="450" alt="image" src="https://github.com/user-attachments/assets/b379cb39-ba54-4b76-9b5d-1847f5da1e77" />

## Deploy to Azure

### Option 1: One-Click Deploy (Coming Soon)

[![Deploy to Azure](https://aka.ms/deploytoazurebutton)](https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2Fpaulyuk%2Fsimple-agent-af%2Fmain%2Finfra%2Fdeploybutton%2Fazuredeploy.json)

> **Note**: The ARM template will be automatically generated when the main branch is updated. See [infra/deploybutton/README.md](infra/deploybutton/README.md) for details.

### Option 2: Azure Developer CLI (Recommended)

Deploy this application to Azure using Azure Developer CLI (azd):

```bash
# Install Azure Developer CLI if you haven't already
# See: https://learn.microsoft.com/azure/developer/azure-developer-cli/install-azd

# Login to Azure
azd auth login

# Provision and deploy the application
azd up
```

The deployment will:
- Create an Azure Functions app with .NET 9 runtime
- Set up Azure AI services with GPT-4o-mini model
- Configure Azure AI Search and Cosmos DB for agent capabilities
- Create all necessary storage and monitoring resources

## Prerequisites

### For Azure Deployment
- [Azure Developer CLI (azd)](https://learn.microsoft.com/azure/developer/azure-developer-cli/install-azd)
- Azure subscription

### For Local Development
- .NET 10
- Azure OpenAI service endpoint and deployment
- Azure CLI for authentication

## Environment Variables

Set the following environment variables:

- `AZURE_OPENAI_ENDPOINT`: Your Azure OpenAI service endpoint
- `AZURE_OPENAI_DEPLOYMENT_NAME`: Your deployment name (optional, defaults to "gpt-4.1-mini")

## Dependencies

- Azure.AI.OpenAI (2.1.0) - Azure OpenAI SDK
- Azure.Identity (1.17.0)  
- Microsoft.Agents.AI.OpenAI (1.0.0-preview.251204.1)

**Note**: This project uses the Azure.AI.OpenAI SDK (which is built on the official OpenAI SDK) to connect to Azure OpenAI endpoints.

## Local Development Usage

After deploying to Azure or setting up your own Azure OpenAI resources:

1. Clone the repository
2. Set the required environment variables (or use `azd env get-values` after deployment)
3. Run the application:

```bash
dotnet run
```
4. Enter a message like `what are the laws?`

The application will start an interactive conversation loop where you can ask questions. Type 'exit' or 'quit' to end the session.

## Authentication

The application uses `DefaultAzureCredential` for authentication. Make sure you're logged in to Azure CLI:

```bash
az login
```

## Learn More

For more information about building Azure AI Agents using Agent Framework 2.0, see the [official documentation](https://learn.microsoft.com/en-us/agent-framework/).

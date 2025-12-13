# Simple Agent Framework Quickstart (.NET 10)

A simple AI agent application using Azure OpenAI with the Azure.AI.OpenAI SDK and Microsoft Agent Framework 2.0.

## Description

This application demonstrates how to create a simple AI agent using the Azure OpenAI SDK configured for Azure OpenAI endpoints. The agent is configured with robot directives and provides an interactive conversational loop.

<img width="450" height="450" alt="image" src="https://github.com/user-attachments/assets/b379cb39-ba54-4b76-9b5d-1847f5da1e77" />

## Prerequisites

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

## Usage

1. Clone the repository
2. Set the required environment variables
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

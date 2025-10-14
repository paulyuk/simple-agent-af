# Simple Agent AF

A simple AI agent application using Azure OpenAI and Microsoft.Agents.AI framework.

## Description

This application demonstrates how to create a simple AI agent that can generate jokes using Azure OpenAI. The agent is configured to be good at telling jokes and provides both synchronous and streaming response capabilities.

## Prerequisites

- .NET 9.0 or later
- Azure OpenAI service endpoint and deployment
- Azure CLI for authentication

## Environment Variables

Set the following environment variables:

- `AZURE_OPENAI_ENDPOINT`: Your Azure OpenAI service endpoint
- `AZURE_OPENAI_DEPLOYMENT_NAME`: Your deployment name (optional, defaults to "gpt-4.1-mini")

## Dependencies

- Azure.AI.OpenAI (2.1.0)
- Azure.Identity (1.17.0)  
- Microsoft.Agents.AI.OpenAI (1.0.0-preview.251009.1)

## Usage

1. Clone the repository
2. Set the required environment variables
3. Run the application:

```bash
dotnet run
```

The application will ask the AI agent to tell a joke about a pirate, first with a synchronous call and then with streaming output.

## Authentication

The application uses `AzureCliCredential` for authentication. Make sure you're logged in to Azure CLI:

```bash
az login
```
# Simple Agent QuickStart (.NET Copilot SDK)

A simple AI agent built with the GitHub Copilot SDK, running as an Azure Function.

## Prerequisites

- [.NET 10.0+](https://dotnet.microsoft.com/download)
- [Azure Functions Core Tools](https://learn.microsoft.com/en-us/azure/azure-functions/functions-run-local#install-the-azure-functions-core-tools)
- [Azure Developer CLI (azd)](https://aka.ms/azd-install) (only needed for deploying Microsoft Foundry resources)
- Access to an AI model via one of:
  - **GitHub Copilot subscription** — models are available automatically
  - **Bring Your Own Key (BYOK)** — use an API key from [Microsoft Foundry](https://ai.azure.com) (see [BYOK docs](https://github.com/github/copilot-sdk/blob/main/docs/auth/byok.md))

## Deploy Microsoft Foundry Resources (if needed)

If you're using BYOK and don't already have a Microsoft Foundry project with a model deployed:

```bash
azd auth login
azd up
```

This provisions all resources and configures local development automatically.

### What Gets Deployed

- Microsoft Foundry project with GPT-5-mini model
- Azure Functions app (.NET 10, Flex Consumption plan)
- Storage, monitoring, and all necessary RBAC role assignments
- Optional: Search for vector store (disabled by default)
- Optional: Cosmos DB for agent thread storage (disabled by default)

## Quickstart

1. Clone the repository

2. Run the function locally:

   ```bash
   cd agent
   func start
   ```

3. Test the agent (in a new terminal):

   ```bash
   # Interactive chat client
   dotnet run --project chat/Chat.csproj

   # Or use curl directly
   curl -X POST http://localhost:7071/api/ask -d "what are the laws"
   ```

   Set `AGENT_URL` to point to a deployed instance:

   ```bash
   AGENT_URL=https://<your-function-app>.azurewebsites.net dotnet run --project chat/Chat.csproj
   ```

## Source Code

The agent logic is in [`agent/Ask.cs`](agent/Ask.cs). It creates a `CopilotClient`, configures a session with a system message (Asimov's Three Laws of Robotics), and exposes an HTTP endpoint (`/api/ask`) that accepts a prompt and returns the agent's response.

[`chat/Chat.cs`](chat/Chat.cs) is a lightweight .NET console client that POSTs messages to the function in a loop, giving you an interactive chat experience. It defaults to `http://localhost:7071` but can be pointed at a deployed instance via the `AGENT_URL` environment variable.

## Using Microsoft Foundry (BYOK)

By default the agent uses GitHub Copilot's models. To use your own model from Microsoft Foundry instead, set these environment variables:

```bash
export AZURE_OPENAI_ENDPOINT="https://<your-ai-services>.openai.azure.com/"
export AZURE_OPENAI_API_KEY="<your-api-key>"
export AZURE_OPENAI_MODEL="gpt-5-mini"  # optional, defaults to gpt-5-mini
```

**Getting these values:**
- If you ran `azd up`, the endpoint is already in your environment — run `azd env get-values | grep AZURE_OPENAI_ENDPOINT`
- For the API key, go to [Azure Portal](https://portal.azure.com) → your AI Services resource → **Keys and Endpoint** → select the **Azure OpenAI** tab
- Or find both in the [Microsoft Foundry portal](https://ai.azure.com) under your project settings

See the [BYOK docs](https://github.com/github/copilot-sdk/blob/main/docs/auth/byok.md) for details.

## Learn More

- [GitHub Copilot SDK](https://github.com/github/copilot-sdk)
- [Copilot SDK .NET docs](https://github.com/github/copilot-sdk/tree/main/dotnet)
- [BYOK (Bring Your Own Key)](https://github.com/github/copilot-sdk/blob/main/docs/auth/byok.md)
- [Azure Developer CLI](https://learn.microsoft.com/en-us/azure/developer/azure-developer-cli/)

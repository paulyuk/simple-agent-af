# Simple Agent QuickStart (.NET Copilot SDK)

A simple AI agent built with the GitHub Copilot SDK, running as a .NET console app.

## Prerequisites

- [.NET 8.0+](https://dotnet.microsoft.com/download)
- [Azure Developer CLI (azd)](https://aka.ms/azd-install) (only needed for deploying Azure resources)
- Access to an AI model via one of:
  - **GitHub Copilot subscription** — models are available automatically
  - **Bring Your Own Key (BYOK)** — use an API key from [Microsoft Foundry](https://ai.azure.com) (see [BYOK docs](https://github.com/github/copilot-sdk/blob/main/docs/auth/byok.md))

## Deploy Azure AI Resources (if needed)

If you're using BYOK and don't already have a Microsoft Foundry AI project with a model deployed, use one of these options:

### Option 1: Azure Developer CLI (Recommended)

Provisions all Azure resources and configures local development automatically:

```bash
azd auth login
azd up
```

### Option 2: One-Click Deploy

[![Deploy to Azure](https://aka.ms/deploytoazurebutton)](https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2Fpaulyuk%2Fsimple-agent-af%2Fmain%2Finfra%2Fdeploybutton%2Fazuredeploy.json)

> **Important:** Fill in the **Principal ID** field with your user object ID from the [Entra blade](https://ms.portal.azure.com/#view/Microsoft_AAD_IAM/ActiveDirectoryMenuBlade/~/Overview) and give the environment a memorable name.

### What Gets Deployed

- Microsoft Foundry AI Project with GPT-5-mini model
- Storage, monitoring, and all necessary RBAC role assignments
- Optional: Azure AI Search for vector store (disabled by default)
- Optional: Cosmos DB for agent thread storage (disabled by default)

## Quickstart

1. Clone the repository

2. Install dependencies:

   ```bash
   dotnet restore
   ```

3. Run the agent:

   ```bash
   dotnet run
   ```

4. Enter a message like `what are the laws?` — type `exit` or `quit` to end the session.

## Source Code

The agent logic is in [`Program.cs`](Program.cs). It creates a `CopilotClient`, configures a session with a system message (Asimov's Three Laws of Robotics), and runs an interactive conversation loop where user input is sent to the agent and responses are printed.

## Using Azure Foundry (BYOK)

By default the agent uses GitHub Copilot's models. To use your own model from Microsoft Foundry instead, set these environment variables:

```bash
export AZURE_OPENAI_ENDPOINT="https://<your-ai-services>.openai.azure.com/"
export AZURE_OPENAI_API_KEY="<your-api-key>"
export AZURE_OPENAI_MODEL="gpt-5-mini"  # optional, defaults to gpt-5-mini
```

**Getting these values:**
- If you ran `azd up`, the endpoint is already in your environment — run `azd env get-values | grep AZURE_OPENAI_ENDPOINT`
- For the API key, go to [Azure Portal](https://portal.azure.com) → your AI Services resource → **Keys and Endpoint** → select the **Azure OpenAI** tab
- Or find both in the [Azure AI Foundry portal](https://ai.azure.com) under your project settings

See the [BYOK docs](https://github.com/github/copilot-sdk/blob/main/docs/auth/byok.md) for details.

## Learn More

- [GitHub Copilot SDK](https://github.com/github/copilot-sdk)
- [Copilot SDK .NET docs](https://github.com/github/copilot-sdk/tree/main/dotnet)
- [BYOK (Bring Your Own Key)](https://github.com/github/copilot-sdk/blob/main/docs/auth/byok.md)
- [Azure Developer CLI](https://learn.microsoft.com/en-us/azure/developer/azure-developer-cli/)

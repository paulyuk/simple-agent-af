# Simple Agent Framework Quickstart (.NET 10)

A simple AI agent application using Azure OpenAI with the Azure.AI.OpenAI SDK and Microsoft Agent Framework 2.0.

## Description

This application demonstrates how to create a simple AI agent using the Azure OpenAI SDK configured for Azure OpenAI endpoints. The agent is configured with robot directives and provides an interactive conversational loop. It also includes an example of connecting to a remote MCP (Model Context Protocol) server to enable additional tools and capabilities.

<img width="450" height="450" alt="image" src="https://github.com/user-attachments/assets/b379cb39-ba54-4b76-9b5d-1847f5da1e77" />

## Deploy to Azure

### Quick Start Options

1. **One-Click Deploy**: [![Deploy to Azure](https://aka.ms/deploytoazurebutton)](https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2Fpaulyuk%2Fsimple-agent-af%2Fmain%2Finfra%2Fdeploybutton%2Fazuredeploy.json)
   
2. **Azure Developer CLI** (Recommended):
   ```bash
   azd auth login
   azd up
   ```

For detailed deployment instructions, see [DEPLOYMENT.md](DEPLOYMENT.md).

### What Gets Deployed

- Azure Functions app with .NET 9 runtime (Flex Consumption plan)
- Azure AI services with GPT-4.1-mini model
- Storage, monitoring, and all necessary role assignments
- Optional: Azure AI Search (for vector store capabilities, disabled by default)
- Optional: Cosmos DB (for agent thread storage, disabled by default)

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
- `AZURE_OPENAI_DEPLOYMENT_NAME`: Your deployment name (optional, defaults to `chat` assuming a model deployment of gpt-4.1-mini or similar)

## Dependencies

- Azure.AI.OpenAI (2.1.0) - Azure OpenAI SDK
- Azure.Identity (1.17.0)  
- Microsoft.Agents.AI.OpenAI (1.0.0-preview.251204.1)
- Microsoft.Extensions.AI (10.0.1) - For MCP server support

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

## MCP (Model Context Protocol) Server Integration

This application includes examples of connecting to remote MCP servers to enable additional tools and capabilities for the agent. The examples demonstrate both standard HTTP and OAuth-authenticated connections.

### Features

- **Remote MCP Servers**: Connects to hosted MCP servers via HTTP/HTTPS
- **OAuth Support**: Automatically handles OAuth authentication when using `api.githubcopilot.com`
- **Tool Management**: Control which tools are available and approval requirements
- **Flexible Configuration**: Can be customized for different MCP servers

### Configuration Examples

The code demonstrates how to configure multiple remote MCP servers:

```csharp
var mcpTools = new List<AITool>
{
    // Example 1: Microsoft Learn MCP server (HTTP)
    new HostedMcpServerTool("microsoft-learn", new Uri("https://learn.microsoft.com/mcp/"))
    {
        ApprovalMode = HostedMcpServerToolApprovalMode.NeverRequire
    },
    
    // Example 2: GitHub MCP server (HTTP with OAuth)
    new HostedMcpServerTool("github", new Uri("https://api.githubcopilot.com/mcp/"))
    {
        // OAuth is handled automatically by the service when using api.githubcopilot.com
        ApprovalMode = HostedMcpServerToolApprovalMode.NeverRequire
    }
};

AIAgent agent = openAIClient
    .GetChatClient(deploymentName)
    .CreateAIAgent(instructions, tools: mcpTools);
```

### Customization Options

- **Server Address**: Change the URI to connect to different MCP servers
- **Approval Mode**: Control when user approval is required for tool calls
  - `NeverRequire`: Tools execute without approval
  - `AlwaysRequire`: All tools require approval
  - `RequireSpecific()`: Specify approval per tool
- **Allowed Tools**: Restrict which tools the agent can use
- **Authorization Token**: Add custom authorization if needed

For more information about MCP in Agent Framework, see the [MCP documentation](https://learn.microsoft.com/en-us/agent-framework/user-guide/model-context-protocol/using-mcp-tools).

## Learn More

For more information about building Azure AI Agents using Agent Framework 2.0, see the [official documentation](https://learn.microsoft.com/en-us/agent-framework/).

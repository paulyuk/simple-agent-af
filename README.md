# Azure Functions AI Agent with .NET 9

This repository contains an AI agent HTTP trigger sample built with Azure Functions using .NET 9 isolated worker process. The sample integrates with Azure OpenAI and uses Microsoft.Agents.AI framework to provide intelligent responses to user messages.

## Features

- ✅ **Azure Functions Flex Consumption** - Serverless, cost-effective execution
- ✅ **.NET 9 Support** - Latest .NET framework with preview Azure Functions packages
- ✅ **Azure OpenAI Integration** - Powered by Microsoft.Agents.AI framework
- ✅ **Modern ASP.NET Patterns** - Using `IActionResult`, `HttpRequest`, and dependency injection
- ✅ **Comprehensive Health Checks** - Real end-to-end system validation
- ✅ **Structured Logging** - Built-in Application Insights integration
- ✅ **Secure by Default** - Uses Azure managed identity for authentication

## Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Azure Functions Core Tools](https://learn.microsoft.com/azure/azure-functions/functions-run-local?pivots=programming-language-csharp#install-the-azure-functions-core-tools) v4.x
- [Azure CLI](https://docs.microsoft.com/cli/azure/install-azure-cli) (for deployment)
- **Azure OpenAI Service** with a deployed model
- To use Visual Studio Code:
  - [Visual Studio Code](https://code.visualstudio.com/)
  - [Azure Functions extension](https://marketplace.visualstudio.com/items?itemName=ms-azuretools.vscode-azurefunctions)

## Quick Start

### 1. Clone and Setup

```bash
git clone https://github.com/paulyuk/simple-agent-af.git
cd simple-agent-af
```

### 2. Configure Environment

Create or update `local.settings.json` in the project root:

```json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
    "AZURE_OPENAI_ENDPOINT": "https://your-openai-service.openai.azure.com/",
    "AZURE_OPENAI_DEPLOYMENT_NAME": "gpt-4o-mini"
  }
}
```

### 3. Customize Instructions

Edit `instructions.md` to customize your AI agent's behavior:

```markdown
You are a helpful AI assistant that specializes in technical questions.
Be concise and accurate in your responses.

Directives of robots:
1) A robot may not injure a human being...
2) A robot must obey orders given it by human beings...
3) A robot must protect its own existence...

Objective: Give me the TLDR in exactly 5 words.

```

### 4. Run Locally

```bash
# Start the Azure Functions host
func start
```

The function will be available at:
- **Health Check**: `GET http://localhost:7071/api/health`
- **Chat**: `POST http://localhost:7071/api/chat`

### 5. Test the Functions

Using curl:

```bash
# Health check
curl http://localhost:7071/api/health

# Chat with the AI agent
curl -X POST http://localhost:7071/api/chat \
  -H "Content-Type: application/json" \
  -d '{"message": "What are the three laws of robotics?"}'
```

Or use the included `test.http` file with VS Code REST Client extension.

## API Reference

### Health Endpoint

**GET** `/api/health`

Returns comprehensive health status including:
- AI Agent service availability
- Azure OpenAI connectivity test
- Configuration validation
- System status

**Response (200 OK):**
```json
{
  "Status": "Healthy",
  "Timestamp": "2025-10-14T22:10:00Z",
  "Checks": {
    "AIAgentService": { "Status": "Healthy" },
    "AzureOpenAI": { "Status": "Healthy", "ResponseLength": 156 },
    "Configuration": { 
      "Status": "Healthy", 
      "HasEndpoint": true,
      "HasDeployment": true,
      "Deployment": "gpt-4o-mini"
    }
  }
}
```

### Chat Endpoint

**POST** `/api/chat`

Processes user messages through the AI agent.

**Request Body:**
```json
{
  "message": "Your question or message here"
}
```

**Response (200 OK):**
```json
{
  "response": "AI agent's response to your message",
  "success": true
}
```

**Error Response (400/500):**
```json
{
  "response": "Error message",
  "success": false
}
```

## Configuration

### Environment Variables

| Variable | Required | Default | Description |
|----------|----------|---------|-------------|
| `AZURE_OPENAI_ENDPOINT` | ✅ | - | Your Azure OpenAI service endpoint |
| `AZURE_OPENAI_DEPLOYMENT_NAME` | ❌ | `gpt-4o-mini` | Deployment/model name to use |
| `AzureWebJobsStorage` | ✅ | `UseDevelopmentStorage=true` | Storage account for Azure Functions |
| `FUNCTIONS_WORKER_RUNTIME` | ✅ | `dotnet-isolated` | Functions runtime |

### Files

- `instructions.md` - Contains the AI agent's system instructions
- `host.json` - Azure Functions host configuration
- `local.settings.json` - Local development settings (not committed)

## Project Structure

```
simple-agent-af/
├── AgentFunction.cs          # HTTP trigger functions
├── AIAgentService.cs         # AI agent service implementation
├── Program.cs               # Application entry point and DI setup
├── instructions.md          # AI agent instructions
├── host.json               # Azure Functions configuration
├── local.settings.json     # Local environment settings
├── test.http              # HTTP test file for VS Code
├── simple-agent-af.csproj # Project file
└── README.md             # This file
```

## Deployment

### Deploy to Azure

1. **Login to Azure:**
   ```bash
   az login
   ```

2. **Create Function App:**
   ```bash
   az functionapp create \
     --resource-group myResourceGroup \
     --consumption-plan-location eastus \
     --runtime dotnet-isolated \
     --functions-version 4 \
     --name mySimpleAgentApp \
     --storage-account mystorageaccount
   ```

3. **Configure App Settings:**
   ```bash
   az functionapp config appsettings set \
     --name mySimpleAgentApp \
     --resource-group myResourceGroup \
     --settings "AZURE_OPENAI_ENDPOINT=https://your-service.openai.azure.com/" \
                "AZURE_OPENAI_DEPLOYMENT_NAME=gpt-4o-mini"
   ```

4. **Deploy Code:**
   ```bash
   func azure functionapp publish mySimpleAgentApp
   ```

### Using Azure Developer CLI (azd)

For a complete infrastructure-as-code deployment, see the [Azure Functions quickstart with azd](https://github.com/Azure-Samples/functions-quickstart-dotnet-azd).

## Architecture

This solution uses:
- **Azure Functions Isolated Worker Process** - Better performance and .NET 9 support
- **Dependency Injection** - Clean separation of concerns
- **Microsoft.Agents.AI** - High-level AI agent framework
- **Azure OpenAI** - Enterprise-grade AI models
- **Azure Managed Identity** - Secure authentication
- **Application Insights** - Monitoring and telemetry

## Troubleshooting

### Common Issues

1. **Function hangs on startup**
   - Ensure you're using .NET 9 compatible packages
   - Check that `instructions.md` is being copied to output directory

2. **Azure OpenAI connection fails**
   - Verify `AZURE_OPENAI_ENDPOINT` is correct
   - Ensure your deployment name matches the actual model deployment
   - Check Azure credentials and permissions

3. **Build warnings about preview packages**
   - These are expected when using .NET 9 with Azure Functions
   - Warnings don't affect functionality

### Debug Mode

Run with verbose logging:
```bash
func start --verbose
```

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Test thoroughly
5. Submit a pull request

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Related Resources

- [Azure Functions Documentation](https://docs.microsoft.com/azure/azure-functions/)
- [Azure OpenAI Service](https://azure.microsoft.com/products/ai-services/openai-service)
- [Microsoft.Agents.AI Documentation](https://github.com/microsoft/agents)
- [.NET 9 Documentation](https://docs.microsoft.com/dotnet/core/whats-new/dotnet-9)
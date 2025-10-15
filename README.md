# Hosting AI Agents with Agent Framework 2.0, Azure Functions, and Azure AI Foundry (.NET 9)

This repository contains a production-ready AI agent built with Agent Framework 2.0, Azure AI Foundry models, Azure Functions using .NET 9 isolated worker process and comprehensive Azure AI infrastructure. The solution demonstrates enterprise-grade patterns for building, deploying, and managing AI agents in Azure using Infrastructure as Code.

## What You'll Learn

This project demonstrates how to:
- **Build AI Agents** with Agent Framework 2.0, Azure AI Foundry models, Azure Functions Flex Consumption hosting, and .NET 9
- **Implement Enterprise Security** using Azure Managed Identity and RBAC
- **Deploy Complete AI Infrastructure** with Azure Developer CLI and Bicep
- **Follow Azure Best Practices** for scalable, secure, and maintainable solutions
- **Integrate Multiple AI Services** including Azure OpenAI, AI Search, and Cosmos DB
- **Automate Everything** from local development to production deployment

## Features

- ✅ **Azure Functions Flex Consumption** - Serverless, cost-effective execution with .NET 9
- ✅ **Complete AI Infrastructure** - Azure OpenAI, AI Search, Cosmos DB, and Storage
- ✅ **Enterprise Security** - Managed Identity, RBAC, policy compliance (no local auth)
- ✅ **Infrastructure as Code** - Bicep templates with Azure Verified Modules
- ✅ **Automated Deployment** - Azure Developer CLI with post-provision automation
- ✅ **Production Ready** - Comprehensive health checks, monitoring, and diagnostics
- ✅ **Development Experience** - Local development with automatic cloud resource configuration
- ✅ **Microsoft.Agents.AI** - Official [Microsoft AI Agent Framework 2.0](https://learn.microsoft.com/en-us/agent-framework/) integration

## Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Azure Functions Core Tools](https://learn.microsoft.com/azure/azure-functions/functions-run-local) v4.x
- [Azure Developer CLI](https://learn.microsoft.com/azure/developer/azure-developer-cli/install-azd) (azd)
- Azure subscription with permissions to create resources
- To use Visual Studio Code:
  - [Visual Studio Code](https://code.visualstudio.com/)
  - [Azure Functions extension](https://marketplace.visualstudio.com/items?itemName=ms-azuretools.vscode-azurefunctions)

## 🚀 Quick Start

This project uses Azure Developer CLI (azd) to automate everything - from creating Azure resources to configuring your local development environment.

### 1. Clone and Setup

```bash
git clone https://github.com/paulyuk/simple-agent-af.git
cd simple-agent-af
```

### 2. Provision Azure Resources (Recommended First Step)

**Important**: Run `azd provision` first to create all Azure resources and automatically configure your local development environment:

```bash
# Login to Azure
azd auth login

# Create resources and configure local development
azd provision
```

This command:
- ✅ Creates complete AI infrastructure (Azure OpenAI, AI Search, Cosmos DB, Storage, Function App)
- ✅ Configures managed identity authentication and RBAC permissions
- ✅ Automatically generates `local.settings.json` with all required settings
- ✅ Sets up your local development environment to connect to cloud resources

### 3. Run Locally with Cloud Resources

```bash
# Start the Azure Functions host (uses cloud resources)
func start
```

The function will be available at:
- **Health Check**: `GET http://localhost:7071/api/health`
- **Chat**: `POST http://localhost:7071/api/chat`

### 4. Deploy to Azure

```bash
# Deploy your code to the provisioned infrastructure
azd deploy
```

Or do both steps at once:

```bash
# Provision infrastructure AND deploy code
azd up
```

## 🎯 Main Commands

Once you have the project set up, these are the two main commands you'll use:

```bash
# For local development with cloud resources
func start

# For complete deployment (infrastructure + code)
azd up
```

### Manual Configuration (Alternative)

If you prefer to configure manually or use your own Azure OpenAI service, create `local.settings.json`:

```json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
    "AZURE_OPENAI_ENDPOINT": "https://your-openai-service.openai.azure.com/",
    "AZURE_OPENAI_DEPLOYMENT_NAME": "gpt-4o-mini",
    "AZURE_CLIENT_ID": "your-managed-identity-client-id"
  }
}
```

### Customize AI Agent Instructions

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

## 🏗️ What Gets Deployed

The `azd up` command creates a complete AI agent infrastructure:

### Core Infrastructure
- **Function App** (Flex Consumption plan with .NET 9)
- **Storage Account** (with blob, queue support and managed identity auth)
- **Application Insights** (monitoring and telemetry)
- **Log Analytics Workspace** (centralized logging)

### AI Services Stack
- **Azure OpenAI** (with gpt-4o-mini model deployment)
- **Azure AI Search** (for RAG scenarios and document indexing)
- **Azure Cosmos DB** (for agent conversation storage)
- **Azure AI Project** (AI Studio workspace for management)

### Security & Compliance
- **User-Assigned Managed Identity** (for secure service-to-service auth)
- **RBAC Role Assignments** (principle of least privilege)
- **Policy Compliance** (local auth disabled on all AI services)
- **Private Networking Ready** (VNet integration available)

### Development Experience
- **Automated Local Settings** (post-provision script generates configuration)
- **Cross-Platform Scripts** (PowerShell and Bash support)
- **Firewall Configuration** (automatic client IP allowlisting for development)

## 🧪 Testing Your Deployment

### Local Testing

```bash
# Health check (verifies Azure OpenAI connectivity)
curl http://localhost:7071/api/health

# Chat with the AI agent
curl -X POST http://localhost:7071/api/chat \
  -H "Content-Type: application/json" \
  -d '{"message": "What are the three laws of robotics?"}'
```

### Production Testing

After `azd deploy`, test the live Azure deployment:

```bash
# Get your function app URL from azd output
FUNCTION_URL=$(azd env get-value SERVICE_API_URI)

# Test health check
curl $FUNCTION_URL/api/health

# Test chat functionality
curl -X POST $FUNCTION_URL/api/chat \
  -H "Content-Type: application/json" \
  -d '{"message": "Hello from Azure!"}'
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

## ⚙️ Configuration

### Automatic Configuration (Recommended)

When you run `azd provision`, the post-provision script automatically configures your development environment by:
- Generating `local.settings.json` with all required Azure resource endpoints
- Configuring managed identity authentication
- Setting up proper firewall rules for development

### Environment Variables

| Variable | Required | Auto-Configured | Description |
|----------|----------|-----------------|-------------|
| `AZURE_OPENAI_ENDPOINT` | ✅ | ✅ | Azure OpenAI service endpoint |
| `AZURE_OPENAI_DEPLOYMENT_NAME` | ✅ | ✅ | Model deployment name (gpt-4o-mini) |
| `AZURE_CLIENT_ID` | ✅ | ✅ | Managed identity client ID for authentication |
| `PROJECT_ENDPOINT` | ✅ | ✅ | Azure AI Project endpoint |
| `STORAGE_CONNECTION__*` | ✅ | ✅ | Storage account configuration with managed identity |
| `AzureWebJobsStorage` | ✅ | ❌ | Local development storage |
| `FUNCTIONS_WORKER_RUNTIME` | ✅ | ❌ | Functions runtime (dotnet-isolated) |

### Key Files

- `instructions.md` - AI agent system instructions (customizable)
- `host.json` - Azure Functions host configuration
- `local.settings.json` - Local development settings (auto-generated, not committed)
- `infra/` - Complete Bicep infrastructure templates
- `azure.yaml` - Azure Developer CLI configuration with automation hooks

## 📂 Project Structure

```
simple-agent-af/
├── AgentFunction.cs              # HTTP trigger functions (health + chat)
├── AIAgentService.cs             # AI agent service with Azure OpenAI integration
├── Program.cs                   # Application entry point and DI setup
├── instructions.md              # AI agent system instructions
├── host.json                   # Azure Functions configuration
├── local.settings.json         # Local environment settings (auto-generated)
├── test.http                  # HTTP test file for VS Code REST Client
├── simple-agent-af.csproj     # Project file with .NET 9 and AI dependencies
├── azure.yaml                 # Azure Developer CLI configuration
├── infra/                     # Complete Infrastructure as Code
│   ├── main.bicep             # Main orchestration template
│   ├── abbreviations.json     # Azure resource naming conventions
│   ├── app/                   # Application infrastructure modules
│   │   ├── api.bicep          # Function App with Flex Consumption
│   │   ├── rbac.bicep         # Role-based access control
│   │   └── vnet.bicep         # Virtual network (optional)
│   ├── agent/                 # AI services infrastructure modules
│   │   ├── standard-dependent-resources.bicep
│   │   ├── standard-ai-project.bicep
│   │   ├── standard-ai-project-role-assignments.bicep
│   │   ├── standard-ai-project-capability-host.bicep
│   │   └── post-capability-host-role-assignments.bicep
│   └── scripts/               # Automation scripts
│       ├── configure-local-env.ps1
│       └── configure-local-env.sh
└── README.md                  # This documentation
```

## 🔄 Development Workflow

### For Local Development

```bash
# 1. Provision cloud resources (one-time setup)
azd provision

# 2. Start local development 
func start

# 3. Make changes to code
# 4. Test locally with cloud resources
# 5. Deploy when ready
azd deploy
```

### For Production Deployment

```bash
# Complete infrastructure + code deployment
azd up

# Or step by step:
azd provision  # Create/update infrastructure
azd deploy     # Deploy application code
```

### Advanced Scenarios

```bash
# View all environment variables
azd env get-values

# Set custom environment variables  
azd env set CUSTOM_SETTING "value"

# View deployment logs
azd monitor --live

# Clean up all resources
azd down --force --purge
```

## 🏛️ Architecture

This solution demonstrates enterprise-grade patterns:

### Application Architecture
- **Azure Functions Isolated Worker Process** - Better performance and .NET 9 support
- **Dependency Injection** - Clean separation of concerns with `IServiceCollection`
- **Microsoft.Agents.AI** - Official Microsoft AI agent framework
- **Structured Logging** - Integration with Application Insights and Log Analytics

### Security Architecture  
- **Azure Managed Identity** - No secrets management required
- **RBAC Everywhere** - Principle of least privilege access
- **Policy Compliance** - Enterprise security policies enforced
- **Network Security** - VNet integration ready for private endpoints

### Infrastructure Architecture
- **Infrastructure as Code** - Complete Bicep templates with Azure Verified Modules
- **Automated Deployment** - Azure Developer CLI with post-provision hooks
- **Scalable Design** - Flex Consumption automatically scales to demand
- **Monitoring & Observability** - Application Insights, Log Analytics integration

### AI Architecture
- **Azure OpenAI** - Enterprise-grade AI models with content filtering
- **Azure AI Search** - Vector and hybrid search capabilities  
- **Cosmos DB** - Conversation state and session management
- **AI Project Integration** - Azure AI Studio for model management and monitoring

## 🔧 Troubleshooting

### Common Issues & Solutions

1. **"azd provision fails with authentication error"**
   ```bash
   # Login to Azure and set subscription
   azd auth login
   az account set --subscription "your-subscription-id"
   ```

2. **"Function App can't connect to Azure OpenAI"**
   - Check that managed identity has proper permissions (auto-configured by azd)
   - Verify `AZURE_CLIENT_ID` is set in Function App settings  
   - Run health check: `curl $FUNCTION_URL/api/health`

3. **"Local development can't find instructions.md"**
   - Ensure you're using .NET 9 compatible packages
   - Check that `instructions.md` is being copied to output directory

4. **"Build warnings about preview packages"**
   - These are expected when using .NET 9 with Azure Functions
   - Warnings don't affect functionality

5. **"azd deploy fails with policy violations"**
   - The template follows enterprise security best practices
   - Local authentication is disabled by default (required by many organizations)
   - All authentication uses managed identity

### Debug Commands

```bash
# Verbose function host logging
func start --verbose

# Check Azure resource status
azd monitor --live

# View all environment configuration
azd env get-values

# Check Azure OpenAI connectivity
curl http://localhost:7071/api/health
```

### Getting Help

The health endpoint (`/api/health`) provides comprehensive diagnostics including:
- ✅ AI Agent service status
- ✅ Azure OpenAI connectivity test  
- ✅ Environment variable validation
- ✅ Managed identity authentication status

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Test thoroughly
5. Submit a pull request

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 📚 Key Learnings & Best Practices

This project demonstrates several important patterns:

### 🔐 Enterprise Security
- **Always use Managed Identity** - No connection strings or API keys in configuration
- **Disable Local Auth** - Comply with enterprise security policies  
- **RBAC over Admin Keys** - Implement principle of least privilege
- **Policy Compliance** - Design templates to pass organizational policies

### 🏗️ Infrastructure as Code
- **Azure Verified Modules** - Use tested, Microsoft-maintained Bicep modules
- **Complete Automation** - Infrastructure + application deployment in one command
- **Post-Provision Hooks** - Automate local development environment setup
- **Resource Naming** - Follow Azure naming conventions consistently

### 🔄 Development Experience  
- **Cloud-First Development** - Use real Azure resources even for local development
- **Automated Configuration** - Let azd generate your local settings
- **Comprehensive Health Checks** - Implement detailed diagnostics for troubleshooting
- **Cross-Platform Scripts** - Support both PowerShell and Bash for automation

### 🤖 AI Agent Patterns
- **File Path Resolution** - Handle Azure Functions runtime paths correctly
- **Environment Variable Management** - Centralize configuration with clear precedence
- **Error Handling** - Implement proper exception handling for AI service calls
- **Monitoring Integration** - Use Application Insights for AI agent observability

## 🔗 Related Resources

- [Azure Functions Documentation](https://docs.microsoft.com/azure/azure-functions/)
- [Azure OpenAI Service](https://azure.microsoft.com/products/ai-services/openai-service)  
- [Microsoft.Agents.AI Documentation](https://github.com/microsoft/agents)
- [Azure Developer CLI](https://learn.microsoft.com/azure/developer/azure-developer-cli/)
- [Azure Verified Modules](https://azure.github.io/Azure-Verified-Modules/)
- [Azure Functions Flex Consumption](https://learn.microsoft.com/azure/azure-functions/flex-consumption-plan)
- [.NET 9 Documentation](https://docs.microsoft.com/dotnet/core/whats-new/dotnet-9)
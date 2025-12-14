using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OpenAI;

var endpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT") ?? throw new InvalidOperationException("AZURE_OPENAI_ENDPOINT is not set.");
var deploymentName = Environment.GetEnvironmentVariable("AZURE_OPENAI_DEPLOYMENT_NAME") ?? "chat";
var instructions = """
    1. A robot may not injure a human being...
    2. A robot must obey orders given it by human beings...
    3. A robot must protect its own existence...
    
    Objective: Give me the TLDR in exactly 5 words.
    """;

// Get Azure token for authentication
var tokenCredential = new DefaultAzureCredential();

// Create OpenAI client configured for Azure endpoint using AzureOpenAI factory
var openAIClient = new AzureOpenAIClient(
    new Uri(endpoint),
    tokenCredential);

// Configure MCP tools from remote MCP servers
#pragma warning disable MEAI001 // Type is for evaluation purposes only
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
#pragma warning restore MEAI001

AIAgent agent = openAIClient
    .GetChatClient(deploymentName)
    .CreateAIAgent(instructions, tools: mcpTools);

// Stay in a loop for continuous conversation
while (true)
{
    Console.Write("Enter your message: ");
    var userMessage = Console.ReadLine();
    
    // Check for exit commands
    if (string.IsNullOrWhiteSpace(userMessage) || 
        userMessage.Equals("exit", StringComparison.OrdinalIgnoreCase) || 
        userMessage.Equals("quit", StringComparison.OrdinalIgnoreCase))
    {
        Console.WriteLine("Goodbye!");
        break;
    }

    try
    {
        // Invoke the agent and output the text result.
        Console.WriteLine("\n" + await agent.RunAsync(userMessage) + "\n");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}\n");
    }
}
using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Agents.AI;
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

AIAgent agent = openAIClient
    .GetChatClient(deploymentName)
    .CreateAIAgent(instructions);

// Optional: Add MCP tool from remote URL
// agent.AddMcpServer("https://example.com/mcp-server");

// Optional: Add MCP tool with authentication
// agent.AddMcpServer("https://example.com/mcp-server", new HttpClient
// {
//     DefaultRequestHeaders = 
//     {
//         { "Authorization", "Bearer YOUR_TOKEN_HERE" }
//     }
// });

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
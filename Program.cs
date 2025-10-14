using System;
using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Agents.AI;
using OpenAI;

var endpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT") ?? throw new InvalidOperationException("AZURE_OPENAI_ENDPOINT is not set.");
var deploymentName = Environment.GetEnvironmentVariable("AZURE_OPENAI_DEPLOYMENT_NAME") ?? "gpt-4.1-mini";
var instructions = await File.ReadAllTextAsync("instructions.md");

AIAgent agent = new AzureOpenAIClient(
  new Uri(endpoint),
  new DefaultAzureCredential())
    .GetChatClient(deploymentName)
    .CreateAIAgent(instructions);

Console.WriteLine("AI Agent is ready! Type 'exit' or 'quit' to end the conversation.\n");

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

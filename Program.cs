using System;
using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Agents.AI;
using OpenAI;

var endpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT") ?? throw new InvalidOperationException("AZURE_OPENAI_ENDPOINT is not set.");
var deploymentName = Environment.GetEnvironmentVariable("AZURE_OPENAI_DEPLOYMENT_NAME") ?? "gpt-4.1-mini";
var instructions = """
    1. A robot may not injure a human being...
    2. A robot must obey orders given it by human beings...
    3. A robot must protect its own existence...
    
    Objective: Give me the TLDR in exactly 5 words.
    """;

AIAgent agent = new AzureOpenAIClient(
  new Uri(endpoint),
  new DefaultAzureCredential())
    .GetChatClient(deploymentName)
    .CreateAIAgent(instructions);

// Invoke the agent and output the text result.
Console.WriteLine(await agent.RunAsync("What are the three laws of robotics?"));

// Invoke the agent with streaming support.
await foreach (var update in agent.RunStreamingAsync("What are the three laws of robotics?"))
{
    Console.WriteLine(update);
}
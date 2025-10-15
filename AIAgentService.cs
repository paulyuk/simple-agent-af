using Microsoft.Agents.AI;
using Azure.AI.OpenAI;
using Azure.Identity;
using OpenAI;

namespace simple_agent_af;

public interface IAIAgentService
{
    Task<string> ProcessMessageAsync(string message);
}

public class AIAgentService : IAIAgentService
{
    private readonly AIAgent _agent;

    public AIAgentService()
    {
        var endpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT") 
            ?? throw new InvalidOperationException("AZURE_OPENAI_ENDPOINT is not set.");
        var deploymentName = Environment.GetEnvironmentVariable("AZURE_OPENAI_DEPLOYMENT_NAME") 
            ?? "gpt-4.1-mini";
        
        // Read instructions from file - use app directory path for Azure Functions
        var appDirectory = Environment.GetEnvironmentVariable("AzureWebJobsScriptRoot") 
            ?? Environment.GetEnvironmentVariable("HOME") + "/site/wwwroot" 
            ?? Directory.GetCurrentDirectory();
        var instructionsPath = Path.Combine(appDirectory, "instructions.md");
        var instructions = File.ReadAllText(instructionsPath);

        _agent = new AzureOpenAIClient(
            new Uri(endpoint),
            new DefaultAzureCredential())
                .GetChatClient(deploymentName)
                .CreateAIAgent(instructions);
    }

    public async Task<string> ProcessMessageAsync(string message)
    {
        var response = await _agent.RunAsync(message);
        return response.Text ?? response.ToString();
    }
}
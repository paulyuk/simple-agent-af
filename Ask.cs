using System.Net;
using GitHub.Copilot.SDK;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace simple_agent_af;

public class Ask
{
    private static readonly CopilotClient Client = new();

    private static readonly string Instructions = """
        1. A robot may not injure a human being...
        2. A robot must obey orders given it by human beings...
        3. A robot must protect its own existence...
        
        Objective: Give me the TLDR in exactly 5 words.
        """;

    private static SessionConfig BuildSessionConfig()
    {
        var config = new SessionConfig
        {
            SystemMessage = new SystemMessageConfig { Content = Instructions }
        };
        var baseUrl = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT");
        var apiKey = Environment.GetEnvironmentVariable("AZURE_OPENAI_API_KEY");
        var model = Environment.GetEnvironmentVariable("AZURE_OPENAI_MODEL") ?? "gpt-5-mini";
        if (!string.IsNullOrEmpty(baseUrl) && !string.IsNullOrEmpty(apiKey))
        {
            config.Model = model;
            config.Provider = new ProviderConfig
            {
                Type = "azure",
                BaseUrl = baseUrl,
                ApiKey = apiKey
            };
        }
        return config;
    }

    [Function("ask")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "ask")] HttpRequestData req)
    {
        var prompt = await new StreamReader(req.Body).ReadToEndAsync();
        if (string.IsNullOrWhiteSpace(prompt))
            prompt = "What are the laws?";

        await using var session = await Client.CreateSessionAsync(BuildSessionConfig());
        var reply = await session.SendAndWaitAsync(new MessageOptions { Prompt = prompt });

        var response = req.CreateResponse(HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "text/plain");
        await response.WriteStringAsync(reply?.Data.Content ?? "No response");
        return response;
    }
}

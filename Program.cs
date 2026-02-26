using GitHub.Copilot.SDK;

var client = new CopilotClient();

var instructions = """
    1. A robot may not injure a human being...
    2. A robot must obey orders given it by human beings...
    3. A robot must protect its own existence...
    
    Objective: Give me the TLDR in exactly 5 words.
    """;

SessionConfig SessionConfig()
{
    var config = new SessionConfig
    {
        SystemMessage = new SystemMessageConfig { Content = instructions }
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

Console.WriteLine("=== Simple Copilot SDK Agent ===\n");

await using var session = await client.CreateSessionAsync(SessionConfig());

while (true)
{
    Console.Write("Enter your message: ");
    var userMessage = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(userMessage) ||
        userMessage.Equals("exit", StringComparison.OrdinalIgnoreCase) ||
        userMessage.Equals("quit", StringComparison.OrdinalIgnoreCase))
    {
        Console.WriteLine("Goodbye!");
        break;
    }

    try
    {
        var reply = await session.SendAndWaitAsync(new MessageOptions { Prompt = userMessage });
        Console.WriteLine($"\nAgent: {reply?.Data.Content}\n");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}\n");
    }
}
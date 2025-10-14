using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using FromBodyAttribute = Microsoft.Azure.Functions.Worker.Http.FromBodyAttribute;
using System.Text.Json.Serialization;

namespace simple_agent_af;

public class AgentFunction
{
    private readonly ILogger _logger;
    private readonly IAIAgentService _agentService;

    public AgentFunction(ILoggerFactory loggerFactory, IAIAgentService agentService)
    {
        _logger = loggerFactory.CreateLogger<AgentFunction>();
        _agentService = agentService;
    }

    [Function("chat")]
    public async Task<IActionResult> ProcessMessage(
        [HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req,
        [FromBody] MessageRequest request)
    {
        _logger.LogInformation("Processing agent message request.");

        try
        {
            if (string.IsNullOrWhiteSpace(request?.Message))
            {
                _logger.LogWarning("Message request received with empty message.");
                return new BadRequestObjectResult("Message cannot be empty.");
            }

            // Process the message through the AI agent
            var response = await _agentService.ProcessMessageAsync(request.Message);

            _logger.LogInformation("Successfully processed agent message.");
            return new OkObjectResult(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing agent message");
            return new ObjectResult("Internal server error occurred.")
            {
                StatusCode = 500
            };
        }
    }

    [Function("health")]
    public IActionResult Health([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req)
    {
        _logger.LogInformation("Health check requested.");
        return new OkObjectResult("Healthy");
    }
}

public class MessageRequest
{
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;
}
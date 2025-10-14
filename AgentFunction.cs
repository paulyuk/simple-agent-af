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
    public async Task<IActionResult> Health([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req)
    {
        _logger.LogInformation("Health check requested.");

        var healthCheck = new
        {
            Status = "Healthy",
            Timestamp = DateTime.UtcNow,
            Checks = new Dictionary<string, object>()
        };

        try
        {
            // Check if AI Agent service is available
            if (_agentService == null)
            {
                healthCheck.Checks["AIAgentService"] = new { Status = "Unhealthy", Error = "Service not available" };
                return new ObjectResult(new { 
                    Status = "Unhealthy", 
                    Timestamp = DateTime.UtcNow, 
                    Checks = healthCheck.Checks 
                }) { StatusCode = 503 };
            }
            
            healthCheck.Checks["AIAgentService"] = new { Status = "Healthy" };

            // Check Azure OpenAI connection by testing a simple request
            var testResponse = await _agentService.ProcessMessageAsync("Health check test");
            if (!string.IsNullOrEmpty(testResponse))
            {
                healthCheck.Checks["AzureOpenAI"] = new { Status = "Healthy", ResponseLength = testResponse.Length };
            }
            else
            {
                healthCheck.Checks["AzureOpenAI"] = new { Status = "Unhealthy", Error = "Empty response" };
                return new ObjectResult(new { 
                    Status = "Unhealthy", 
                    Timestamp = DateTime.UtcNow, 
                    Checks = healthCheck.Checks 
                }) { StatusCode = 503 };
            }

            // Check environment variables
            var endpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT");
            var deployment = Environment.GetEnvironmentVariable("AZURE_OPENAI_DEPLOYMENT_NAME");
            
            healthCheck.Checks["Configuration"] = new { 
                Status = "Healthy", 
                HasEndpoint = !string.IsNullOrEmpty(endpoint),
                HasDeployment = !string.IsNullOrEmpty(deployment),
                Deployment = deployment ?? "gpt-4o-mini"
            };

            _logger.LogInformation("Health check completed successfully.");
            return new OkObjectResult(healthCheck);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Health check failed");
            
            healthCheck.Checks["Exception"] = new { 
                Status = "Unhealthy", 
                Error = ex.Message,
                Type = ex.GetType().Name
            };

            return new ObjectResult(new { 
                Status = "Unhealthy", 
                Timestamp = DateTime.UtcNow, 
                Checks = healthCheck.Checks 
            }) { StatusCode = 503 };
        }
    }
}

public class MessageRequest
{
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;
}
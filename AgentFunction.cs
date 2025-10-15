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
        
        var healthStatus = new
        {
            Status = "Healthy",
            Timestamp = DateTime.UtcNow,
            Environment = new
            {
                AzureOpenAIEndpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT") ?? "Not Set",
                AzureOpenAIDeploymentName = Environment.GetEnvironmentVariable("AZURE_OPENAI_DEPLOYMENT_NAME") ?? "Not Set",
                ProjectEndpoint = Environment.GetEnvironmentVariable("PROJECT_ENDPOINT") ?? "Not Set",
                ModelDeploymentName = Environment.GetEnvironmentVariable("MODEL_DEPLOYMENT_NAME") ?? "Not Set"
            },
            Checks = new Dictionary<string, object>()
        };

        var checks = (Dictionary<string, object>)healthStatus.Checks;

        // Check environment variables
        var endpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT");
        var deploymentName = Environment.GetEnvironmentVariable("AZURE_OPENAI_DEPLOYMENT_NAME");
        
        checks["EnvironmentVariables"] = new
        {
            Status = !string.IsNullOrEmpty(endpoint) && !string.IsNullOrEmpty(deploymentName) ? "Healthy" : "Unhealthy",
            EndpointConfigured = !string.IsNullOrEmpty(endpoint),
            DeploymentNameConfigured = !string.IsNullOrEmpty(deploymentName)
        };

        // Test Azure OpenAI connectivity
        try
        {
            if (!string.IsNullOrEmpty(endpoint) && !string.IsNullOrEmpty(deploymentName))
            {
                // Test the AI agent service initialization
                var testResponse = await _agentService.ProcessMessageAsync("Health check test - respond with 'OK'");
                
                checks["AzureOpenAI"] = new
                {
                    Status = "Healthy",
                    Message = "Successfully connected to Azure OpenAI",
                    TestResponse = testResponse?.Substring(0, Math.Min(testResponse.Length, 100)) ?? "No response"
                };
            }
            else
            {
                checks["AzureOpenAI"] = new
                {
                    Status = "Unhealthy",
                    Message = "Missing required environment variables for Azure OpenAI"
                };
            }
        }
        catch (Exception ex)
        {
            checks["AzureOpenAI"] = new
            {
                Status = "Unhealthy",
                Message = "Failed to connect to Azure OpenAI",
                Error = ex.Message
            };
            _logger.LogWarning(ex, "Azure OpenAI health check failed");
        }

        // Determine overall status
        var overallHealthy = checks.Values.All(check => 
        {
            var checkObj = check as dynamic;
            return checkObj?.Status?.ToString() == "Healthy";
        });

        var result = new
        {
            healthStatus.Status,
            OverallHealth = overallHealthy ? "Healthy" : "Degraded",
            healthStatus.Timestamp,
            healthStatus.Environment,
            healthStatus.Checks
        };

        return new OkObjectResult(result);
    }
}

public class MessageRequest
{
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;
}
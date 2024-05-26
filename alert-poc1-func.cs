using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Company.Function
{
    public class alert_poc1_func
    {
        private readonly ILogger<alert_poc1_func> _logger;

        public alert_poc1_func(ILogger<alert_poc1_func> logger)
        {
            _logger = logger;
        }

        [Function("alert_poc1_func")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            return new OkObjectResult("Welcome to Azure Functions!");
        }
    }
}

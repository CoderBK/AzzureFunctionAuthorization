using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using AzureFunctionAuthorization.Domain.ViewModel;
using AzureFunctionAuthorization.IService;
using AzureFunctionAuthorization.Attributes;

namespace AzureFunctionAuthorization {
    public class HttpTriggerFunction {
        public IJWTHandlerService _jWTHandlerService { get; }

        public HttpTriggerFunction(IJWTHandlerService jWTHandlerService) {
            _jWTHandlerService = jWTHandlerService;
        }

        /// <summary>
        /// Use Attribute which will help to check token
        /// </summary>
        /// <param name="req"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        [FunctionName(nameof(HttpTriggerFunction.TestAuthorization))]
        public IActionResult TestAuthorization(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log,
            [JWTAuthorization()] JWTClaim jwtClaim) {
            log.LogInformation("C# HTTP trigger function processed a request.");

            // Check token is valid or not
            if (jwtClaim == null)
            {
                return new UnauthorizedObjectResult("Invalid token");
            }

            return new OkObjectResult("Valid Token");
        }

        /// <summary>
        /// Craete token and return
        /// </summary>
        /// <param name="req"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        [FunctionName(nameof(HttpTriggerFunction.CreateToken))]
        public IActionResult CreateToken(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log) {
            log.LogInformation("C# HTTP trigger function processed a request.");

            // Just create sample JWTClaim object to create token
            var jwtClaim = new JWTClaim {
                AccountId = 1
            };

            // Generate token
            var token = _jWTHandlerService.GenerateAccessToken(jwtClaim);

            return new OkObjectResult(token);
        }
    }
}

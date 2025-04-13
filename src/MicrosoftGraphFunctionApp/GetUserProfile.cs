using System.Text.Json;
using System.Text.Json.Serialization;
using Azure.Core;
using Azure.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;

namespace MicrosoftGraphFunctionApp
{
    public class GetUserProfile(ILoggerFactory loggerFactory)
    {
        private readonly ILogger _logger = loggerFactory.CreateLogger<GetUserProfile>();

        [Function("GetUserProfile")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
        {
            _logger.LogInformation($"C# HTTP trigger function executed at: {DateTime.Now}");
            var requestHeaderClientPrincipalId = req.Headers["X-MS-CLIENT-PRINCIPAL-ID"].ToString();

            string? userProfile;
            try
            {
                userProfile = await GetUserProfileAsync(requestHeaderClientPrincipalId);
                _logger.LogInformation($"User Profile: {userProfile}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching user profile: {ex.Message}");
                return new ObjectResult(new { error = "An error occurred while fetching the user profile." })
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }

            return new OkObjectResult(userProfile);
        }

        private static async Task<string> GetUserProfileAsync(string? userId)
        {
            // Get the environment variable to determine if we are in Development (local) or Production (Azure)
            // and set the appropriate credential for Graph API access.
            var environment = Environment.GetEnvironmentVariable("AZURE_FUNCTIONS_ENVIRONMENT");

            TokenCredential? credential = new AzureCliCredential();
            var graphClient = new GraphServiceClient(credential);
            Microsoft.Graph.Models.User user;

            if (environment == "Development"){
                var me = await graphClient.Me.GetAsync() ?? throw new InvalidOperationException("Failed to retrieve the user profile.");
                user = me;
            }
            else
            {
                credential = new ManagedIdentityCredential();
                graphClient = new GraphServiceClient(credential);
                user = await graphClient.Users[userId].GetAsync() ?? throw new InvalidOperationException("Failed to retrieve the user profile.");
            }

            var userProfileJson = JsonSerializer.Serialize(user, new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                WriteIndented = true
            });

            return userProfileJson;
        }
    }
}
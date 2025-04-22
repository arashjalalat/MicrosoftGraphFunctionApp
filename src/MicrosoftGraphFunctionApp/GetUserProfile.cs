using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using MicrosoftGraphFunctionApp.Interfaces;
using MicrosoftGraphFunctionApp.Services;

namespace MicrosoftGraphFunctionApp
{
    public class GetUserProfile
    {
        private readonly ILogger _logger;
        private readonly IUserProfileService _userProfileService;

        public GetUserProfile(ILoggerFactory loggerFactory, IUserProfileService userProfileService)
        {
            _logger = loggerFactory.CreateLogger<GetUserProfile>();
            _userProfileService = userProfileService;
        }

        [Function("GetUserProfile")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
        {
            _logger.LogInformation($"C# HTTP trigger function executed at: {DateTime.Now}");
            var requestHeaderClientPrincipalId = req.Headers["X-MS-CLIENT-PRINCIPAL-ID"].ToString();

            string? userProfile;
            try
            {
                userProfile = await _userProfileService.GetUserProfileAsync(requestHeaderClientPrincipalId);
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
    }
}
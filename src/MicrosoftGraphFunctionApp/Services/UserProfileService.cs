using System.Text.Json;
using System.Text.Json.Serialization;
using Azure.Core;
using Azure.Identity;
using Microsoft.Graph;
using MicrosoftGraphFunctionApp.Interfaces;

namespace MicrosoftGraphFunctionApp.Services
{
    public class UserProfileService : IUserProfileService
    {
        public async Task<string> GetUserProfileAsync(string? userId)
        {
            var environment = Environment.GetEnvironmentVariable("AZURE_FUNCTIONS_ENVIRONMENT");

            TokenCredential? credential = new AzureCliCredential();
            var graphClient = new GraphServiceClient(credential);
            Microsoft.Graph.Models.User user;

            if (environment == "Development")
            {
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
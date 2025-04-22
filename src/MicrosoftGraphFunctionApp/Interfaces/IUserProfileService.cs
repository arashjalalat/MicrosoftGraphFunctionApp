using System.Threading.Tasks;

namespace MicrosoftGraphFunctionApp.Interfaces
{
    public interface IUserProfileService
    {
        Task<string> GetUserProfileAsync(string? userId);
    }
}
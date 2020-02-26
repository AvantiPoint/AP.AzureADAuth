using System.Threading.Tasks;
using Microsoft.Identity.Client;

namespace AP.AzureADAuth.Services
{
    public static class IAuthenticationHandlerExtensions
    {
        public static async Task<string> GetLatestAccessToken(this IAuthenticationHandler handler)
        {
            var result = await GetLatestAuthenticationResult(handler);
            return result?.AccessToken;
        }

        public static async Task<AuthenticationResult> GetLatestAuthenticationResult(this IAuthenticationHandler handler)
        {
            AuthenticationResult result = null;
            if (handler is IAuthenticationService authService)
            {
                 result = await authService.LoginSilentAsync();
            }

            return result;
        }
    }
}

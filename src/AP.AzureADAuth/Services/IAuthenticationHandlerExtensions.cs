using System.Threading.Tasks;
using System.Reactive.Linq;

namespace AP.AzureADAuth.Services
{
    public static class IAuthenticationHandlerExtensions
    {
        public static async Task<string> GetLatestAccessToken(this IAuthenticationHandler handler)
        {
            if(handler is IAuthenticationService authService)
            {
                await authService.LoginSilentAsync();
            }

            return await handler.AccessToken;
        }
    }
}

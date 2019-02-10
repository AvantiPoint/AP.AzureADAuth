using System.Linq;
using System.Threading.Tasks;
using Microsoft.Identity.Client;

namespace AP.AzureADAuth.Services
{
    internal class AuthenticationService : IAuthenticationService
    {
        private IAuthOptions AuthOptions { get; }
        private UIParent UIParent { get; }
        private IPublicClientApplication PublicClientApplication { get; }

        public AuthenticationService(IAuthOptions authOptions, IPublicClientApplication publicClientApplication, UIParent uiParent)
        {
            AuthOptions = authOptions;
            PublicClientApplication = publicClientApplication;
            UIParent = uiParent;
        }

        public async Task<AuthenticationResult> LoginAsync()
        {
            var result = await LoginSilentAsync();
            if (result is null)
            {
                var accounts = await PublicClientApplication.GetAccountsAsync();
                result = await PublicClientApplication.AcquireTokenAsync(AuthOptions.Scopes, accounts?.FirstOrDefault(), UIParent);
            }

            return result;
        }

        public async Task<AuthenticationResult> LoginSilentAsync()
        {
            try
            {
                var accounts = await PublicClientApplication.GetAccountsAsync();
                return await PublicClientApplication.AcquireTokenSilentAsync(AuthOptions.Scopes, accounts?.FirstOrDefault());
            }
            catch
            {
                return null;
            }
        }

        public async Task LogoutAsync()
        {
            var accounts = await PublicClientApplication.GetAccountsAsync();
            if (!accounts?.Any() ?? false) return;

            foreach (var acct in accounts)
                await PublicClientApplication.RemoveAsync(acct);
        }
    }
}

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Identity.Client;
#if __ANDROID__
using Plugin.CurrentActivity;
#endif

namespace AP.AzureADAuth.Services
{
    internal class AuthenticationService : IAuthenticationService
    {
        IAuthConfiguration _configuration { get; }
        IPublicClientApplication _client { get; }

#if __ANDROID__
        ICurrentActivity CurrentActivity { get; }

        public AuthenticationService(Func<IPublicClientApplication> pcaFactory, IAuthConfiguration configuration,
            ICurrentActivity currentActivity)
        {
            _client = pcaFactory();
            _configuration = configuration;
            CurrentActivity = currentActivity;
        }
#else
        public AuthenticationService(Func<IPublicClientApplication> pcaFactory, IAuthConfiguration configuration)
        {
            _client = pcaFactory();
            _configuration = configuration;
        }
#endif

        public async Task<AuthenticationResult> LoginAsync()
        {
            var result = await LoginSilentAsync();
            if (result is null)
            {
                var accounts = await _client.GetAccountsAsync();
                var builder = _client.AcquireTokenInteractive(_configuration.Scopes);

#if __ANDROID__
                builder = builder.WithParentActivityOrWindow(CurrentActivity.Activity);
#endif
                result = await builder.WithUseEmbeddedWebView(true)
                                      .ExecuteAsync();
            }

            return result;
        }

        public async Task<AuthenticationResult> LoginSilentAsync()
        {
            try
            {
                var accounts = await _client.GetAccountsAsync();
                return await _client.AcquireTokenSilent(_configuration.Scopes, accounts.FirstOrDefault())
                                    .WithForceRefresh(true)
                                    .ExecuteAsync();
            }
            catch
            {
                return null;
            }
        }

        public async Task LogoutAsync()
        {
            var accounts = await _client.GetAccountsAsync();
            foreach (var account in accounts)
                await _client.RemoveAsync(account);
        }
    }
}

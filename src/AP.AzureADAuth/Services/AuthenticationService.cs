using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Identity.Client;

namespace AP.AzureADAuth.Services
{
    internal class AuthenticationService : IAuthenticationService
    {
        IAuthConfiguration _configuration { get; }
        IPublicClientApplication _client { get; }

        public AuthenticationService(IPublicClientApplication pca, IAuthConfiguration configuration)
        {
            _client = pca;
            _configuration = configuration;
        }

        public async Task<AuthenticationResult> LoginAsync()
        {
            var result = await LoginSilentAsync();
            if (result is null)
            {
                var accounts = await _client.GetAccountsAsync();
                var builder = _client.AcquireTokenInteractive(_configuration.Scopes);
                if(accounts.Any())
                {
                    builder = builder.WithAccount(accounts.First());
                }
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

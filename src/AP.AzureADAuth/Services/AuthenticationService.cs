using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using AP.AzureADAuth.Events;
using Microsoft.Identity.Client;
using Prism.Events;
using Prism.Logging;
using ReactiveUI;

namespace AP.AzureADAuth.Services
{
    internal class AuthenticationService : IAuthenticationService, IAuthenticationHandler
    {
        private IAuthConfiguration _configuration { get; }
        private IPublicClientApplication _client { get; }
        private IEventAggregator _eventAggregator { get; }
        private ILogger _logger { get; }

        private Subject<string> _accessToken { get; }
        private Subject<AuthenticationResult> _authResult { get; }
        private ReactiveCommand<Unit, Unit> LogoutCommand { get; }
        private ReactiveCommand<Unit, Unit> RefreshTokenCommand { get; }

        public AuthenticationService(IPublicClientApplication pca, IAuthConfiguration configuration, IEventAggregator eventAggregator, ILogger logger)
        {
            _accessToken = new Subject<string>();
            _authResult = new Subject<AuthenticationResult>();
            _client = pca;
            _configuration = configuration;
            _eventAggregator = eventAggregator;
            _logger = logger;

            LogoutCommand = ReactiveCommand.CreateFromTask(OnLogoutCommandExecuted);
            RefreshTokenCommand = ReactiveCommand.CreateFromTask(OnRefreshTokenCommandExecuted);

            _eventAggregator.GetEvent<LogoutRequestedEvent>().Subscribe(OnLogoutRequestedEventPublished);
            _eventAggregator.GetEvent<RefreshTokenRequestedEvent>().Subscribe(OnRefreshTokenRequestedEventPublished);
        }

        public IObservable<string> AccessToken => _accessToken;
        public IObservable<AuthenticationResult> AuthenticationResult => _authResult;

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

            _authResult.OnNext(result);
            _accessToken.OnNext(result?.AccessToken);
            return result;
        }

        public async Task<AuthenticationResult> LoginSilentAsync()
        {
            try
            {
                var accounts = await _client.GetAccountsAsync();
                var result = await _client.AcquireTokenSilent(_configuration.Scopes, accounts.FirstOrDefault())
                                    //.WithForceRefresh(true)
                                    .ExecuteAsync();

                _authResult.OnNext(result);
                _accessToken.OnNext(result?.AccessToken);
                return result;
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

        private void OnLogoutRequestedEventPublished()
        {
            LogoutCommand.Execute();
        }

        private void OnRefreshTokenRequestedEventPublished()
        {
            RefreshTokenCommand.Execute();
        }

        private async Task OnRefreshTokenCommandExecuted()
        {
            try
            {
                _logger.TrackEvent("Token Refresh Requested");
                var result = await LoginSilentAsync();
                if (result is null)
                {
                    _logger.TrackEvent("Unable to Refresh Token");
                }

                _eventAggregator.GetEvent<TokenRefreshedEvent>().Publish(result);
            }
            catch (Exception ex)
            {
                _logger.Report(ex, new Dictionary<string, string> { { "event", "Logout Requested Event" } });
            }
        }

        private async Task OnLogoutCommandExecuted()
        {
            try
            {
                _logger.TrackEvent("Logout Requested");
                await LogoutAsync();
                _logger.TrackEvent("User Logged Out");
                _eventAggregator.GetEvent<UserLoggedOutEvent>().Publish();
            }
            catch (Exception ex)
            {
                _logger.Report(ex, new Dictionary<string, string> { { "event", "Logout Requested Event" } });
            }
        }
    }
}

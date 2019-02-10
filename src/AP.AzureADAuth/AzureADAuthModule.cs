using System;
using System.Collections.Generic;
using AP.AzureADAuth.Events;
using AP.AzureADAuth.Services;
using AP.AzureADAuth.ViewModels;
using AP.AzureADAuth.Views;
using Microsoft.Identity.Client;
using Prism.Events;
using Prism.Ioc;
using Prism.Logging;
using Prism.Modularity;

namespace AP.AzureADAuth
{
    public class AzureADAuthModule : IModule
    {
        private IAuthenticationService _authenticationService;
        private IEventAggregator _eventAggregator;
        private ILogger _logger;

        public void OnInitialized(IContainerProvider containerProvider)
        {
            _authenticationService = containerProvider.Resolve<IAuthenticationService>();
            _eventAggregator = containerProvider.Resolve<IEventAggregator>();
            _logger = containerProvider.Resolve<ILogger>();

            _eventAggregator.GetEvent<LogoutRequestedEvent>().Subscribe(OnLogoutRequestedEventPublished);
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            if (!containerRegistry.IsRegistered<ILogger>())
            {
                containerRegistry.RegisterSingleton<ILogger, ConsoleLoggingService>();
            }

            if (!containerRegistry.IsRegistered<UIParent>())
            {
                containerRegistry.RegisterInstance(new UIParent());
            }

            var c = ((IContainerExtension)containerRegistry);
            IAuthOptions options = containerRegistry.IsRegistered<IAuthOptions>() ?
                c.Resolve<IAuthOptions>() :
                c.Resolve<DefaultAuthOptions>();
            var authority = $"https://login.microsoftonline.com/tfp/{options.Tenant}/{options.Policy}";

            IPublicClientApplication pca = options.IsB2C ?
                new PublicClientApplication(options.ClientId, authority) :
                new PublicClientApplication(options.ClientId);
            pca.RedirectUri = $"msal{options.ClientId}://auth";

            containerRegistry.RegisterInstance<IPublicClientApplication>(pca);
            containerRegistry.Register<IAuthenticationService, AuthenticationService>();
            containerRegistry.RegisterForNavigation<LoginPage, LoginPageViewModel>("login");
        }

        private async void OnLogoutRequestedEventPublished()
        {
            try
            {
                _logger.TrackEvent("Logout Requested");
                await _authenticationService.LogoutAsync();
                _logger.TrackEvent("User Logged Out");
                _eventAggregator.GetEvent<UserLoggedOutEvent>().Publish();
            }
            catch (Exception ex)
            {
                _logger.Report(ex, new Dictionary<string, string> { { "event", nameof(OnLogoutRequestedEventPublished) } });
            }
            
        }
    }
}



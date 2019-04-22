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
using Prism.Services;

namespace AP.AzureADAuth
{
    public class AzureADAuthModule : IModule
    {
        private IAuthenticationService _authenticationService;
        private IEventAggregator _eventAggregator;
        private ILogger _logger;
        private IContainerProvider _containerProvider;

        public void OnInitialized(IContainerProvider containerProvider)
        {
            _authenticationService = containerProvider.Resolve<IAuthenticationService>();
            _containerProvider = containerProvider;
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

            if (containerRegistry.IsRegistered<IAuthOptions>())
            {
                containerRegistry.Register<IAuthConfiguration, UserDefinedConfiguration>();
                if(((IContainerProvider)containerRegistry).Resolve<IAuthOptions>().IsB2C)
                {
                    containerRegistry.RegisterInstance<Func<IPublicClientApplication>>(CreateB2CClient);
                }
                else
                {
                    containerRegistry.RegisterInstance<Func<IPublicClientApplication>>(CreateAADClient);
                }
            }
            else if(containerRegistry.IsRegistered<IB2COptions>())
            {
                containerRegistry.Register<IAuthConfiguration, DefaultB2CConfiguration>();
                containerRegistry.RegisterInstance<Func<IPublicClientApplication>>(CreateB2CClient);
            }
            else if(containerRegistry.IsRegistered<IAADOptions>())
            {
                containerRegistry.Register<IAuthConfiguration, DefaultAADConfiguration>();
                containerRegistry.RegisterInstance<Func<IPublicClientApplication>>(CreateAADClient);
            }
            else
            {
                ((IContainerProvider)containerRegistry).Resolve<IPageDialogService>().DisplayAlertAsync("Error", "You must register a a configuration for Azure Active Directory or Azure Active Directory B2C", "Ok");
                throw new ModuleInitializeException("No configuration for Azure Active Directory or Azure Active Directory B2C was found");
            }

            containerRegistry.Register<IAuthenticationService, AuthenticationService>();
            containerRegistry.RegisterForNavigation<LoginPage, LoginPageViewModel>("login");
        }

        private IPublicClientApplication CreateAADClient()
        {
            if (_containerProvider is null) return null;

            var configuration = _containerProvider.Resolve<IAuthConfiguration>();

            return CreateBaseBuilder(configuration)
                .Build();
        }

        private IPublicClientApplication CreateB2CClient()
        {
            if (_containerProvider is null) return null;

            var configuration = _containerProvider.Resolve<IAuthConfiguration>();

            return CreateBaseBuilder(configuration)
                    .WithB2CAuthority(configuration.Authority)
                    .Build();
        }

        private PublicClientApplicationBuilder CreateBaseBuilder(IAuthConfiguration configuration)
        {
            return PublicClientApplicationBuilder.Create(configuration.ClientId)
                                                 .WithRedirectUri(configuration.RedirectUri)
#if __IOS__
                                                 .WithIosKeychainSecurityGroup(Xamarin.Essentials.AppInfo.PackageName)
#endif
                                                 .WithLogging(AADLog, configuration.LogLevel);
        }

        private void AADLog(LogLevel level, string message, bool containsPii)
        {
            _logger.Log(message, new Dictionary<string, string> { { "level", $"{level}" }, { "containsPii", $"{containsPii}" } });
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



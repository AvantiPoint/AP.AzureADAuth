using System.Collections.Generic;
using AP.AzureADAuth.Services;
using AP.AzureADAuth.ViewModels;
using AP.AzureADAuth.Views;
using Microsoft.Identity.Client;
using Prism.Ioc;
using Prism.Logging;
using Prism.Modularity;
using Prism.Services;

namespace AP.AzureADAuth
{
    public class AzureADAuthModule : AzureADAuthModule<LoginPage>
    {
    }

    public class AzureADAuthModule<TView> : IModule
        where TView : Xamarin.Forms.ContentPage
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            if (!containerRegistry.IsRegistered<ILogger>())
            {
                if (System.Diagnostics.Debugger.IsAttached)
                    containerRegistry.RegisterSingleton<ILogger, ConsoleLoggingService>();
                else
                    containerRegistry.RegisterSingleton<ILogger, NullLoggingService>();
            }

            if (containerRegistry.IsRegistered<IAuthOptions>())
            {
                containerRegistry.Register<IAuthConfiguration, UserDefinedConfiguration>();
                if(((IContainerProvider)containerRegistry).Resolve<IAuthOptions>().IsB2C)
                {
                    containerRegistry.Register<IPublicClientApplication>(CreateB2CClient);
                }
                else
                {
                    containerRegistry.Register<IPublicClientApplication>(CreateAADClient);
                }
            }
            else if(containerRegistry.IsRegistered<IB2COptions>())
            {
                containerRegistry.Register<IAuthConfiguration, DefaultB2CConfiguration>();
                containerRegistry.Register<IPublicClientApplication>(CreateB2CClient);
            }
            else if(containerRegistry.IsRegistered<IAADOptions>())
            {
                containerRegistry.Register<IAuthConfiguration, DefaultAADConfiguration>();
                containerRegistry.Register<IPublicClientApplication>(CreateAADClient);
            }
            else
            {
                ((IContainerProvider)containerRegistry).Resolve<IPageDialogService>().DisplayAlertAsync("Error", "You must register a a configuration for Azure Active Directory or Azure Active Directory B2C", "Ok");
                throw new ModuleInitializeException("No configuration for Azure Active Directory or Azure Active Directory B2C was found");
            }

            containerRegistry.RegisterManySingleton<AuthenticationService>();
            containerRegistry.RegisterForNavigation<TView, LoginPageViewModel>("login");
        }

        private static IPublicClientApplication CreateAADClient(IContainerProvider containerProvider)
        {
            if (containerProvider is null) return null;

            var configuration = containerProvider.Resolve<IAuthConfiguration>();

            return CreateBaseBuilder(configuration, containerProvider)
                    .Build();
        }

        private static IPublicClientApplication CreateB2CClient(IContainerProvider containerProvider)
        {
            if (containerProvider is null) return null;

            var configuration = containerProvider.Resolve<IAuthConfiguration>();

            return CreateBaseBuilder(configuration, containerProvider)
                    .WithB2CAuthority(configuration.Authority)
                    .Build();
        }

        private static PublicClientApplicationBuilder CreateBaseBuilder(IAuthConfiguration configuration, IContainerProvider containerProvider)
        {
            if(_logger is null)
            {
                _logger = containerProvider.Resolve<ILogger>();
            }

            var redirectUri = string.IsNullOrEmpty(configuration.RedirectUri) ? $"msal{configuration.ClientId}://auth" : configuration.RedirectUri;
            var builder = PublicClientApplicationBuilder.Create(configuration.ClientId)
                                                 .WithRedirectUri(redirectUri);
#if __IOS__
            builder = builder.WithIosKeychainSecurityGroup(Xamarin.Essentials.AppInfo.PackageName);
#elif MONOANDROID
            builder = builder.WithParentActivityOrWindow(() => Xamarin.Essentials.Platform.CurrentActivity);
#endif
            return builder.WithLogging(AADLog, configuration.LogLevel);
        }

        private static ILogger _logger;
        private static void AADLog(LogLevel level, string message, bool containsPii)
        {
            _logger.Log(message, new Dictionary<string, string> { { "level", $"{level}" }, { "containsPii", $"{containsPii}" } });
        }
    }
}



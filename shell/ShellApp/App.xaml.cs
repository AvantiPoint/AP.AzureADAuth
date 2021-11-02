using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using AP.AzureADAuth;
using AP.AzureADAuth.Events;
using AP.AzureADAuth.Services;
using Microsoft.Identity.Client;
using Prism;
using Prism.DryIoc;
using Prism.Events;
using Prism.Ioc;
using Prism.Logging;
using Prism.Logging.Syslog;
using Prism.Modularity;
using Prism.Navigation;
using ShellApp.Helpers;
using ShellApp.Views;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;
using SyslogOptions = ShellApp.Helpers.SyslogOptions;

[assembly: InternalsVisibleTo("ShellApp.Android")]
[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace ShellApp
{
    public partial class App : PrismApplication
    {
        public App() : base() { }
        public App(IPlatformInitializer initializer) : base(initializer) { }

        private ILogger _logger;
        public ILogger Logger => _logger ??= Container.Resolve<ILogger>();

        protected override async void OnInitialized()
        {
            InitializeComponent();

#if DEBUG
            Log.Listeners.Add(new DelegateLogListener(InternalLogger));
#else
            AppCenter.Start(Secrets.AppCenterSecret, typeof(Analytics), typeof(Crashes));
#endif

            var ea = Container.Resolve<IEventAggregator>();
            ea.GetEvent<UserAuthenticatedEvent>().Subscribe(OnUserAuthenticatedEventPublished);
            ea.GetEvent<UserLoggedOutEvent>().Subscribe(OnUserLoggedOutEventPublished);

            var result = await NavigationService.NavigateAsync("login");

            if (!result.Success)
            {
                MainPage = new NavigationPage(new ContentPage
                {
                    Title = result.Exception.GetType().Name,
                    Content = new ScrollView
                    {

                        Margin = new Thickness(20),
                        Content = new Label { Text = result.Exception.ToString() }
                    }
                });
            }
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IB2COptions, B2COptions>();
#if DEBUG
            containerRegistry.RegisterSingleton<ISyslogOptions, SyslogOptions>();
            containerRegistry.RegisterManySingleton<SyslogLogger>();
#else
            containerRegistry.RegisterManySingleton<AppCenterLogger>();
#endif

            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<TabbedPage>();
            containerRegistry.RegisterForNavigation<MainPage>();
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            moduleCatalog.AddModule<AzureADAuthModule>();
        }

        protected override void InitializeModules()
        {
            var manager = Container.Resolve<IModuleManager>();
            manager.LoadModuleCompleted += (sender, args) => LoadModuleCompleted(args.ModuleInfo, args.Error, args.IsErrorHandled);

        }

        private void LoadModuleCompleted(IModuleInfo moduleInfo, Exception error, bool isHandled)
        {
            if(error != null)
            {
                Debugger.Break();
                Logger.Debug($"{moduleInfo.ModuleName} has encountered an error while loading...");
                Logger.Report(error);
            }
        }

        private void InternalLogger(string category, string message)
        {
            Trace.WriteLine($"{category}: {message}");
            Container.Resolve<ILogger>().Log($"{category}: {message}");
        }

        private void OnUserLoggedOutEventPublished()
        {
            NavigationService.NavigateAsync("/login");
        }

        private void OnUserAuthenticatedEventPublished(AuthenticationResult authResult)
        {
            NavigationService.NavigateAsync("/NavigationPage/MainPage", ("authResult", authResult));
        }
    }
}



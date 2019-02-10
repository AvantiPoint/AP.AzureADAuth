using System;
using System.Diagnostics;
using System.Linq;
using AP.AzureADAuth;
using AP.AzureADAuth.Events;
using AP.AzureADAuth.Services;
using DryIoc;
using Microsoft.Identity.Client;
using Newtonsoft.Json;
using Prism;
using Prism.DryIoc;
using Prism.Events;
using Prism.Ioc;
using Prism.Logging;
using Prism.Logging.Syslog;
using Prism.Modularity;
using ShellApp.Helpers;
using ShellApp.Views;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace ShellApp
{
    public partial class App
    {
        public App() : base() { }
        public App(IPlatformInitializer initializer) : base(initializer) { }

        protected override async void OnInitialized()
        {
            InitializeComponent();

            Log.Listeners.Add(new DelegateLogListener(InternalLogger));
            var ea = Container.Resolve<IEventAggregator>();
            ea.GetEvent<UserAuthenticatedEvent>().Subscribe(OnUserAuthenticatedEventPublished);
            ea.GetEvent<UserLoggedOutEvent>().Subscribe(OnUserLoggedOutEventPublished);

            try
            {
                var c = Container.GetContainer();
                var registrations = c.GetServiceRegistrations();
                Trace.WriteLine(JsonConvert.SerializeObject(registrations, Formatting.Indented));
                var view = Container.Resolve<object>("login");
            }
            catch(Exception ex)
            {
                Trace.WriteLine(ex);
                Debugger.Break();
            }

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
            containerRegistry.RegisterInstance(new UIParent(null, true));
            containerRegistry.RegisterSingleton<IAuthOptions, AuthOptions>();
            containerRegistry.RegisterSingleton<ISyslogOptions, SyslogOptions>();
            containerRegistry.GetContainer().RegisterMany<SyslogLogger>(Reuse.Singleton,
                    serviceTypeCondition: t => typeof(SyslogLogger).ImplementsServiceType(t));

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
            manager.LoadModuleCompleted += OnLoadModuleCompleted;
            try
            {
                manager.Run();
            }
            catch (Exception ex)
            {
                Container.Resolve<ILogger>().Report(ex);
                Debugger.Break();
            }
        }

        protected virtual void OnLoadModuleCompleted(object sender, LoadModuleCompletedEventArgs e)
        {
            var logger = Container.Resolve<ILogger>();
            if(e.Error is null)
            {
                logger.Debug($"{e.ModuleInfo.ModuleName} has loaded successfully");
            }
            else
            {
                Debugger.Break();
                logger.Debug($"{e.ModuleInfo.ModuleName} has encountered an error while loading...");
                logger.Report(e.Error);
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
            NavigationService.NavigateAsync("/NavigationPage/MainPage");
        }
    }
}



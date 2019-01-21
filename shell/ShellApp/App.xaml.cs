using DryIoc;
using AP.AzureADAuth;
using Prism;
using Prism.DryIoc;
using Prism.Ioc;
using Prism.Logging;
using Prism.Logging.Syslog;
using Prism.Modularity;
using ShellApp.Helpers;
using ShellApp.Views;
using System;
using System.Diagnostics;
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
            var result = await NavigationService.NavigateAsync("MainPage");
            if (!result.Success)
                Debugger.Break();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
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
                logger.Debug($"{e.ModuleInfo.ModuleName} has encountered an error while loading...");
                logger.Report(e.Error);
            }
        }

        private void InternalLogger(string category, string message)
        {
            System.Trace.WriteLine($"{category}: {message}");
            Container.Resolve<ILogger>().Log($"{category}: {message}");
        }
    }
}



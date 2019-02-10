using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using AP.AzureADAuth.Tests.Mocks;
using Prism.Navigation;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xunit;
using Xunit.Abstractions;

namespace AP.AzureADAuth.Tests.Tests
{
    public class AppFixture
    {
        private ITestOutputHelper _testOutputHelper;

        public AppFixture()
        {
            Device.PlatformServices = new PlatformServices(Device.iOS);
            Device.SetIdiom(TargetIdiom.Phone);
        }

        public void UpdateTestOutputHelper(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        public XunitApp CreateApp()
        {
            PageNavigationRegistry.ClearRegistrationCache();
            var initializer = new XunitPlatformInitializer(_testOutputHelper);
            return new XunitApp(initializer);
        }
    }

    [CollectionDefinition("App collection")]
    public class AppCollection : ICollectionFixture<AppFixture>
    {

    }

    internal class PlatformServices : IPlatformServices
    {
        //private readonly IsolatedStorageFile _isolatedStorageFile = new IsolatedStorageFile();

        public PlatformServices(string runtimePlatform)
        {
            RuntimePlatform = runtimePlatform;
        }

        public bool IsInvokeRequired
        {
            get { return false; }
        }

        public string RuntimePlatform
        {
            get;
            private set;
        }

        public void BeginInvokeOnMainThread(Action action)
        {
            action();
        }

        public Ticker CreateTicker()
        {
            return null;
        }

        public Assembly[] GetAssemblies()
        {
            return new Assembly[0];
        }

        public string GetMD5Hash(string input)
        {
            throw new NotImplementedException();
        }

        public double GetNamedSize(NamedSize size, Type targetElementType, bool useOldSizes)
        {
            return 14;
        }

        public Task<Stream> GetStreamAsync(Uri uri, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public IIsolatedStorageFile GetUserStoreForApplication()
        {
            return null;
        }

        public void OpenUriAction(Uri uri)
        {
            
        }

        public void QuitApplication() { }

        public SizeRequest GetNativeSize(VisualElement view, double widthConstraint, double heightConstraint)
        {
            return new SizeRequest();
        }

        public async void StartTimer(TimeSpan interval, Func<bool> callback)
        {
            while (true)
            {
                await Task.Delay(interval);

                if (!callback())
                    return;
            }
        }
    }
}


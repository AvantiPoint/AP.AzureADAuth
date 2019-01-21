using Xamarin.Forms.Internals;
using Xunit.Abstractions;

namespace AP.AzureADAuth.Tests.Mocks.Logging
{
    internal class XunitLogListener : LogListener
    {
        private ITestOutputHelper _testOutputHelper { get; }

        public XunitLogListener(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        public override void Warning(string category, string message) =>
            _testOutputHelper.WriteLine($"  {category}: {message}");
    }
}


using NUnit.Framework;
using System.Linq;


namespace AP.AzureADAuth.UITests.Pages
{
    public class MainPage : BasePage
    {
        protected override PlatformQuery Trait =>
            new PlatformQuery
            {
                Default = x => x.Id("HelloWorld")
            };
    }
}


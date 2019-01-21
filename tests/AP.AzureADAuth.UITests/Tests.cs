using System;
using System.IO;
using System.Linq;
using AP.AzureADAuth.UITests.Pages;
using NUnit.Framework;
using Xamarin.UITest;
using Xamarin.UITest.Queries;

namespace AP.AzureADAuth.UITests
{
    public class Tests : BaseTestFixture
    {
        public Tests(Platform platform)
            : base(platform)
        {
        }

        [Test]
        public void WelcomeTextIsDisplayed()
        {
            new MainPage();
        }
    }
}


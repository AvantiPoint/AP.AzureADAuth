using Prism.Modularity;
using Xunit;
using Xunit.Abstractions;

namespace AP.AzureADAuth.Tests.Tests
{
    [Collection("App collection")]
    public class ModuleTests
    {
        private AppFixture _fixture { get; }
        private ITestOutputHelper _testOutputHelper { get; }
        public ModuleTests(ITestOutputHelper testOutputHelper)
        {
            try
            {
                _fixture = new AppFixture();
                _fixture.UpdateTestOutputHelper(testOutputHelper);
                _testOutputHelper = testOutputHelper;
            }
            catch (System.Exception ex)
            {
                _testOutputHelper.WriteLine(ex.ToString());
            }
        }

        [Fact]
        public void AppInitializesWithoutException()
        {
            //var ex = Record.Exception(() => _fixture.CreateApp());
            //_testOutputHelper.WriteLine(ex.ToString());
            //Assert.Null(ex);
        }

        [Fact]
        public void ModuleIsLoaded()
        {
            //var app = _fixture.CreateApp();
            //_testOutputHelper.WriteLine("Created App");
            //Assert.Single(app.ModuleLoadEvents);
            //_testOutputHelper.WriteLine("Had a single Module Load Event");
            //var loadEvent = app.ModuleLoadEvents[0];
            //Assert.Equal(ModuleState.Initialized, loadEvent.ModuleInfo.State);
        }

        [Fact]
        public void ModuleDidNotThrowInitializationException()
        {
            //var app = _fixture.CreateApp();
            //Assert.Single(app.ModuleLoadEvents);
            //var loadEvent = app.ModuleLoadEvents[0];
            //Assert.Null(loadEvent.Error);
        }
    }
}


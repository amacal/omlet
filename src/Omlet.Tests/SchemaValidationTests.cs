using Nancy;
using Nancy.Bootstrapper;
using Nancy.Testing;
using Xunit;

namespace Omlet.Tests
{
    [Collection("Nancy")]
    public class SchemaValidationTests
    {
        private readonly INancyBootstrapper bootstrapper;
        private readonly Browser browser;

        public SchemaValidationTests()
        {
            bootstrapper = new ConfigurableBootstrapper(with =>
            {
                with.AllDiscoveredModules();
                with.ApplicationStartup(OmletSchema.Enable);
            });

            browser = new Browser(bootstrapper);
        }

        [Fact]
        public void PostInvalidPayload()
        {
            BrowserResponse result = browser.Get("/users/search", with =>
            {
                with.Header("Content-Type", "application/json");
                with.Body(@"{ ""firstName"":"""", ""lastName"":null }");
            });

            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
            Assert.Empty(result.Body.AsString());
        }
    }
}
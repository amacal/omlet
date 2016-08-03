using Jinx;
using Jinx.Schema;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Testing;
using System.IO;
using Xunit;

namespace Omlet.Tests
{
    [Collection("Nancy")]
    public class OnRequestSchemaTests
    {
        private readonly INancyBootstrapper bootstrapper;
        private readonly Browser browser;

        public OnRequestSchemaTests()
        {
            bootstrapper = new ConfigurableBootstrapper(with =>
            {
                with.Module<UsersModule>();
            });

            browser = new Browser(bootstrapper);
        }

        [Fact]
        public void RequestWithInvalidPayload()
        {
            BrowserResponse result = browser.Get("/users/search", with =>
            {
                with.Header("Content-Type", "application/json");
                with.Body(@"{ ""firstName"":"""", ""lastName"":null }");
            });

            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
            Assert.Empty(result.Body.AsString());
        }

        public class UsersModule : NancyModule
        {
            public UsersModule()
            {
                Get["/users/search"] =
                    this.WithSchema(x => HttpStatusCode.OK)
                    .OnRequest(Schemas.UsersSearch);
            }
        }

        public static class Schemas
        {
            public static JsonSchema UsersSearch
            {
                get
                {
                    using (Stream stream = OpenStream("Schemas.users-search-embedded"))
                        return JsonConvert.GetSchema(stream);
                }
            }

            private static Stream OpenStream(string resource)
            {
                return typeof(Schemas).Assembly.GetManifestResourceStream(typeof(Schemas), resource);
            }
        }
    }
}
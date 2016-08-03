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
    public class OnResponseSchemaTests
    {
        private readonly INancyBootstrapper bootstrapper;
        private readonly Browser browser;

        public OnResponseSchemaTests()
        {
            bootstrapper = new ConfigurableBootstrapper(with =>
            {
                with.Module<UsersModule>();
            });

            browser = new Browser(bootstrapper);
        }

        [Fact]
        public void ResponseWithInvalidPayload()
        {
            BrowserResponse result = browser.Get("/users/search", with =>
            {
                with.Header("Content-Type", "application/json");
            });

            Assert.Equal(HttpStatusCode.InternalServerError, result.StatusCode);
        }

        public class UsersModule : NancyModule
        {
            public UsersModule()
            {
                Get["/users/search"] =
                    this.WithSchema(OnUserSearch)
                    .OnResponse(HttpStatusCode.OK, Schemas.UsersSearch200);
            }

            private Response OnUserSearch(dynamic parameters)
            {
                var model = new[]
                {
                    new
                    {
                        lastName = "john",
                        firstName = 13
                    }
                };

                return Response.AsJson(model);
            }
        }

        public static class Schemas
        {
            public static JsonSchema UsersSearch200
            {
                get
                {
                    using (Stream stream = OpenStream("Schemas.users-search-200"))
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
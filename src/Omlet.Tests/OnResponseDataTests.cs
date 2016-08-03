using System.IO;
using Jinx;
using Jinx.Schema;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Testing;
using Xunit;

namespace Omlet.Tests
{
    [Collection("Nancy")]
    public class OnResponseDataTests
    {
        private readonly INancyBootstrapper bootstrapper;
        private readonly Browser browser;

        public OnResponseDataTests()
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

            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.NotNull(JsonConvert.GetDocument(result.Body.AsStream()).Root);
        }

        public class UsersModule : NancyModule
        {
            public UsersModule()
            {
                Get["/users/search"] =
                    this.WithSchema(OnUserSearch)
                    .OnResponse(HttpStatusCode.OK, "/schemas/users-search-200");
            }

            private Response OnUserSearch(dynamic parameters)
            {
                var model = new[]
                {
                    new
                    {
                        lastName = "john",
                        firstName = "doe"
                    }
                };

                return Response.AsJson(model);
            }
        }
    }
}

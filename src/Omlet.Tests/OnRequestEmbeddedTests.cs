using System.Collections.Generic;
using System.Reflection;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Testing;
using Xunit;

namespace Omlet.Tests
{
    [Collection("Nancy")]
    public class OnRequestEmbeddedTests
    {
        private readonly INancyBootstrapper bootstrapper;
        private readonly Browser browser;

        public OnRequestEmbeddedTests()
        {
            bootstrapper = new ConfigurableBootstrapper(with =>
            {
                with.Module<UsersModule>();
                with.Dependency<ISchemaConfiguration>(new SchemaConfiguration());
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

        [Fact]
        public void RequestWithValidPayload()
        {
            BrowserResponse result = browser.Get("/users/search", with =>
            {
                with.Header("Content-Type", "application/json");
                with.Body(@"{ ""firstName"":"""", ""lastName"":"""" }");
            });

            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Empty(result.Body.AsString());
        }

        public class UsersModule : NancyModule
        {
            public UsersModule()
            {
                Get["/users/search"] =
                    this.WithSchema(x => HttpStatusCode.OK)
                        .OnRequest("/schemas/users-search-embedded");
            }
        }

        public class SchemaConfiguration : ISchemaConfiguration
        {
            public IEnumerable<Assembly> Assemblies
            {
                get { yield return typeof(SchemaConfiguration).Assembly; }
            }
        }
    }
}
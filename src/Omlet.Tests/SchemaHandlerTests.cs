using System.Collections.Generic;
using Jinx.Schema;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Testing;
using Xunit;
using Nancy.Responses;

namespace Omlet.Tests
{
    public class SchemaHandlerTests
    {
        private readonly INancyBootstrapper bootstrapper;
        private readonly Browser browser;

        public SchemaHandlerTests()
        {
            bootstrapper = new ConfigurableBootstrapper(with =>
            {
                with.AllDiscoveredModules();
                with.ApplicationStartup(OmletSchema.Enable);
                with.Dependency<ISchemaHandler>(typeof(SchemaHandler));
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
            Assert.NotEmpty(result.Body.AsString());
        }

        private class SchemaHandler : ISchemaHandler
        {
            public Response OnBrokenRequest(NancyContext context, Request request, IResponseFormatter formatter, ICollection<JsonSchemaMessage> violations)
            {
                return formatter.AsJson(new { }, HttpStatusCode.BadRequest);
            }
        }
    }
}

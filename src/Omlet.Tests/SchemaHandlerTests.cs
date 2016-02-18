using Jinx.Schema;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Testing;
using System.Collections.Generic;
using Xunit;

namespace Omlet.Tests
{
    [Collection("Nancy")]
    public class SchemaHandlerTests
    {
        private readonly INancyBootstrapper bootstrapper;
        private readonly Browser browser;

        public SchemaHandlerTests()
        {
            bootstrapper = new ConfigurableBootstrapper(with =>
            {
                with.Module<UsersModule>();
                with.ApplicationStartup(OmletSchema.Enable);
                with.Dependency<ISchemaHandler>(typeof(SchemaHandler));
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
            Assert.NotEmpty(result.Body.AsString());
        }

        [Fact]
        public void ResponseWithInvalidPayload()
        {
            BrowserResponse result = browser.Get("/users/search", with =>
            {
                with.Header("Content-Type", "application/json");
                with.Body(@"{ ""firstName"":"""", ""lastName"":"""" }");
            });

            Assert.Equal(HttpStatusCode.InternalServerError, result.StatusCode);
        }

        public class UsersModule : NancyModule
        {
            public UsersModule()
            {
                Get["/users/search"] =
                    this.WithSchema(OnUserSearch)
                    .OnRequest("/schemas/users-search")
                    .OnResponse(HttpStatusCode.OK, "/schemas/users-search-200");
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

        private class SchemaHandler : ISchemaHandler
        {
            public Response OnBrokenRequest(NancyContext context, Request request, IResponseFormatter formatter, ICollection<JsonSchemaMessage> violations)
            {
                return formatter.AsJson(new { }, HttpStatusCode.BadRequest);
            }

            public Response OnBrokerResponse(NancyContext context, Request request, Response response, IResponseFormatter formatter, ICollection<JsonSchemaMessage> violations)
            {
                return HttpStatusCode.InternalServerError;
            }
        }
    }
}
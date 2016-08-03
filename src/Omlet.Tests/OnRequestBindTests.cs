using System;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.ModelBinding;
using Nancy.Testing;
using Xunit;

namespace Omlet.Tests
{
    [Collection("Nancy")]
    public class OnRequestBindTests
    {
        private readonly INancyBootstrapper bootstrapper;
        private readonly Browser browser;

        public OnRequestBindTests()
        {
            bootstrapper = new ConfigurableBootstrapper(with =>
            {
                with.Module<UsersModule>();
            });

            browser = new Browser(bootstrapper);
        }

        [Fact]
        public void RequestWithValidPayload()
        {
            BrowserResponse result = browser.Get("/users/search", with =>
            {
                with.Header("Content-Type", "application/json");
                with.Body(@"{ ""firstName"":""John"", ""lastName"":""Doe"" }");
            });

            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Empty(result.Body.AsString());
        }

        public class UsersModule : NancyModule
        {
            public UsersModule()
            {
                Get["/users/search"] =
                    this.WithSchema(x => Handle())
                    .OnRequest("/schemas/users-search");
            }

            private object Handle()
            {
                UserModel model = this.Bind<UserModel>();

                if (String.IsNullOrWhiteSpace(model.firstName))
                    return HttpStatusCode.InternalServerError;

                if (String.IsNullOrWhiteSpace(model.lastName))
                    return HttpStatusCode.InternalServerError;

                return HttpStatusCode.OK;
            }
        }

        public class UserModel
        {
            public string firstName { get; set; }
            public string lastName { get; set; }
        }
    }
}

using Nancy;

namespace Omlet.Tests.Modules
{
    public class UsersModule : NancyModule
    {
        public UsersModule()
        {
            Get["/users/search"] = this.WithSchema("/schemas/users-search", x => HttpStatusCode.OK);
        }
    }
}
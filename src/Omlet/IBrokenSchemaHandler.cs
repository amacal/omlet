using Nancy;

namespace Omlet
{
    public interface IBrokenSchemaHandler
    {
        Response OnRequest(NancyContext context, Request request, object violations);

        //Response OnResponse(NancyContext context, Response response, object violations);
    }
}

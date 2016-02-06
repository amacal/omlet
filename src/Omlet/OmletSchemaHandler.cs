using Nancy;
using System;

namespace Omlet
{
    public class OmletSchemaHandler
    {
        public OmletSchemaHandler()
        {
            OnRequest = OnRequestFallback;
            OnResponse = OnResponseFallback;
        }

        public OmletSchemaHandler(IBrokenSchemaHandler handler)
        {
            OnRequest = handler.OnRequest;
            //OnResponse = handler.OnResponse;
        }

        public Func<NancyContext, Request, object, Response> OnRequest { get; private set; }

        public Func<NancyContext, Response, object, Response> OnResponse { get; private set; }

        private Response OnRequestFallback(NancyContext context, Request request, object violations)
        {
            return HttpStatusCode.BadRequest;
        }

        private Response OnResponseFallback(NancyContext context, Response response, object violations)
        {
            return HttpStatusCode.InternalServerError;
        }
    }
}

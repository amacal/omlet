using Jinx.Schema;
using Nancy;
using System;
using System.Collections.Generic;

namespace Omlet
{
    public class OmletSchemaHandler
    {
        private readonly Func<NancyContext, Request, IResponseFormatter, ICollection<JsonSchemaMessage>, Response> onRequest;
        private readonly Func<NancyContext, Request, Response, IResponseFormatter, ICollection<JsonSchemaMessage>, Response> onResponse;

        public OmletSchemaHandler()
        {
            onRequest = OnRequestFallback;
            onResponse = OnResponseFallback;
        }

        public OmletSchemaHandler(ISchemaHandler handler)
        {
            onRequest = handler.OnBrokenRequest;
            onResponse = handler.OnBrokerResponse;
        }

        public Func<NancyContext, Request, IResponseFormatter, ICollection<JsonSchemaMessage>, Response> OnRequest
        {
            get { return onRequest; }
        }

        public Func<NancyContext, Request, Response, IResponseFormatter, ICollection<JsonSchemaMessage>, Response> OnResponse
        {
            get { return onResponse; }
        }

        private static Response OnRequestFallback(NancyContext context, Request request, IResponseFormatter formatter, ICollection<JsonSchemaMessage> violations)
        {
            return HttpStatusCode.BadRequest;
        }

        private static Response OnResponseFallback(NancyContext context, Request request, Response response, IResponseFormatter formatter, ICollection<JsonSchemaMessage> violations)
        {
            return HttpStatusCode.InternalServerError;
        }
    }
}
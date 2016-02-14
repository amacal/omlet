using Jinx.Schema;
using Nancy;
using System;
using System.Collections.Generic;

namespace Omlet
{
    public class OmletSchemaHandler
    {
        public OmletSchemaHandler()
        {
            OnRequest = OnRequestFallback;
        }

        public OmletSchemaHandler(ISchemaHandler handler)
        {
            OnRequest = handler.OnBrokenRequest;
        }

        public Func<NancyContext, Request, IResponseFormatter, ICollection<JsonSchemaMessage>, Response> OnRequest { get; private set; }

        private static Response OnRequestFallback(NancyContext context, Request request, IResponseFormatter formatter, ICollection<JsonSchemaMessage> violations)
        {
            return HttpStatusCode.BadRequest;
        }
    }
}
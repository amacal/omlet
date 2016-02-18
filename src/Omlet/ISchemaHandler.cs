using Jinx.Schema;
using Nancy;
using System.Collections.Generic;

namespace Omlet
{
    public interface ISchemaHandler
    {
        Response OnBrokenRequest(NancyContext context, Request request, IResponseFormatter formatter, ICollection<JsonSchemaMessage> violations);

        Response OnBrokerResponse(NancyContext context, Request request, Response response, IResponseFormatter formatter, ICollection<JsonSchemaMessage> violations);
    }
}
using Jinx;
using Jinx.Dom;
using Jinx.Schema;
using Nancy;
using System;
using System.Collections.Generic;
using System.IO;

namespace Omlet
{
    public class OmletSchemaBuilder
    {
        private readonly INancyModule module;
        private readonly Func<dynamic, dynamic> callback;

        private JsonSchema onRequest;
        private Dictionary<HttpStatusCode, JsonSchema> onResponse;

        public OmletSchemaBuilder(INancyModule module, Func<dynamic, dynamic> callback)
        {
            this.module = module;
            this.callback = callback;
            this.onResponse = new Dictionary<HttpStatusCode, JsonSchema>();
        }

        public OmletSchemaBuilder OnRequest(string path)
        {
            string root = OmletSchema.RootProvider.GetRootPath();
            string full = Path.Combine(root, path.Trim('/', '\\')).Trim('/', '\\').Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar);

            return OnRequest(JsonConvert.GetSchema(full));
        }

        public OmletSchemaBuilder OnRequest(JsonSchema schema)
        {
            onRequest = schema;
            return this;
        }

        public OmletSchemaBuilder OnResponse(HttpStatusCode status, string path)
        {
            string root = OmletSchema.RootProvider.GetRootPath();
            string full = Path.Combine(root, path.Trim('/', '\\')).Trim('/', '\\').Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar);

            return OnResponse(status, JsonConvert.GetSchema(full));
        }

        public OmletSchemaBuilder OnResponse(HttpStatusCode status, JsonSchema schema)
        {
            onResponse[status] = schema;
            return this;
        }

        public static implicit operator Func<dynamic, dynamic>(OmletSchemaBuilder builder)
        {
            if (OmletSchema.IsEnabled == false)
                return builder.callback;

            if (builder.onRequest == null && builder.onResponse.Count == 0)
                return builder.callback;

            return parameters =>
            {
                JsonSchema schema;
                JsonDocument document;

                INancyModule module = builder.module;
                List<JsonSchemaMessage> messages = new List<JsonSchemaMessage>();

                if (builder.onRequest != null)
                {
                    schema = builder.onRequest;
                    document = JsonConvert.GetDocument(module.Request.Body);

                    if (document != null && schema.IsValid(document.Root, messages) == false)
                        return OmletSchema.SchemaHandler.OnRequest(module.Context, module.Request, module.Response, messages);
                }

                Response response = builder.callback(parameters);
                HttpStatusCode status = response.StatusCode;

                builder.onResponse.TryGetValue(status, out schema);

                if (schema != null)
                {
                    using (MemoryStream output = new MemoryStream())
                    {
                        messages.Clear();
                        response.Contents(output);
                        output.Seek(0, SeekOrigin.Begin);
                        document = JsonConvert.GetDocument(output);
                    }

                    if (document != null)
                        if (schema.IsValid(document.Root, messages) == false)
                            return OmletSchema.SchemaHandler.OnResponse(module.Context, module.Request, response, module.Response, messages);
                }

                return response;
            };
        }
    }
}
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

        private JsonSchema schema;

        public OmletSchemaBuilder(INancyModule module, Func<dynamic, dynamic> callback)
        {
            this.module = module;
            this.callback = callback;
        }

        public OmletSchemaBuilder OnRequest(string path)
        {
            string root = OmletSchema.RootProvider.GetRootPath();
            string full = Path.Combine(root, path.Trim('/', '\\')).Trim('/', '\\').Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar);

            return OnRequest(JsonConvert.GetSchema(full));
        }

        public OmletSchemaBuilder OnRequest(JsonSchema schema)
        {
            this.schema = schema;
            return this;
        }

        public static implicit operator Func<dynamic, dynamic>(OmletSchemaBuilder builder)
        {
            if (OmletSchema.IsEnabled == false)
                return builder.callback;

            if (builder.schema == null)
                return builder.callback;

            return parameters =>
            {
                INancyModule module = builder.module;
                List<JsonSchemaMessage> messages = new List<JsonSchemaMessage>();
                JsonDocument request = JsonConvert.GetDocument(module.Request.Body);

                if (builder.schema.IsValid(request.Root, messages))
                    return builder.callback(parameters);

                return OmletSchema.SchemaHandler.OnRequest(module.Context, module.Request, module.Response, messages);
            };
        }
    }
}
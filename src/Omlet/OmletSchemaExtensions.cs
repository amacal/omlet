﻿using Jinx;
using Jinx.Dom;
using Jinx.Schema;
using Nancy;
using System;
using System.Collections.Generic;
using System.IO;

namespace Omlet
{
    public static class OmletSchemaExtensions
    {
        public static Func<dynamic, dynamic> WithSchema(this INancyModule module, string schemaPath, Func<dynamic, dynamic> callback)
        {
            if (OmletSchema.IsEnabled == false)
                return callback;

            return parameters =>
            {
                string root = OmletSchema.RootProvider.GetRootPath();
                string path = Path.Combine(root, schemaPath.Trim('/', '\\')).Trim('/', '\\').Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar);

                List<JsonSchemaMessage> messages = new List<JsonSchemaMessage>();
                JsonDocument request = JsonConvert.GetDocument(module.Request.Body);
                JsonSchema schema = JsonConvert.GetSchema(path);

                if (schema.IsValid(request.Root, messages))
                    return callback(parameters);

                return OmletSchema.SchemaHandler.OnRequest(module.Context, module.Request, module.Response, messages);
            };
        }
    }
}
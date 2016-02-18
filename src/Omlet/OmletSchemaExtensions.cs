using Nancy;
using System;

namespace Omlet
{
    public static class OmletSchemaExtensions
    {
        public static OmletSchemaBuilder WithSchema(this INancyModule module, Func<dynamic, dynamic> callback)
        {
            return new OmletSchemaBuilder(module, callback);
        }
    }
}
using System;
using System.Collections.Generic;
using Jinx.Schema;

namespace Omlet
{
    public class OmletSchemaCache
    {
        private readonly Func<string, JsonSchema> fallback;
        private readonly Dictionary<string, JsonSchema> items;
        private readonly object synchronize;

        public OmletSchemaCache(Func<string, JsonSchema> fallback)
        {
            this.fallback = fallback;
            this.items = new Dictionary<string, JsonSchema>();
            this.synchronize = new object();
        }

        public JsonSchema Resolve(string path)
        {
            JsonSchema schema;

            lock (synchronize)
            {
                if (items.TryGetValue(path, out schema) == false)
                {
                    schema = fallback.Invoke(path);
                    items.Add(path, schema);
                }
            }

            return schema;
        }
    }
}

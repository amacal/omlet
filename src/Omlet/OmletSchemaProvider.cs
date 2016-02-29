using System;
using System.IO;
using System.Reflection;
using Jinx;
using Jinx.Schema;
using Nancy;

namespace Omlet
{
    public class OmletSchemaProvider
    {
        private readonly IRootPathProvider rootProvider;
        private readonly ISchemaConfiguration configuration;

        public OmletSchemaProvider(IRootPathProvider rootProvider, ISchemaConfiguration configuration)
        {
            this.rootProvider = rootProvider;
            this.configuration = configuration;
        }

        public JsonSchema GetSchema(string path)
        {
            string root = rootProvider.GetRootPath();
            string full = Path.Combine(root, path.Trim('/', '\\')).Trim('/', '\\').Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar);

            if (File.Exists(full))
                return JsonConvert.GetSchema(full);

            if (configuration != null)
            {
                foreach (Assembly assembly in configuration.Assemblies)
                {
                    foreach (string resource in assembly.GetManifestResourceNames())
                    {
                        if (resource.EndsWith(path.Replace("/", "."), StringComparison.InvariantCultureIgnoreCase))
                        {
                            using (Stream stream = assembly.GetManifestResourceStream(resource))
                            {
                                return JsonConvert.GetSchema(stream);
                            }
                        }
                    }
                }
            }

            return null;
        }
    }
}

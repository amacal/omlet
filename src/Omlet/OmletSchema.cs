using Nancy;
using Nancy.Bootstrapper;
using Nancy.TinyIoc;

namespace Omlet
{
    public static class OmletSchema
    {
        private static bool isEnabled;
        private static OmletSchemaProvider schemaProvider;
        private static OmletSchemaHandler schemaHandler;

        public static void Enable(TinyIoCContainer container, IPipelines pipelines)
        {
            ISchemaHandler handler;
            container.TryResolve(out handler);

            ISchemaConfiguration configuration;
            container.TryResolve(out configuration);

            isEnabled = true;
            schemaProvider = new OmletSchemaProvider(container.Resolve<IRootPathProvider>(), configuration);
            schemaHandler = new OmletSchemaHandler(handler);
        }

        public static bool IsEnabled
        {
            get { return isEnabled; }
        }

        public static OmletSchemaProvider SchemaProvider
        {
            get { return schemaProvider; }
        }

        public static OmletSchemaHandler SchemaHandler
        {
            get { return schemaHandler; }
        }
    }
}
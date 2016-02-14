using Nancy;
using Nancy.Bootstrapper;
using Nancy.TinyIoc;

namespace Omlet
{
    public static class OmletSchema
    {
        private static bool isEnabled;
        private static IRootPathProvider rootProvider;
        private static OmletSchemaHandler schemaHandler;

        public static void Enable(TinyIoCContainer container, IPipelines pipelines)
        {
            ISchemaHandler handler;
            container.TryResolve(out handler);

            isEnabled = true;
            rootProvider = container.Resolve<IRootPathProvider>();
            schemaHandler = handler != null ? new OmletSchemaHandler(handler) : new OmletSchemaHandler();
        }

        public static bool IsEnabled
        {
            get { return isEnabled; }
        }

        public static IRootPathProvider RootProvider
        {
            get { return rootProvider; }
        }

        public static OmletSchemaHandler SchemaHandler
        {
            get { return schemaHandler; }
        }
    }
}
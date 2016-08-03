using Nancy;
using Nancy.Bootstrapper;
using Nancy.TinyIoc;

namespace Omlet
{
    public class OmletStartup : IApplicationStartup
    {
        private readonly TinyIoCContainer container;

        public OmletStartup(TinyIoCContainer container)
        {
            this.container = container;
        }

        public void Initialize(IPipelines pipelines)
        {
            OmletSchema.Enable(container, pipelines);
        }
    }
}

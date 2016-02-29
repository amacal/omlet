using System.Collections.Generic;
using System.Reflection;

namespace Omlet
{
    public interface ISchemaConfiguration
    {
        IEnumerable<Assembly> Assemblies { get; }
    }
}

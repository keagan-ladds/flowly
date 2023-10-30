using Flowly.Core.Providers;
using System.Reflection;

namespace Flowly.Core
{
    public interface IRuntimeDependencyResolver
    {
        void ResolveForAssembly(IExtensionProvider extensionProvider, Assembly assembly);
    }
}

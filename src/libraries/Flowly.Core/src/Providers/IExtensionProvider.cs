using Flowly.Core.Definitions;
using System;
using System.Threading.Tasks;

namespace Flowly.Core.Providers
{
    public interface IExtensionProvider
    {
        Task LoadAsync(ExtensionDefinition[] extensions);
        bool TryResolveType(string name, out Type type);
        bool TryResolveRuntimeDependency(string dependency, out string filePath);
    }
}

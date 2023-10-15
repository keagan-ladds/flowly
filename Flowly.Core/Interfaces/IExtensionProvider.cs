using Flowly.Core.Definitions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Flowly.Core.Interfaces
{
    public interface IExtensionProvider
    {
        Task LoadAsync(ExtensionDefinition[] extensions);
        Type? ResolveType(string typeName);
    }
}

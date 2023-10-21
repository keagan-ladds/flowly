using Flowly.Core.Providers;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Flowly.Core
{
    public interface IRuntimeDependencyResolver
    {
        void ResolveForAssembly(IExtensionProvider extensionProvider, Assembly assembly);
    }
}

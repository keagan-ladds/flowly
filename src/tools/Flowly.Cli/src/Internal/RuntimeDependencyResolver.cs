using Flowly.Core;
using Flowly.Core.Providers;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Flowly.Cli.Internal
{
    internal class RuntimeDependencyResolver : IRuntimeDependencyResolver
    {
        public IntPtr DllImportResolver(IExtensionProvider extensionProvider, string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
        {
            if (extensionProvider.TryResolveRuntimeDependency(libraryName, out var filePath))
            {
                if (NativeLibrary.TryLoad(filePath, out var handle))
                {
                    return handle;
                }
            }

            return IntPtr.Zero;
        }

        public void ResolveForAssembly(IExtensionProvider extensionProvider, Assembly assembly)
        {
            NativeLibrary.SetDllImportResolver(assembly, (libraryName, assembly, searchPath) =>
            {
                return DllImportResolver(extensionProvider, libraryName, assembly, searchPath);
            });
        }
    }
}

using Flowly.Core.Providers;
using System;

namespace Flowly.Core.Internal
{
    internal class ExtensionTypeResolver : ReflectionTypeResolver, ITypeResolver
    {
        private readonly IExtensionProvider _extensionProvider;
        public ExtensionTypeResolver(IExtensionProvider extensionProvider)
        {
            _extensionProvider = extensionProvider ?? throw new ArgumentNullException(nameof(extensionProvider));
        }

        public override bool TryResolveType(string name, out Type type)
        {
            if (_extensionProvider.TryResolveType(name, out type)) 
                return true;

            return base.TryResolveType(name, out type);
        }
    }
}

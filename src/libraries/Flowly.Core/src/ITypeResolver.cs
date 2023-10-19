using System;

namespace Flowly.Core
{
    public interface ITypeResolver
    {
        bool TryResolveType(string name, out Type type);
    }
}

using System;
using System.Collections.Generic;
using System.Reflection;

namespace Flowly.Core.Internal
{
    internal class ReflectionTypeResolver : ITypeResolver
    {
        static Dictionary<string, Type> TypeCache = new Dictionary<string, Type>();

        public virtual bool TryResolveType(string name, out Type type)
        {
            lock (TypeCache)
            {
                if (!TypeCache.TryGetValue(name, out type))
                {
                    foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
                    {
                        type = a.GetType(name);
                        if (type != null)
                            break;
                    }
                    TypeCache[name] = type; 
                }
            }
            return type != null;
        }
    }
}

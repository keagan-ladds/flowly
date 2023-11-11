using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;

namespace Flowly.Core.Internal
{
    internal static class TypeMapper
    {
        public static void Map(ExpandoObject source, object destination)
        {
            Map((IDictionary<string, object>)source, destination);
        }

        public static void Map(IDictionary<string, object> source, object destination)
        {
            source = source ?? throw new ArgumentNullException(nameof(source));
            destination = destination ?? throw new ArgumentNullException(nameof(destination));

            string normalizeName(string name) => name.ToLowerInvariant();

            var type = destination.GetType();

            var setters = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanWrite && p.GetSetMethod() != null)
                .ToDictionary(p => normalizeName(p.Name));

            foreach (var item in source)
            {
                if (setters.TryGetValue(normalizeName(item.Key), out var setter))
                {
                    var propertyType = setter.PropertyType;
                    if (propertyType.IsArray && Type.GetTypeCode(propertyType.GetElementType()) == TypeCode.Object) 
                    {
                        List<object> objs = ((IEnumerable)item.Value).Cast<object>().ToList();
                        var elementType = propertyType.GetElementType();
                        var array = Array.CreateInstance(elementType, objs.Count);
                        for (int i = 0; i < objs.Count; i++)
                        {
                            var elementValue = Activator.CreateInstance(elementType);
                            var nestedObject = ((IDictionary<object, object>)objs[i]).ToDictionary(_ => _.Key.ToString(), _ => _.Value);
                            Map(nestedObject, elementValue);
                            array.SetValue(elementValue, i);
                        }

                        setter.SetValue(destination, array);
                    } 
                    else
                    {
                        var value = propertyType.ChangeType(item.Value);
                        setter.SetValue(destination, value);
                    }
                }
            }
        }
    }
}

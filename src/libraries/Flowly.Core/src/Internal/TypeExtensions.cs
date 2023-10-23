using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Flowly.Core.Internal
{
    internal static class TypeExtensions
    {
        public static bool IsNullable(this Type type)
        {
            type = type ?? throw new ArgumentNullException(nameof(type));
            return type.IsGenericType
                && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>));
        }

        public static bool IsNullAssignable(this Type type)
        {
            type = type ?? throw new ArgumentNullException(nameof(type));
            return type.IsNullable() || !type.IsValueType;
        }

        public static object ChangeType(this Type type, object instance)
        {
            type = type ?? throw new ArgumentNullException(nameof(type));
            var t = instance.GetType();
            if (instance == null)
            {
                if (!type.IsNullAssignable())
                {
                    throw new InvalidCastException($"{type.FullName} is not null-assignable");
                }
                return null;
            }
            if (type.IsNullable())
            {
                type = Nullable.GetUnderlyingType(type);
            }


            if (instance.IsList())
            {
                List<object> objs = ((IEnumerable)instance).Cast<object>().ToList();
                Type containedType = instance.GetType().GenericTypeArguments.First();

                if (type.IsList())
                    return objs.Select(item => Convert.ChangeType(item, type)).ToList();

                if (type.IsArray)
                {
                    var elementType = type.GetElementType();
                    var array = Array.CreateInstance(elementType, objs.Count);
                    for (int i = 0; i < objs.Count; i++)
                    {
                        var value = Convert.ChangeType(objs[i], elementType);
                        array.SetValue(objs[i], i);
                    }

                    return array;
                }

            }

            return Convert.ChangeType(instance, type);
        }

        public static bool IsInstanceOfGenericType(this Type type, Type baseType, out Type typedJobStep)
        {
            typedJobStep = null;

            while (type != null)
            {
                if (type.IsGenericType &&
                    type.GetGenericTypeDefinition() == baseType)
                {
                    typedJobStep = type;
                    return true;
                }
                type = type.BaseType;
            }
            return false;
        }

        public static bool IsList(this object o)
        {
            if (o == null) return false;
            return o is IList &&
                o.GetType().IsGenericType &&
                o.GetType().GetGenericTypeDefinition().IsAssignableFrom(typeof(List<>));
        }
    }
}

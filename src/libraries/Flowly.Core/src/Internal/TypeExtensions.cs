using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Flowly.Core.Internal
{
    /// <summary>
    /// A static class containing extension methods for working with types.
    /// </summary>
    internal static class TypeExtensions
    {
        /// <summary>
        /// Determines whether a given type is nullable.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns><c>true</c> if the type is nullable; otherwise, <c>false</c>.</returns>
        public static bool IsNullable(this Type type)
        {
            type = type ?? throw new ArgumentNullException(nameof(type));
            return type.IsGenericType
                && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>));
        }

        /// <summary>
        /// Determines whether a given type is nullable or reference type (not value type).
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns><c>true</c> if the type is nullable or reference type; otherwise, <c>false</c>.</returns>
        public static bool IsNullAssignable(this Type type)
        {
            type = type ?? throw new ArgumentNullException(nameof(type));
            return type.IsNullable() || !type.IsValueType;
        }

        /// <summary>
        /// Converts an object to a specified type, handling nullable and list conversions.
        /// </summary>
        /// <param name="type">The target type for conversion.</param>
        /// <param name="instance">The object to be converted.</param>
        /// <returns>The converted object of the specified type.</returns>
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

        /// <summary>
        /// Determines whether a type is an instance of a specified generic type.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <param name="baseType">The generic type definition to compare against.</param>
        /// <param name="typedJobStep">The specific type that matches the generic definition, if found.</param>
        /// <returns><c>true</c> if the type is an instance of the specified generic type; otherwise, <c>false</c>.</returns>
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

        /// <summary>
        /// Determines whether an object is of a list type.
        /// </summary>
        /// <param name="o">The object to check.</param>
        /// <returns><c>true</c> if the object is a list; otherwise, <c>false</c>.</returns>
        public static bool IsList(this object o)
        {
            if (o == null) return false;
            return o is IList &&
                o.GetType().IsGenericType &&
                o.GetType().GetGenericTypeDefinition().IsAssignableFrom(typeof(List<>));
        }
    }
}

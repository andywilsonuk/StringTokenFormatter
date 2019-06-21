using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace StringTokenFormatter
{
    /// <summary>
    /// Converts only the properties exposed by {T} (but not any members on derived classes) to a token value container.
    /// This implementation runs ~15% faster than the non-generic version by caching the TypeDescriptor lookups.
    /// </summary>
    /// <typeparam name="T">A type indicating the exact properties that will be used for formatting.</typeparam>
    public class ObjectPropertiesTokenValueContainer<T> : ITokenValueContainer {

        private static readonly IDictionary<PropertyInfo, Func<T, Object>> propertyCache;
        static ObjectPropertiesTokenValueContainer(){
            propertyCache = (
                from x in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.Instance)
                let GetMethod = x.GetGetMethod()
                where GetMethod != null && GetMethod.GetParameters().Length == 0
                let Getter = CreateGetter(GetMethod)
                select new {
                    Property = x,
                    Getter = Getter
                }).ToDictionary(x => x.Property, x => x.Getter);
                
        }

        private static Func<T, object> CreateGetter(MethodInfo Getter) {
            var instance = System.Linq.Expressions.Expression.Parameter(typeof(T), "instance");

            var ex = Expression.Convert(
                Expression.Call(instance, Getter),
                typeof(object)
                );
            
            var parameters = new ParameterExpression[] { instance };

            var ret = Expression.Lambda<Func<T, object>>(ex, parameters).Compile();

            return ret;
        }

        private IDictionary<string, NonLockingLazy<object>> dictionary;
        private readonly ITokenMatcher matcher;

        public ObjectPropertiesTokenValueContainer(T tokenValueObject, ITokenMatcher tokenMatcher)
        {
            if (tokenValueObject == null) throw new ArgumentNullException(nameof(tokenValueObject));
            matcher = tokenMatcher ?? throw new ArgumentNullException(nameof(tokenMatcher));
            dictionary = ConvertObjectToDictionary(tokenValueObject);
        }

        private IDictionary<string, NonLockingLazy<object>> ConvertObjectToDictionary(T values)
        {
            var mappings = new Dictionary<string, NonLockingLazy<object>>(matcher.TokenNameComparer);

            foreach (var property in propertyCache)
            {
                mappings[property.Key.Name] = new NonLockingLazy<object>(() => property.Value(values));
            }

            return mappings;
        }

        public bool TryMap(IMatchedToken matchedToken, out object mapped)
        {
            if (dictionary.TryGetValue(matchedToken.Token, out var lazy))
            {
                mapped = lazy.Value;
                return true;
            }
            mapped = null;
            return false;
        }
    }
}

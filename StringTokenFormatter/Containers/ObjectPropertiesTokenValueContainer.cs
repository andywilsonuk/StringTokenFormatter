using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;

namespace StringTokenFormatter
{
    /// <summary>
    /// Converts all of an objects properties to a Token Value Converter.
    /// </summary>
    public class ObjectPropertiesTokenValueContainer : ITokenValueContainer
    {
        private IDictionary<string, NonLockingLazy<object>> dictionary;
        private readonly ITokenMatcher matcher;

        public ObjectPropertiesTokenValueContainer(object tokenValueObject, ITokenMatcher tokenMatcher)
        {
            if (tokenValueObject == null) throw new ArgumentNullException(nameof(tokenValueObject));
            matcher = tokenMatcher ?? throw new ArgumentNullException(nameof(tokenMatcher));
            dictionary = ConvertObjectToDictionary(tokenValueObject);
        }

        private IDictionary<string, NonLockingLazy<object>> ConvertObjectToDictionary(object values) {
            var mappings = new Dictionary<string, NonLockingLazy<object>>(matcher.TokenNameComparer);

            foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(values))
            {
                mappings[descriptor.Name] = new NonLockingLazy<object>(() => descriptor.GetValue(values));
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

    /// <summary>
    /// Converts only the properties exposed by {T} (but not any members on derived classes) to a token value container.
    /// This implementation runs ~15% faster than the non-generic version by caching the TypeDescriptor lookups.
    /// </summary>
    /// <typeparam name="T">A type indicating the exact properties that will be used for formatting.</typeparam>
    public class ObjectPropertiesTokenValueContainer<T> : ITokenValueContainer {
        private IDictionary<string, NonLockingLazy<object>> dictionary;
        private readonly ITokenMatcher matcher;

        static List<PropertyDescriptor> PropertyDescriptors = TypeDescriptor.GetProperties(typeof(T)).OfType<PropertyDescriptor>().ToList();
        static ObjectPropertiesTokenValueContainer() {

        }

        public ObjectPropertiesTokenValueContainer(T tokenValueObject, ITokenMatcher tokenMatcher) {
            if (tokenValueObject == null) throw new ArgumentNullException(nameof(tokenValueObject));
            matcher = tokenMatcher ?? throw new ArgumentNullException(nameof(tokenMatcher));
            dictionary = ConvertObjectToDictionary(tokenValueObject);
        }

        private IDictionary<string, NonLockingLazy<object>> ConvertObjectToDictionary(T values) {
            var mappings = new Dictionary<string, NonLockingLazy<object>>(matcher.TokenNameComparer);

            foreach (var descriptor in PropertyDescriptors) {
                mappings[descriptor.Name] = new NonLockingLazy<object>(() => descriptor.GetValue(values));
            }

            return mappings;
        }

        public bool TryMap(IMatchedToken matchedToken, out object mapped) {
            if (dictionary.TryGetValue(matchedToken.Token, out var lazy)) {
                mapped = lazy.Value;
                return true;
            }
            mapped = null;
            return false;
        }
    }





    /// <summary>
    /// This class mimics the System.Lazy type except it specifically does not have locking implemented.  This is a big performance gain in multi-threaded scenarios.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    class NonLockingLazy<T> {

        private Func<T> Creator;

        public NonLockingLazy(Func<T> Creator) {
            this.Creator = Creator;
        }


        public bool IsValueCreated { get; private set; }
        public T CreatedValue { get; private set; }

        public T Value {
            get {
                if (!IsValueCreated) {
                    CreatedValue = Creator();
                    IsValueCreated = true;
                }

                return CreatedValue;
            }
        }

    }
}
using System;
using System.Collections.Generic;

namespace StringTokenFormatter {
    public static class TokenValueContainer {
        public static ObjectPropertiesTokenValueContainer<T> FromObject<T>(T values, ITokenNameComparer? nameComparer = default) {
            return ObjectPropertiesTokenValueContainerFactory.Create(values, nameComparer);
        }

        public static ITokenValueContainer FromObject(object values, ITokenNameComparer? nameComparer = default) {
            return ObjectPropertiesTokenValueContainerFactory.Create(values, nameComparer);
        }

        public static SingleTokenValueContainer<T> FromValue<T>(string name, T value, ITokenNameComparer? nameComparer = default, ITokenParser? parser = default) {
            return new SingleTokenValueContainer<T>(name, value, nameComparer, parser);
        }

        public static FuncTokenValueContainer<T> FromFunc<T>(Func<string, ITokenNameComparer, T> values, ITokenNameComparer? nameComparer = default) {
            return new FuncTokenValueContainer<T>(values, nameComparer);
        }

        public static FuncTokenValueContainer<T> FromFunc<T>(Func<string, T> values, ITokenNameComparer? nameComparer = default) {
            return new FuncTokenValueContainer<T>(values, nameComparer);
        }

        public static DictionaryTokenValueContainer<T> FromDictionary<T>(IEnumerable<KeyValuePair<string, T>> values, ITokenNameComparer? nameComparer = default, ITokenParser? parser = default) {
            return new DictionaryTokenValueContainer<T>(values, nameComparer, parser);
        }

        public static CompositeTokenValueContainer Combine(IEnumerable<ITokenValueContainer> containers) {
            return new CompositeTokenValueContainer(containers);
        }

        public static CompositeTokenValueContainer Combine(params ITokenValueContainer[] containers) {
            return new CompositeTokenValueContainer(containers);
        }

        public static EmptyTokenValueContainer Empty {
            get {
                return EmptyTokenValueContainer.Instance;
            }
        }

        /// <summary>
        /// Wraps the provided <see cref="ITokenValueContainer"/> to prevent replacement of null values.
        /// </summary>
        public static IgnoreNullOrEmptyTokenValueContainer IgnoreNullTokenValues(this ITokenValueContainer This) {
            return new IgnoreNullOrEmptyTokenValueContainer(This);
        }

        /// <summary>
        /// Wraps the provided <see cref="ITokenValueContainer"/> to prevent replacement of null or empty values.
        /// </summary>
        public static IgnoreNullOrEmptyTokenValueContainer IgnoreNullOrEmptyTokenValues(this ITokenValueContainer This) {
            return new IgnoreNullOrEmptyTokenValueContainer(This);
        }

    }
}
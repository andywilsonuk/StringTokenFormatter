using System;
using System.Collections.Generic;

namespace StringTokenFormatter {
    public static class TokenValueContainer {
        public static ObjectPropertiesTokenValueContainer<T> FromObject<T>(T values, ITokenParser parser = default) {
            return ObjectPropertiesTokenValueContainerFactory.Create(values, parser);
        }

        public static ITokenValueContainer FromObject(object values, ITokenParser parser = default) {
            return ObjectPropertiesTokenValueContainerFactory.Create(values, parser);
        }

        public static SingleTokenValueContainer<T> FromValue<T>(string name, T value, ITokenParser parser = default) {
            return new SingleTokenValueContainer<T>(name, value, parser);
        }

        public static FuncTokenValueContainer<T> FromFunc<T>(Func<string, ITokenParser, T> values, ITokenParser parser = default) {
            return new FuncTokenValueContainer<T>(values, parser);
        }

        public static FuncTokenValueContainer<T> FromFunc<T>(Func<string, T> values, ITokenParser parser = default) {
            return new FuncTokenValueContainer<T>(values, parser);
        }

        public static DictionaryTokenValueContainer<T> FromDictionary<T>(IEnumerable<KeyValuePair<string, T>> values, ITokenParser parser = default) {
            return new DictionaryTokenValueContainer<T>(values, parser);
        }

        public static CompositeTokenValueContainer Combine(IEnumerable<ITokenValueContainer> containers) {
            return new CompositeTokenValueContainer(containers);
        }

        public static CompositeTokenValueContainer Combine(params ITokenValueContainer[] containers) {
            return new CompositeTokenValueContainer(containers);
        }
    }
}
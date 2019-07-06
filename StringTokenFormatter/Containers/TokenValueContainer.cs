using System;
using System.Collections.Generic;

namespace StringTokenFormatter {
    public static class TokenValueContainer {
        public static ObjectPropertiesTokenValueContainer<T> FromObject<T>(T Values, ITokenParser Parser = default) {
            return new ObjectPropertiesTokenValueContainer<T>(Values, Parser);
        }

        public static SingleTokenValueContainer<T> FromValue<T>(string Name, T Value, ITokenParser Parser = default) {
            return new SingleTokenValueContainer<T>(Name, Value, Parser);
        }

        public static FuncTokenValueContainer<T> FromFunc<T>(Func<string, ITokenParser, T> Values, ITokenParser Parser = default) {
            return new FuncTokenValueContainer<T>(Values, Parser);
        }

        public static FuncTokenValueContainer<T> FromFunc<T>(Func<string, T> Values, ITokenParser Parser = default) {
            return new FuncTokenValueContainer<T>(Values, Parser);
        }

        public static DictionaryTokenValueContainer<T> FromDictionary<T>(IEnumerable<KeyValuePair<string, T>> Values, ITokenParser Parser = default) {
            return new DictionaryTokenValueContainer<T>(Values, Parser);
        }

        public static CompositeTokenValueContainer Combine(IEnumerable<ITokenValueContainer> Containers) {
            return new CompositeTokenValueContainer(Containers);
        }

        public static CompositeTokenValueContainer Combine(params ITokenValueContainer[] Containers) {
            return new CompositeTokenValueContainer(Containers);
        }
    }
}
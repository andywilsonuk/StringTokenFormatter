using System;
using System.Collections.Generic;

namespace StringTokenFormatter.Impl.TokenValueContainers {

    /// <summary>
    /// This Value Container resolves values by looking them up in a dictionary.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class DictionaryTokenValueContainerImpl<T> : ITokenValueContainer {
        protected readonly IDictionary<string, T> dictionary;

        public DictionaryTokenValueContainerImpl(IEnumerable<KeyValuePair<string, T>> itemSource, ITokenNameComparer nameComparer) {
            itemSource = itemSource ?? throw new ArgumentNullException(nameof(itemSource));

            dictionary = NormalizeDictionary(itemSource, nameComparer);
        }

        private IDictionary<string, T> NormalizeDictionary(IEnumerable<KeyValuePair<string, T>> values, ITokenNameComparer nameComparer) {
            var ret = new Dictionary<string, T>(nameComparer);

            foreach (var pair in values) {
                var key = pair.Key;
                var value = pair.Value;

                //We do this instead of the add so that if something funky happens (ie. Two properties with the same name) we don't error.
                ret[key] = pair.Value;
            }
            return ret;
        }

        public TryGetResult TryMap(ITokenMatch matchedToken) {
            var ret = default(TryGetResult);
            if(dictionary.TryGetValue(matchedToken.Token, out var value)) {
                ret = TryGetResult.Success(value);
            }

            return ret;
        }
    }
}

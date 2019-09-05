using System;
using System.Collections.Generic;

namespace StringTokenFormatter {

    /// <summary>
    /// This Value Container resolves values by looking them up in a dictionary.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DictionaryTokenValueContainer<T> : ITokenValueContainer {
        private IDictionary<string, T> dictionary;

        public DictionaryTokenValueContainer(IEnumerable<KeyValuePair<string, T>> itemSource, ITokenParser parser = default) {
            itemSource = itemSource ?? throw new ArgumentNullException(nameof(itemSource));

            dictionary = NormalizeDictionary(itemSource, parser ?? TokenParser.Default);
        }

        private IDictionary<string, T> NormalizeDictionary(IEnumerable<KeyValuePair<string, T>> values, ITokenParser parser) {
            var ret = new Dictionary<string, T>(parser.TokenNameComparer);

            foreach (var pair in values) {
                string key = pair.Key;
                if (string.IsNullOrEmpty(key)) continue;

                key = parser.RemoveTokenMarkers(key);

                //We do this instead of the add so that if something funky happens (ie. Two properties with the same name) we don't error.
                ret[key] = pair.Value;
            }
            return ret;
        }

        public bool TryMap(IMatchedToken matchedToken, out object mapped) {
            if (!dictionary.TryGetValue(matchedToken.Token, out var value)) {
                mapped = null;
                return false;
            }
            mapped = value;
            return true;
        }
    }
}

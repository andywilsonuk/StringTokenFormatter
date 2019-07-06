using System;
using System.Collections.Generic;

namespace StringTokenFormatter {

    /// <summary>
    /// This Value Container resolves values by looking them up in a dictionary.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DictionaryTokenValueContainer<T> : ITokenValueContainer {
        private IDictionary<string, T> dictionary;

        public DictionaryTokenValueContainer(IEnumerable<KeyValuePair<string, T>> ItemSource, ITokenParser Parser = default) {
            ItemSource = ItemSource ?? throw new ArgumentNullException(nameof(ItemSource));

            dictionary = NormalizeDictionary(ItemSource, Parser ?? TokenParser.Default);
        }

        private IDictionary<string, T> NormalizeDictionary(IEnumerable<KeyValuePair<string, T>> Values, ITokenParser parser) {
            var ret = new Dictionary<string, T>(parser.TokenNameComparer);

            foreach (var pair in Values) {
                string key = pair.Key;
                if (string.IsNullOrEmpty(key)) continue;

                key = parser.RemoveTokenMarkers(key);
                ret.Add(key, pair.Value);
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

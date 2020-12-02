using System;
using System.Collections.Generic;

namespace StringTokenFormatter {

    /// <summary>
    /// This Value Container resolves values by looking them up in a dictionary.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DictionaryTokenValueContainer<T> : ITokenValueContainer {
        protected readonly IDictionary<string, T> dictionary;

        public DictionaryTokenValueContainer(IEnumerable<KeyValuePair<string, T>> itemSource, ITokenNameComparer? nameComparer = default, ITokenParser? parser = default ) {
            itemSource = itemSource ?? throw new ArgumentNullException(nameof(itemSource));

            dictionary = NormalizeDictionary(itemSource, nameComparer ?? TokenNameComparer.Default, parser ?? TokenParser.Default);
        }

        private IDictionary<string, T> NormalizeDictionary(IEnumerable<KeyValuePair<string, T>> values, ITokenNameComparer nameComparer, ITokenParser parser) {
            var ret = new Dictionary<string, T>(nameComparer.Comparer);

            foreach (var pair in values) {
                var key = pair.Key;
                if (string.IsNullOrEmpty(key)) continue;

                key = parser.RemoveTokenMarkers(key);
                if (string.IsNullOrEmpty(key)) continue;

                //We do this instead of the add so that if something funky happens (ie. Two properties with the same name) we don't error.
                ret[key] = pair.Value;
            }
            return ret;
        }

        public virtual bool TryMap(IMatchedToken matchedToken, out object? mapped) {
            if (!dictionary.TryGetValue(matchedToken.Token, out var value)) {
                mapped = null;
                return false;
            }
            mapped = value;
            return true;
        }
    }
}

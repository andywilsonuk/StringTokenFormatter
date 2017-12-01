using System;
using System.Collections.Generic;

namespace StringTokenFormatter
{
    public class DictionaryTokenValueContainer : ITokenValueContainer
    {
        private IDictionary<string, object> dictionary;
        private readonly ITokenMatcher matcher;

        public DictionaryTokenValueContainer(IDictionary<string, object> tokenValueDictionary, ITokenMatcher tokenMatcher)
        {
            dictionary = tokenValueDictionary ?? throw new ArgumentNullException(nameof(tokenValueDictionary));
            matcher = tokenMatcher ?? throw new ArgumentNullException(nameof(tokenMatcher));
            NormalisedTokenValues();
        }

        private void NormalisedTokenValues()
        {
            var normalisedTokens = new Dictionary<string, object>(dictionary.Count, matcher.TokenNameComparer);
            foreach (var pair in dictionary)
            {
                string key = pair.Key;
                if (string.IsNullOrEmpty(key)) continue;

                key = matcher.RemoveTokenMarkers(key);
                normalisedTokens.Add(key, pair.Value);
            }
            dictionary = normalisedTokens;
        }

        public bool TryMap(IMatchedToken matchedToken, out object mapped)
        {
            if (!dictionary.TryGetValue(matchedToken.Token, out object value))
            {
                mapped = null;
                return false;
            }
            mapped = value;
            return true;
        }
    }
}

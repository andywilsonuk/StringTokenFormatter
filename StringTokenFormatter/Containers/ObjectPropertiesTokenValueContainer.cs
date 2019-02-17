using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace StringTokenFormatter
{

    public class ObjectPropertiesTokenValueContainer : ITokenValueContainer
    {
        private IDictionary<string, Lazy<object>> dictionary;
        private readonly ITokenMatcher matcher;

        public ObjectPropertiesTokenValueContainer(object tokenValueObject, ITokenMatcher tokenMatcher)
        {
            if (tokenValueObject == null) throw new ArgumentNullException(nameof(tokenValueObject));
            matcher = tokenMatcher ?? throw new ArgumentNullException(nameof(tokenMatcher));
            dictionary = ConvertObjectToDictionary(tokenValueObject);
        }

        private IDictionary<string, Lazy<object>> ConvertObjectToDictionary(object values)
        {
            var mappings = new Dictionary<string, Lazy<object>>(matcher.TokenNameComparer);

            foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(values)) {
                mappings[descriptor.Name] = new Lazy<Object>(() => descriptor.GetValue(values), false);
            }

            return mappings;
        }

        public bool TryMap(IMatchedToken matchedToken, out object mapped)
        {
            var ret = false;
            mapped = null;

            if (dictionary.TryGetValue(matchedToken.Token, out var lazy)) {
                mapped = lazy.Value;
                ret = true;
            }

            return ret;
        }
    }
}

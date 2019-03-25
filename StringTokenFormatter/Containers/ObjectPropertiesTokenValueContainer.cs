using System;
using System.Collections.Generic;
using System.ComponentModel;

using System.Threading;

namespace StringTokenFormatter
{

    public class ObjectPropertiesTokenValueContainer : ITokenValueContainer
    {
        private IDictionary<string, Lazy<object>> dictionary;
        private readonly ITokenMatcher matcher;


        public ObjectPropertiesTokenValueContainer(object tokenValueObject, ITokenMatcher tokenMatcher) 
            : this(tokenValueObject, tokenMatcher, LazyThreadSafetyMode.PublicationOnly) 
        {
            //Do nothing
        }

        public ObjectPropertiesTokenValueContainer(object tokenValueObject, ITokenMatcher tokenMatcher, LazyThreadSafetyMode threadSafetyMode)
        {
            if (tokenValueObject == null) throw new ArgumentNullException(nameof(tokenValueObject));
            matcher = tokenMatcher ?? throw new ArgumentNullException(nameof(tokenMatcher));
            dictionary = ConvertObjectToDictionary(tokenValueObject, threadSafetyMode);
        }

        private IDictionary<string, Lazy<object>> ConvertObjectToDictionary(object values, LazyThreadSafetyMode threadSafetyMode) {
            var mappings = new Dictionary<string, Lazy<object>>(matcher.TokenNameComparer);

            foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(values))
            {
                mappings[descriptor.Name] = new Lazy<object>(() => descriptor.GetValue(values), threadSafetyMode);
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
}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace StringTokenFormatter
{
    public class ObjectPropertiesTokenValueContainer : ITokenValueContainer
    {
        private IDictionary<string, object> dictionary;
        private readonly ITokenMatcher matcher;

        public ObjectPropertiesTokenValueContainer(object tokenValueObject, ITokenMatcher tokenMatcher)
        {
            if (tokenValueObject == null) throw new ArgumentNullException(nameof(tokenValueObject));
            matcher = tokenMatcher ?? throw new ArgumentNullException(nameof(tokenMatcher));
            dictionary = ConvertObjectToDictionary(tokenValueObject);
        }

        private IDictionary<string, object> ConvertObjectToDictionary(object values)
        {
            Dictionary<string, object> mappings = new Dictionary<string, object>(matcher.TokenNameComparer);

            foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(values))
            {
                object obj2 = descriptor.GetValue(values);
                mappings.Add(descriptor.Name, obj2);
            }

            return mappings;
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

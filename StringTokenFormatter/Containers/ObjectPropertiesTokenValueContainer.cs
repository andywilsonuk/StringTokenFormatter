﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;

namespace StringTokenFormatter
{
    /// <summary>
    /// Converts all of an objects properties to a Token Value Converter.
    /// </summary>
    public class ObjectPropertiesTokenValueContainer : ITokenValueContainer
    {
        private IDictionary<string, NonLockingLazy<object>> dictionary;
        private readonly ITokenMatcher matcher;

        public ObjectPropertiesTokenValueContainer(object tokenValueObject, ITokenMatcher tokenMatcher)
        {
            if (tokenValueObject == null) throw new ArgumentNullException(nameof(tokenValueObject));
            matcher = tokenMatcher ?? throw new ArgumentNullException(nameof(tokenMatcher));
            dictionary = ConvertObjectToDictionary(tokenValueObject);
        }

        private IDictionary<string, NonLockingLazy<object>> ConvertObjectToDictionary(object values)
        {
            var mappings = new Dictionary<string, NonLockingLazy<object>>(matcher.TokenNameComparer);

            foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(values))
            {
                mappings[descriptor.Name] = new NonLockingLazy<object>(() => descriptor.GetValue(values));
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
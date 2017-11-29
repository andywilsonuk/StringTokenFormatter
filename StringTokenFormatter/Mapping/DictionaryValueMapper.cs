using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StringTokenFormatter
{
    public class DictionaryValueMapper : ITokenToValueMapper
    {
        private IDictionary<string, object> tokenValueDictionary;

        public DictionaryValueMapper(IDictionary<string, object> tokenValueDictionary)
        {
            this.tokenValueDictionary = tokenValueDictionary ?? throw new ArgumentNullException(nameof(tokenValueDictionary));

            // expand each object into an ITokenToValueMapper
            // handle token markers
        }

        public object Map(IMatchedToken matchedToken)
        {
            string token = matchedToken.Token;
            if (!tokenValueDictionary.TryGetValue(token, out object value))
            {
                return matchedToken.Original;
            }

            ExpandFunction(token);
            ExpandFunctionString(token);
            ExpandLazy(token);
            ExpandLazyString(token);
            return tokenValueDictionary[token];
        }

        private void ExpandFunction(string token)
        {
            Func<string, object> func = tokenValueDictionary[token] as Func<string, object>;
            if (func == null) return;
            tokenValueDictionary[token] = func(token);
        }

        private void ExpandFunctionString(string token)
        {
            Func<string, string> func = tokenValueDictionary[token] as Func<string, string>;
            if (func == null) return;
            tokenValueDictionary[token] = func(token);
        }

        private void ExpandLazy(string token)
        {
            Lazy<object> lazy = tokenValueDictionary[token] as Lazy<object>;
            if (lazy == null) return;
            tokenValueDictionary[token] = lazy.Value.ToString();
        }

        private void ExpandLazyString(string token)
        {
            Lazy<string> lazy = tokenValueDictionary[token] as Lazy<string>;
            if (lazy == null) return;
            tokenValueDictionary[token] = lazy.Value;
        }

    }
}

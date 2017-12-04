using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StringTokenFormatter
{
    public static class UriTokenExtensions
    {
        public static Uri FormatToken(this Uri url, string token, object value)
        {
            return new Uri(new TokenReplacer().FormatFromSingle(url.OriginalString, token, value), UriKind.RelativeOrAbsolute);
        }

        public static Uri FormatToken(this Uri url, object propertyValues)
        {
            return new Uri(new TokenReplacer().FormatFromProperties(url.OriginalString, propertyValues), UriKind.RelativeOrAbsolute);
        }

        public static Uri FormatToken(this Uri url, IDictionary<string, object> dictionaryValues)
        {
            return new Uri(new TokenReplacer().FormatFromDictionary(url.OriginalString, dictionaryValues), UriKind.RelativeOrAbsolute);
        }

        public static Uri FormatToken(this Uri url, IDictionary<string, string> dictionaryValues)
        {
            return new Uri(new TokenReplacer().FormatFromDictionary(url.OriginalString, dictionaryValues), UriKind.RelativeOrAbsolute);
        }
    }
}

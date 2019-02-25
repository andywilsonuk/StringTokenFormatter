using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StringTokenFormatter
{
    public static class UriTokenExtensions
    {
        /// <summary>
        /// Replaces each instance of the single format token in the specified Uri with the text equivalent of a corresponding object's value.
        /// </summary>
        /// <param name="url">The Uri containing the token to be replaced.</param>
        /// <param name="token">The token to be replaced.</param>
        /// <param name="replacementValue">The replacement value.</param>
        /// <returns>A copy of url in which the format tokens have been replaced by the string representation of the corresponding object's values.</returns>
        public static Uri FormatToken(this Uri url, string token, object replacementValue)
        {
            return new Uri(new TokenReplacer().FormatFromSingle(url.OriginalString, token, replacementValue), UriKind.RelativeOrAbsolute);
        }

        /// <summary>
        /// Replaces each format token in a specified Uri with the equivalent property's text equivalent value in the object.
        /// </summary>
        /// <param name="url">The Uri containing the tokens to be replaced.</param>
        /// <param name="propertyValues">The object containing the property values to be used in replacements.</param>
        /// <returns>A copy of url in which the format tokens have been replaced by the string representation of the corresponding object's values.</returns>
        public static Uri FormatToken(this Uri url, object propertyValues)
        {
            return new Uri(new TokenReplacer().FormatFromProperties(url.OriginalString, propertyValues), UriKind.RelativeOrAbsolute);
        }

        /// <summary>
        /// Replaces each format token in a specified string with the equivalent matching token object's text equivalent value.
        /// </summary>
        /// <param name="input">The Uri containing the tokens to be replaced.</param>
        /// <param name="dictionaryValues">The token keys and associated values.</param>
        /// <returns>A copy of url in which the format tokens have been replaced by the string representation of the corresponding object's values.</returns>
        public static Uri FormatToken(this Uri url, IDictionary<string, object> dictionaryValues)
        {
            return new Uri(new TokenReplacer().FormatFromDictionary(url.OriginalString, dictionaryValues), UriKind.RelativeOrAbsolute);
        }

        /// <summary>
        /// Replaces each format token in a specified string with the equivalent matching token's string value.
        /// </summary>
        /// <param name="input">The Uri containing the tokens to be replaced.</param>
        /// <param name="dictionaryValues">The token keys and associated values.</param>
        /// <returns>A copy of url in which the format tokens have been replaced by the string representation of the corresponding object's values.</returns>
        public static Uri FormatToken(this Uri url, IDictionary<string, string> dictionaryValues)
        {
            return new Uri(new TokenReplacer().FormatFromDictionary(url.OriginalString, dictionaryValues), UriKind.RelativeOrAbsolute);
        }
    }
}

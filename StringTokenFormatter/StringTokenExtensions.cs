using System;
using System.Collections.Generic;

namespace StringTokenFormatter
{
    public static class StringTokenExtensions
    {
        /// <summary>
        /// Replaces each instance of the single format token in the specified string with the text equivalent of a corresponding object's value.
        /// </summary>
        /// <param name="input">The string containing the token to be replaced.</param>
        /// <param name="token">The token to be replaced.</param>
        /// <param name="replacementValue">The replacement value.</param>
        /// <returns>A copy of input in which the format tokens have been replaced by the string representation of the corresponding object's values.</returns>
        public static string FormatToken(this string input, string token, object replacementValue)
        {
            return FormatToken(input, null, token, replacementValue);
        }

        /// <summary>
        /// Replaces each instance of the single format token in the specified string with the text equivalent of a corresponding object's value using the format provider specified.
        /// </summary>
        /// <param name="input">The string containing the token to be replaced.</param>
        /// <param name="provider">The formatting provider.</param>
        /// <param name="token">The token to be replaced.</param>
        /// <param name="replacementValue">The replacement value.</param>
        /// <returns>A copy of input in which the format tokens have been replaced by the string representation of the corresponding object's values.</returns>
        public static string FormatToken(this string input, IFormatProvider provider, string token, object replacementValue)
        {
            return new TokenReplacer(TokenReplacer.DefaultMatcher, TokenReplacer.DefaultMappers, new FormatProviderValueFormatter(provider)).FormatFromSingle(input, token, replacementValue);
        }

        /// <summary>
        /// Replaces each format token in a specified string with the equivalent matching token object's text equivalent value.
        /// </summary>
        /// <param name="input">The string containing the tokens to be replaced.</param>
        /// <param name="tokenValues">The token keys and associated values.</param>
        /// <returns>A copy of input in which the format tokens have been replaced by the string representation of the corresponding object's values.</returns>
        public static string FormatToken(this string input, IDictionary<string, object> tokenValues)
        {
            return FormatToken(input, null, tokenValues);
        }

        /// <summary>
        /// Replaces each format token in a specified string with the equivalent matching token object's text equivalent value using the format provider specified.
        /// </summary>
        /// <param name="input">The string containing the tokens to be replaced.</param>
        /// <param name="provider">The formatting provider.</param>
        /// <param name="tokenValues">The token keys and associated values.</param>
        /// <returns>A copy of input in which the format tokens have been replaced by the string representation of the corresponding object's values.</returns>
        public static string FormatToken(this string input, IFormatProvider provider, IDictionary<string, object> tokenValues)
        {
            return new TokenReplacer(TokenReplacer.DefaultMatcher, TokenReplacer.DefaultMappers, new FormatProviderValueFormatter(provider)).FormatFromDictionary(input, tokenValues);
        }

        /// <summary>
        /// Replaces each format token in a specified string with the equivalent property's text equivalent value in the object.
        /// </summary>
        /// <param name="input">The string containing the token to be replaced.</param>
        /// <param name="tokenValues">The object containing the property values to be used in replacements.</param>
        /// <returns>A copy of input in which the format tokens have been replaced by the string representation of the corresponding object's values.</returns>
        public static string FormatToken(this string input, object tokenValues)
        {
            return FormatToken(input, (IFormatProvider)null, tokenValues);
        }

        /// <summary>
        /// Replaces each format token in a specified string with the equivalent property's text equivalent value using the format provider specified.
        /// </summary>
        /// <param name="input">The string containing the token to be replaced.</param>
        /// <param name="provider">The formatting provider.</param>
        /// <param name="tokenValues">The object containing the property values to be used in replacements.</param>
        /// <returns>A copy of input in which the format tokens have been replaced by the string representation of the corresponding object's values.</returns>
        public static string FormatToken(this string input, IFormatProvider provider, object tokenValues)
        {
            return new TokenReplacer(TokenReplacer.DefaultMatcher, TokenReplacer.DefaultMappers, new FormatProviderValueFormatter(provider)).FormatFromProperties(input, tokenValues);
        }

        /// <summary>
        /// Replaces each format token in a specified string with the equivalent matching token's string value.
        /// </summary>
        /// <param name="input">The string containing the tokens to be replaced.</param>
        /// <param name="tokenValues">The token keys and associated values.</param>
        /// <returns>A copy of input in which the format tokens have been replaced by the string representation of the corresponding object's values.</returns>
        public static string FormatToken(this string input, IDictionary<string, string> tokenValues)
        {
            return FormatToken(input, null, tokenValues);
        }

        /// <summary>
        /// Replaces each format token in a specified string with the equivalent matching token's string value using the format provider specified.
        /// </summary>
        /// <param name="input">The string containing the tokens to be replaced.</param>
        /// <param name="provider">The formatting provider.</param>
        /// <param name="tokenValues">The token keys and associated values.</param>
        /// <returns>A copy of input in which the format tokens have been replaced by the string representation of the corresponding object's values.</returns>
        public static string FormatToken(this string input, IFormatProvider provider, IDictionary<string, string> tokenValues)
        {
            return new TokenReplacer(TokenReplacer.DefaultMatcher, TokenReplacer.DefaultMappers, new FormatProviderValueFormatter(provider)).FormatFromDictionary(input, tokenValues);
        }
    }
}

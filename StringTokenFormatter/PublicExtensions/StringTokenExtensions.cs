using System;
using System.Collections.Generic;

namespace StringTokenFormatter {
    public static class StringTokenExtensions
    {
        /// <summary>
        /// Replaces each token in <paramref name="Input"/> with <paramref name="Values"/>'s properties that are exposed by the type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type or interface that will limit the available properties.</typeparam>
        /// <param name="Input">The string containing the tokens to be replaced.</param>
        /// <param name="Values">The object containing the property values to be used in replacements.</param>
        /// <param name="Formatter">The format provider to use (or <see cref="null"/> for default)</param>
        /// <param name="Converter">The value converter to use (or <see cref="null"/> for default)</param>
        /// <param name="Parser">The token matcher to use (or <see cref="null"/> for default)</param>
        /// <returns>A copy of <paramref name="Input"/> in which the format tokens have been replaced by string representations of <paramref name="Values"/> properties.</returns>
        public static string FormatToken<T>(this string Input, T Values, ITokenValueFormatter Formatter = default, ITokenValueConverter Converter = default, ITokenParser Parser = default) {
            return TokenValueContainer
                .FromObject(Values, Parser)
                .FormatToken(Input, Formatter, Converter, Parser)
                ;
        }

        /// <summary>
        /// Replaces each instance of <paramref name="Token"/> in <paramref name="Input"/> with <paramref name="ReplacementValue"/>
        /// </summary>
        /// <param name="Input">The string containing the tokens to be replaced.</param>
        /// <param name="Token">The name of the token to replace</param>
        /// <param name="ReplacementValue">The value for the above token</param>
        /// <param name="Formatter">The format provider to use (or <see cref="null"/> for default)</param>
        /// <param name="Converter">The value converter to use (or <see cref="null"/> for default)</param>
        /// <param name="Parser">The token matcher to use (or <see cref="null"/> for default)</param>
        /// <returns>A copy of <paramref name="Input"/> in which <paramref name="Token"/> has been replaced with string representations of <paramref name="ReplacementValue"/>.</returns>
        public static string FormatToken(this string Input, string Token, object ReplacementValue, ITokenValueFormatter Formatter = default, ITokenValueConverter Converter = default, ITokenParser Parser = default) {
            return TokenValueContainer
                .FromValue(Token, ReplacementValue, Parser)
                .FormatToken(Input, Formatter, Converter, Parser)
                ;
        }

        /// <summary>
        /// Replaces each instance of a token in <paramref name="Input"/> by invoking <paramref name="Values"/> with the name of the token.
        /// </summary>
        /// <typeparam name="T">The type of object returned by <paramref name="Values"/></typeparam>
        /// <param name="Input">The string containing the tokens to be replaced.</param>
        /// <param name="Values">A function that will resolve token names to values</param>
        /// <param name="Formatter">The format provider to use (or <see cref="null"/> for default)</param>
        /// <param name="Converter">The value converter to use (or <see cref="null"/> for default)</param>
        /// <param name="Parser">The token matcher to use (or <see cref="null"/> for default)</param>
        /// <returns>A copy of <paramref name="Input"/> in which <paramref name="Token"/> has been replaced with string representations of the values returned by <paramref name="Values"/>.</returns>
        public static string FormatToken<T>(this string Input, Func<string, ITokenParser, T> Values, ITokenValueFormatter Formatter = default, ITokenValueConverter Converter = default, ITokenParser Parser = default) {
            return TokenValueContainer
                .FromFunc(Values, Parser)
                .FormatToken(Input, Formatter, Converter, Parser)
                ;
        }

        /// <summary>
        /// Replaces each instance of a token in <paramref name="Input"/> by invoking <paramref name="Values"/> with the name of the token.
        /// </summary>
        /// <typeparam name="T">The type of object returned by <paramref name="Values"/></typeparam>
        /// <param name="Input">The string containing the tokens to be replaced.</param>
        /// <param name="Values">A function that will resolve token names to values</param>
        /// <param name="Formatter">The format provider to use (or <see cref="null"/> for default)</param>
        /// <param name="Converter">The value converter to use (or <see cref="null"/> for default)</param>
        /// <param name="Parser">The token matcher to use (or <see cref="null"/> for default)</param>
        /// <returns>A copy of <paramref name="Input"/> in which <paramref name="Token"/> has been replaced with string representations of the values returned by <paramref name="Values"/>.</returns>
        public static string FormatToken<T>(this string Input, Func<string, T> Values, ITokenValueFormatter Formatter = default, ITokenValueConverter Converter = default, ITokenParser Parser = default) {
            return TokenValueContainer
                .FromFunc(Values, Parser)
                .FormatToken(Input, Formatter, Converter, Parser)
                ;
        }

        /// <summary>
        /// Replaces each instance of a token in <paramref name="Input"/> by looking up the value in <paramref name="Values"/>
        /// </summary>
        /// <typeparam name="T">The type of objects contained in <paramref name="Values"/></typeparam>
        /// <param name="Input">The string containing the tokens to be replaced.</param>
        /// <param name="Values">A <see cref="IDictionary{String, T}"/> containing the values to use for replacement.</param>
        /// <param name="Formatter">The format provider to use (or <see cref="null"/> for default)</param>
        /// <param name="Converter">The value converter to use (or <see cref="null"/> for default)</param>
        /// <param name="Parser">The token matcher to use (or <see cref="null"/> for default)</param>
        /// <returns>A copy of <paramref name="Input"/> in which <paramref name="Token"/> has been replaced with string representations of the values inside <paramref name="Values"/>.</returns>
        public static string FormatDictionary<T>(this string Input, IEnumerable<KeyValuePair<string, T>> Values, ITokenValueFormatter Formatter = default, ITokenValueConverter Converter = default, ITokenParser Parser = default) {
            return TokenValueContainer
                .FromDictionary(Values, Parser)
                .FormatToken(Input, Formatter, Converter, Parser)
                ;
        }

        /// <summary>
        /// Replaces each instance of a token in <paramref name="Input"/> by looking up the value in <paramref name="Values"/>
        /// </summary>
        /// <param name="Input">The string containing the tokens to be replaced.</param>
        /// <param name="Values">A <see cref="ITokenValueContainer"/> containing the values to use for replacement.</param>
        /// <param name="Formatter">The format provider to use (or <see cref="null"/> for default)</param>
        /// <param name="Converter">The value converter to use (or <see cref="null"/> for default)</param>
        /// <param name="Parser">The token matcher to use (or <see cref="null"/> for default)</param>
        /// <returns>A copy of <paramref name="Input"/> in which <paramref name="Token"/> has been replaced with string representations of the values inside <paramref name="Values"/>.</returns>
        public static string FormatContainer(this string Input, ITokenValueContainer Values, ITokenValueFormatter Formatter = default, ITokenValueConverter Converter = default, ITokenParser Parser = default) {
            return Values.FormatToken(Input, Formatter, Converter, Parser);
        }
    }

}

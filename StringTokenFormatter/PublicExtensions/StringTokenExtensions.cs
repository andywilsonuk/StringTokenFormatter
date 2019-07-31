using System;
using System.Collections.Generic;

namespace StringTokenFormatter {
    public static class StringTokenExtensions
    {
        /// <summary>
        /// Replaces each token in <paramref name="input"/> with <paramref name="values"/>'s properties that are exposed by the type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type or interface that will limit the available properties.</typeparam>
        /// <param name="input">The string containing the tokens to be replaced.</param>
        /// <param name="values">The object containing the property values to be used in replacements.</param>
        /// <param name="formatter">The format provider to use (or <see cref="null"/> for default)</param>
        /// <param name="converter">The value converter to use (or <see cref="null"/> for default)</param>
        /// <param name="parser">The token matcher to use (or <see cref="null"/> for default)</param>
        /// <returns>A copy of <paramref name="input"/> in which the format tokens have been replaced by string representations of <paramref name="values"/> properties.</returns>
        public static string FormatToken<T>(this string input, T values, ITokenValueFormatter formatter = default, ITokenValueConverter converter = default, ITokenParser parser = default) {
            return TokenValueContainer
                .FromObject(values, parser)
                .FormatToken(input, formatter, converter, parser)
                ;
        }

        /// <summary>
        /// Replaces each token in <paramref name="input"/> with <paramref name="values"/>'s properties that are exposed by the type <typeparamref name="T"/>.  The generic method is faster.
        /// </summary>
        /// <typeparam name="T">The type or interface that will limit the available properties.</typeparam>
        /// <param name="input">The string containing the tokens to be replaced.</param>
        /// <param name="values">The object containing the property values to be used in replacements.</param>
        /// <param name="formatter">The format provider to use (or <see cref="null"/> for default)</param>
        /// <param name="converter">The value converter to use (or <see cref="null"/> for default)</param>
        /// <param name="parser">The token matcher to use (or <see cref="null"/> for default)</param>
        /// <returns>A copy of <paramref name="input"/> in which the format tokens have been replaced by string representations of <paramref name="values"/> properties.</returns>
        public static string FormatToken(this string input, object values, ITokenValueFormatter formatter = default, ITokenValueConverter converter = default, ITokenParser parser = default) {
            return TokenValueContainer
                .FromObject(values, parser)
                .FormatToken(input, formatter, converter, parser)
                ;
        }

        /// <summary>
        /// Replaces each instance of <paramref name="token"/> in <paramref name="input"/> with <paramref name="replacementValue"/>
        /// </summary>
        /// <param name="input">The string containing the tokens to be replaced.</param>
        /// <param name="token">The name of the token to replace</param>
        /// <param name="replacementValue">The value for the above token</param>
        /// <param name="formatter">The format provider to use (or <see cref="null"/> for default)</param>
        /// <param name="converter">The value converter to use (or <see cref="null"/> for default)</param>
        /// <param name="parser">The token matcher to use (or <see cref="null"/> for default)</param>
        /// <returns>A copy of <paramref name="input"/> in which <paramref name="token"/> has been replaced with string representations of <paramref name="replacementValue"/>.</returns>
        public static string FormatToken(this string input, string token, object replacementValue, ITokenValueFormatter formatter = default, ITokenValueConverter converter = default, ITokenParser parser = default) {
            return TokenValueContainer
                .FromValue(token, replacementValue, parser)
                .FormatToken(input, formatter, converter, parser)
                ;
        }

        /// <summary>
        /// Replaces each instance of a token in <paramref name="input"/> by invoking <paramref name="values"/> with the name of the token.
        /// </summary>
        /// <typeparam name="T">The type of object returned by <paramref name="values"/></typeparam>
        /// <param name="input">The string containing the tokens to be replaced.</param>
        /// <param name="values">A function that will resolve token names to values</param>
        /// <param name="formatter">The format provider to use (or <see cref="null"/> for default)</param>
        /// <param name="converter">The value converter to use (or <see cref="null"/> for default)</param>
        /// <param name="parser">The token matcher to use (or <see cref="null"/> for default)</param>
        /// <returns>A copy of <paramref name="input"/> in which <paramref name="Token"/> has been replaced with string representations of the values returned by <paramref name="values"/>.</returns>
        public static string FormatToken<T>(this string input, Func<string, ITokenParser, T> values, ITokenValueFormatter formatter = default, ITokenValueConverter converter = default, ITokenParser parser = default) {
            return TokenValueContainer
                .FromFunc(values, parser)
                .FormatToken(input, formatter, converter, parser)
                ;
        }

        /// <summary>
        /// Replaces each instance of a token in <paramref name="input"/> by invoking <paramref name="values"/> with the name of the token.
        /// </summary>
        /// <typeparam name="T">The type of object returned by <paramref name="values"/></typeparam>
        /// <param name="input">The string containing the tokens to be replaced.</param>
        /// <param name="values">A function that will resolve token names to values</param>
        /// <param name="formatter">The format provider to use (or <see cref="null"/> for default)</param>
        /// <param name="converter">The value converter to use (or <see cref="null"/> for default)</param>
        /// <param name="parser">The token matcher to use (or <see cref="null"/> for default)</param>
        /// <returns>A copy of <paramref name="input"/> in which <paramref name="Token"/> has been replaced with string representations of the values returned by <paramref name="values"/>.</returns>
        public static string FormatToken<T>(this string input, Func<string, T> values, ITokenValueFormatter formatter = default, ITokenValueConverter converter = default, ITokenParser parser = default) {
            return TokenValueContainer
                .FromFunc(values, parser)
                .FormatToken(input, formatter, converter, parser)
                ;
        }

        /// <summary>
        /// Replaces each instance of a token in <paramref name="input"/> by looking up the value in <paramref name="values"/>
        /// </summary>
        /// <typeparam name="T">The type of objects contained in <paramref name="values"/></typeparam>
        /// <param name="input">The string containing the tokens to be replaced.</param>
        /// <param name="values">A <see cref="IDictionary{String, T}"/> containing the values to use for replacement.</param>
        /// <param name="formatter">The format provider to use (or <see cref="null"/> for default)</param>
        /// <param name="converter">The value converter to use (or <see cref="null"/> for default)</param>
        /// <param name="parser">The token matcher to use (or <see cref="null"/> for default)</param>
        /// <returns>A copy of <paramref name="input"/> in which <paramref name="Token"/> has been replaced with string representations of the values inside <paramref name="values"/>.</returns>
        public static string FormatDictionary<T>(this string input, IEnumerable<KeyValuePair<string, T>> values, ITokenValueFormatter formatter = default, ITokenValueConverter converter = default, ITokenParser parser = default) {
            return TokenValueContainer
                .FromDictionary(values, parser)
                .FormatToken(input, formatter, converter, parser)
                ;
        }

        /// <summary>
        /// Replaces each instance of a token in <paramref name="input"/> by looking up the value in <paramref name="values"/>
        /// </summary>
        /// <param name="input">The string containing the tokens to be replaced.</param>
        /// <param name="values">A <see cref="ITokenValueContainer"/> containing the values to use for replacement.</param>
        /// <param name="formatter">The format provider to use (or <see cref="null"/> for default)</param>
        /// <param name="converter">The value converter to use (or <see cref="null"/> for default)</param>
        /// <param name="parser">The token matcher to use (or <see cref="null"/> for default)</param>
        /// <returns>A copy of <paramref name="input"/> in which <paramref name="Token"/> has been replaced with string representations of the values inside <paramref name="values"/>.</returns>
        public static string FormatContainer(this string input, ITokenValueContainer values, ITokenValueFormatter formatter = default, ITokenValueConverter converter = default, ITokenParser parser = default) {
            return values.FormatToken(input, formatter, converter, parser);
        }
    }

}

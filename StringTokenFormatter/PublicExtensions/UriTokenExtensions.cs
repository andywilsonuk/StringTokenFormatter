using System;
using System.Collections.Generic;

namespace StringTokenFormatter {


    public static class UriTokenExtensions {

        /// <summary>
        /// Replaces each token in <paramref name="input"/> with the values that are provided.
        /// </summary>
        /// <typeparam name="T">The type or interface that will limit the available properties.</typeparam>
        /// <param name="input">The string containing the tokens to be replaced.</param>
        /// <param name="values">The object containing the property values to be used in replacements.</param>
        /// <param name="formatter">The format provider to use (or <see cref="null"/> for default)</param>
        /// <param name="converter">The value converter to use (or <see cref="null"/> for default)</param>
        /// <param name="parser">The token matcher to use (or <see cref="null"/> for default)</param>
        /// <param name="nameComparer">The token name comparer to use (or <see cref="null"/> for default</param>
        /// <returns>A copy of <paramref name="input"/> in which the format tokens have been replaced by string representations of <paramref name="values"/> properties.</returns>
        public static Uri FormatToken<T>(this Uri input, T values, ITokenValueFormatter? formatter = default, ITokenValueConverter? converter = default, ITokenParser? parser = default, ITokenNameComparer? nameComparer = default) {
            return input.OriginalString
                .FormatToken(values, formatter, converter, parser, nameComparer)
                .ToUri()
                ;
        }

        /// <inheritdoc cref="FormatToken{T}(Uri, T, ITokenValueFormatter, ITokenValueConverter, ITokenParser, ITokenNameComparer)"/>
        /// <param name="input">The string containing the tokens to be replaced.</param>
        public static Uri FormatToken(this Uri input, object values, ITokenValueFormatter? formatter = default, ITokenValueConverter? converter = default, ITokenParser? parser = default, ITokenNameComparer? nameComparer = default) {
            return input.OriginalString
                .FormatToken(values, formatter, converter, parser, nameComparer)
                .ToUri()
                ;
        }

        /// <inheritdoc cref="FormatToken{T}(Uri, T, ITokenValueFormatter, ITokenValueConverter, ITokenParser, ITokenNameComparer)"/>
        /// <param name="token">The name of the token to replace</param>
        public static Uri FormatToken(this Uri input, string token, object replacementValue, ITokenValueFormatter? formatter = default, ITokenValueConverter? converter = default, ITokenParser? parser = default, ITokenNameComparer? nameComparer = default) {
            return input.OriginalString
                .FormatToken(token, replacementValue, formatter, converter, parser, nameComparer)
                .ToUri()
                ;
        }

        /// <inheritdoc cref="FormatToken{T}(Uri, T, ITokenValueFormatter, ITokenValueConverter, ITokenParser, ITokenNameComparer)"/>
        /// <param name="values">A function that will resolve token names to values</param>
        public static Uri FormatToken<T>(this Uri input, Func<string, ITokenParser, T> values, ITokenValueFormatter? formatter = default, ITokenValueConverter? converter = default, ITokenParser? parser = default, ITokenNameComparer? nameComparer = default) {
            return input.OriginalString
                .FormatToken(values, formatter, converter, parser, nameComparer)
                .ToUri()
                ;
        }

        /// <inheritdoc cref="FormatToken{T}(Uri, T, ITokenValueFormatter, ITokenValueConverter, ITokenParser, ITokenNameComparer)"/>
        /// <param name="values">A function that will resolve token names to values</param>
        public static Uri FormatToken<T>(this Uri input, Func<string, T> values, ITokenValueFormatter? formatter = default, ITokenValueConverter? converter = default, ITokenParser? parser = default, ITokenNameComparer? nameComparer = default) {
            return input.OriginalString
                .FormatToken(values, formatter, converter, parser, nameComparer)
                .ToUri()
                ;
        }

        /// <inheritdoc cref="FormatToken{T}(Uri, T, ITokenValueFormatter, ITokenValueConverter, ITokenParser, ITokenNameComparer)"/>
        /// <param name="values">A <see cref="IDictionary{String, T}"/> containing the values to use for replacement.</param>
        public static Uri FormatDictionary<T>(this Uri input, IEnumerable<KeyValuePair<string, T>> values, ITokenValueFormatter? formatter = default, ITokenValueConverter? converter = default, ITokenParser? parser = default, ITokenNameComparer? nameComparer = default) {
            return input.OriginalString
                .FormatDictionary(values, formatter, converter, parser, nameComparer)
                .ToUri()
                ;
        }

        /// <inheritdoc cref="FormatToken{T}(Uri, T, ITokenValueFormatter, ITokenValueConverter, ITokenParser, ITokenNameComparer)"/>
        /// <param name="values">A <see cref="ITokenValueContainer"/> containing the values to use for replacement.</param>
        public static Uri FormatContainer(this Uri input, ITokenValueContainer values, ITokenValueFormatter? formatter = default, ITokenValueConverter? converter = default, ITokenParser? parser = default, ITokenNameComparer? nameComparer = default) {
            return input.OriginalString
                .FormatContainer(values, formatter, converter, parser, nameComparer)
                .ToUri()
                ;
        }

        private static Uri ToUri(this string This) {
            return new Uri(This, UriKind.RelativeOrAbsolute);
        }

    }
}

using System;
using System.Collections.Generic;

namespace StringTokenFormatter {
    public static class SegmentedStringExtensions {
        /// <summary>
        /// Replaces each token in <paramref name="input"/> with the provided values.
        /// </summary>
        /// <typeparam name="T">The type or interface that will limit the available properties.</typeparam>
        /// <param name="input">The string containing the tokens to be replaced.</param>
        /// <param name="values">The object containing the property values to be used in replacements.</param>
        /// <param name="formatter">The format provider to use (or <see cref="null"/> for default)</param>
        /// <param name="converter">The value converter to use (or <see cref="null"/> for default)</param>
        /// <param name="parser">The token matcher to use (or <see cref="null"/> for default)</param>
        /// <param name="nameComparer">The token name comparer to use (or <see cref="null"/> for default</param>
        /// <returns>A copy of <paramref name="input"/> in which the tokens have been replaced by string representations of the provided tokens.</returns>
        public static string FormatToken<T>(this SegmentedString input, T values, ITokenValueFormatter? formatter = default, ITokenValueConverter? converter = default, ITokenParser? parser = default, ITokenNameComparer? nameComparer = default) {
            return TokenValueContainer
                .FromObject(values, nameComparer)
                .FormatToken(input, formatter, converter, parser, nameComparer)
                ;
        }

        /// <inheritdoc cref="FormatToken{T}(SegmentedString, T, ITokenValueFormatter, ITokenValueConverter, ITokenParser, ITokenNameComparer)"/>
        /// <param name="values">The object containing the property values to be used in replacements.</param>
        public static string FormatToken(this SegmentedString input, object values, ITokenValueFormatter? formatter = default, ITokenValueConverter? converter = default, ITokenParser? parser = default, ITokenNameComparer? nameComparer = default) {
            return TokenValueContainer
                .FromObject(values, nameComparer)
                .FormatToken(input, formatter, converter, parser, nameComparer)
                ;
        }

        /// <inheritdoc cref="FormatToken{T}(SegmentedString, T, ITokenValueFormatter, ITokenValueConverter, ITokenParser, ITokenNameComparer)"/>
        /// <param name="token">The name of the token to replace</param>
        public static string FormatToken(this SegmentedString input, string token, object replacementValue, ITokenValueFormatter? formatter = default, ITokenValueConverter? converter = default, ITokenParser? parser = default, ITokenNameComparer? nameComparer = default) {
            return TokenValueContainer
                .FromValue(token, replacementValue, nameComparer, parser)
                .FormatToken(input, formatter, converter, parser, nameComparer)
                ;
        }

        /// <inheritdoc cref="FormatToken{T}(SegmentedString, T, ITokenValueFormatter, ITokenValueConverter, ITokenParser, ITokenNameComparer)"/>
        /// <param name="values">A function that will resolve token names to values</param>
        public static string FormatToken<T>(this SegmentedString input, Func<string, ITokenNameComparer, T> values, ITokenValueFormatter? formatter = default, ITokenValueConverter? converter = default, ITokenParser? parser = default, ITokenNameComparer? nameComparer = default) {
            return TokenValueContainer
                .FromFunc(values, nameComparer)
                .FormatToken(input, formatter, converter, parser, nameComparer)
                ;
        }

        /// <inheritdoc cref="FormatToken{T}(SegmentedString, T, ITokenValueFormatter, ITokenValueConverter, ITokenParser, ITokenNameComparer)"/>
        /// <param name="values">A function that will resolve token names to values</param>
        public static string FormatToken<T>(this SegmentedString input, Func<string, T> values, ITokenValueFormatter? formatter = default, ITokenValueConverter? converter = default, ITokenParser? parser = default, ITokenNameComparer? nameComparer = default) {
            return TokenValueContainer
                .FromFunc(values, nameComparer)
                .FormatToken(input, formatter, converter, parser, nameComparer)
                ;
        }

        /// <inheritdoc cref="FormatToken{T}(SegmentedString, T, ITokenValueFormatter, ITokenValueConverter, ITokenParser, ITokenNameComparer)"/>
        /// <param name="values">A <see cref="IDictionary{String, T}"/> containing the values to use for replacement.</param>
        public static string FormatDictionary<T>(this SegmentedString input, IEnumerable<KeyValuePair<string, T>> values, ITokenValueFormatter? formatter = default, ITokenValueConverter? converter = default, ITokenParser? parser = default, ITokenNameComparer? nameComparer = default) {
            return TokenValueContainer
                .FromDictionary(values, nameComparer, parser)
                .FormatToken(input, formatter, converter, parser, nameComparer)
                ;
        }

        /// <inheritdoc cref="FormatToken{T}(SegmentedString, T, ITokenValueFormatter, ITokenValueConverter, ITokenParser, ITokenNameComparer)"/>
        /// <param name="values">A <see cref="ITokenValueContainer"/> containing the values to use for replacement.</param>
        public static string FormatContainer(this SegmentedString input, ITokenValueContainer values, ITokenValueFormatter? formatter = default, ITokenValueConverter? converter = default, ITokenParser? parser = default, ITokenNameComparer? nameComparer = default) {
            return values.FormatToken(input, formatter, converter, parser, nameComparer);
        }
    }

}

﻿using StringTokenFormatter.Impl;

namespace StringTokenFormatter {
    public record InterpolationSettingsBuilder {
        public ITokenSyntax TokenSyntax { get; init; } = StringTokenFormatter.TokenSyntaxes.Default;

        public ITokenNameComparer TokenNameComparer { get; init; } = TokenNameComparers.Default;

        public ITokenValueConverter TokenValueConverter { get; init; } = TokenValueConverters.Default;
        public ITokenValueFormatter TokenValueFormatter { get; init; } = TokenValueFormatters.Default;

        public IInterpolationSettings Build() {

            var InterpolatedStringParser = InterpolatedStringParsers.Create(TokenSyntax);
            var TokenValueContainerFactory = TokenValueContainerFactories.Create(TokenNameComparer);

            var ret = new Impl.InterpolationSettings.InterpolationSettings(
                TokenSyntax, 
                InterpolatedStringParser, 
                TokenNameComparer, 
                TokenValueContainerFactory, 
                TokenValueConverter, 
                TokenValueFormatter
                );

            return ret;
        }

    }

}
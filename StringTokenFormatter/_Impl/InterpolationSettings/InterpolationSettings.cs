using StringTokenFormatter.Impl;
using System;

namespace StringTokenFormatter.Impl.InterpolationSettings {

    public class InterpolationSettings : IInterpolationSettings {
        
        public InterpolationSettings(
            ITokenSyntax tokenSyntax, 
            IInterpolatedStringParser interpolatedStringParser, 
            ITokenNameComparer tokenNameComparer, 
            
            ITokenValueContainerFactory tokenValueContainerFactory,
            ITokenValueConverter tokenValueConverter, 
            ITokenValueFormatter tokenValueFormatter
            ) {

            TokenSyntax = tokenSyntax;
            InterpolatedStringParser = interpolatedStringParser;
            TokenNameComparer = tokenNameComparer;

            TokenValueContainerFactory = tokenValueContainerFactory;
            TokenValueConverter = tokenValueConverter;
            TokenValueFormatter = tokenValueFormatter;
        }

        public ITokenSyntax TokenSyntax { get; }
        public IInterpolatedStringParser InterpolatedStringParser { get; }
        public ITokenNameComparer TokenNameComparer { get; }

        public ITokenValueContainerFactory TokenValueContainerFactory { get; }
        public ITokenValueConverter TokenValueConverter { get; }
        public ITokenValueFormatter TokenValueFormatter { get; }



    }

}

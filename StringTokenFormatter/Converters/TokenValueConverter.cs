using System;
using System.Collections.Generic;

namespace StringTokenFormatter {
    public static class TokenValueConverter {

        private static ITokenValueConverter __Default = new CompositeTokenValueConverter(new ITokenValueConverter[] {
            FromNull(),
            FromPrimitives(),

            FromLazy<string>(),
            FromLazy<object>(),

            FromFunc<string>(),
            FromFunc<object>(),

            FromTokenFunc<string>(),
            FromTokenFunc<object>(),
        });

        public static ITokenValueConverter Default {
            get => __Default;
            set => __Default = value ?? throw new ArgumentNullException(nameof(value));
        }

        public static NullTokenValueConverter FromNull() => 
            NullTokenValueConverter.Instance
            ;

        public static PrimitiveTokenValueConverter FromPrimitives() => 
            PrimitiveTokenValueConverter.Instance
            ;

        public static LazyTokenValueConverter<T> FromLazy<T>() => 
            LazyTokenValueConverter<T>.Instance
            ;

        public static FuncTokenValueConverter<T> FromFunc<T>() => 
            FuncTokenValueConverter<T>.Instance
            ;

        public static TokenNameFuncTokenValueConverter<T> FromTokenFunc<T>() => 
            TokenNameFuncTokenValueConverter<T>.Instance
            ;

        public static CompositeTokenValueConverter Combine(IEnumerable<ITokenValueConverter> Converters) => 
            new CompositeTokenValueConverter(Converters)
            ;

        public static CompositeTokenValueConverter Combine(params ITokenValueConverter[] Converters) => 
            new CompositeTokenValueConverter(Converters)
            ;

    }

}

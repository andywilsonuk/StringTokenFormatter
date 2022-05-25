using StringTokenFormatter.Impl;
using StringTokenFormatter.Impl.TokenValueConverters;
using System;
using System.Collections.Generic;

namespace StringTokenFormatter {
    public static class TokenValueConverters {

        public static ITokenValueConverter Default { get; }
        
        public static ITokenValueConverter FromNull() {
            return NullTokenValueConverter.Instance;
        }

        public static ITokenValueConverter FromPrimitives() {
            return PrimitiveTokenValueConverter.Instance;
        }

        public static ITokenValueConverter FromLazy<T>() {
            return LazyTokenValueConverter<T>.Instance;
        }

        public static ITokenValueConverter FromFunc<T>() {
            return FuncTokenValueConverter<T>.Instance;
        }

        public static ITokenValueConverter FromTokenFunc<T>() {
            return TokenNameFuncTokenValueConverter<T>.Instance;
        }

        public static ITokenValueConverter Combine(IEnumerable<ITokenValueConverter> Converters) {
            return new CompositeTokenValueConverter(Converters);
        }

        public static ITokenValueConverter Combine(params ITokenValueConverter[] Converters) {
            return new CompositeTokenValueConverter(Converters);
        }

        static TokenValueConverters() {
            Default = Combine(
                FromNull(),
                FromPrimitives(),

                FromLazy<string>(),
                FromLazy<object>(),

                FromFunc<string>(),
                FromFunc<object>(),

                FromTokenFunc<string>(),
                FromTokenFunc<object>()
            );
        }



    }

}

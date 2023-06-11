using StringTokenFormatter.Impl.TokenValueConverters;

namespace StringTokenFormatter
{
    public static class TokenValueConverters {

        public static ITokenValueConverter Default { get; }
        
        public static ITokenValueConverter FromNull() {
            return NullTokenValueConverterImpl.Instance;
        }

        public static ITokenValueConverter FromPrimitives() {
            return PrimitiveTokenValueConverterImpl.Instance;
        }

        public static ITokenValueConverter FromLazy<T>() {
            return LazyTokenValueConverterImpl<T>.Instance;
        }

        public static ITokenValueConverter FromFunc<T>() {
            return FuncTokenValueConverterImpl<T>.Instance;
        }

        public static ITokenValueConverter FromTokenFunc<T>() {
            return TokenNameFuncTokenValueConverterImpl<T>.Instance;
        }

        public static ITokenValueConverter Combine(IEnumerable<ITokenValueConverter> Converters) {
            return new CompositeTokenValueConverterImpl(Converters);
        }

        public static ITokenValueConverter Combine(params ITokenValueConverter[] Converters) {
            return new CompositeTokenValueConverterImpl(Converters);
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

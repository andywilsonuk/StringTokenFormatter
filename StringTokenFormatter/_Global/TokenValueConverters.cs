using StringTokenFormatter.Impl.TokenValueConverters;

namespace StringTokenFormatter;

public static class TokenValueConverters {

    public static ITokenValueConverter Default { get; }

    public static ITokenValueConverter FromNull() => NullTokenValueConverterImpl.Instance;

    public static ITokenValueConverter FromPrimitives() => PrimitiveTokenValueConverterImpl.Instance;

    public static ITokenValueConverter FromLazy<T>() => LazyTokenValueConverterImpl<T>.Instance;

    public static ITokenValueConverter FromFunc<T>() => FuncTokenValueConverterImpl<T>.Instance;

    public static ITokenValueConverter FromTokenFunc<T>() => TokenNameFuncTokenValueConverterImpl<T>.Instance;

    public static ITokenValueConverter Combine(IEnumerable<ITokenValueConverter> Converters) => new CompositeTokenValueConverterImpl(Converters);

    public static ITokenValueConverter Combine(params ITokenValueConverter[] Converters) => new CompositeTokenValueConverterImpl(Converters);

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

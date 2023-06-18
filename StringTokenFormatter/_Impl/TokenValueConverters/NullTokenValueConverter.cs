namespace StringTokenFormatter.Impl.TokenValueConverters; 

/// <summary>
/// A short-circuit value converter that triggers when the value is null.
/// </summary>
internal sealed class NullTokenValueConverterImpl : ITokenValueConverter {
    public bool TryConvert(ITokenMatch token, object? value, out object? mapped) {
        mapped = null;
        return value == null;
    }

    private NullTokenValueConverterImpl() { }

    public static NullTokenValueConverterImpl Instance { get; } = new NullTokenValueConverterImpl();

}

namespace StringTokenFormatter.Impl.TokenValueConverters;

/// <summary>
///  Loops through all child converters until it finds one that applies to the current value.
/// </summary>
internal sealed class CompositeTokenValueConverterImpl : ITokenValueConverter {
    private readonly IEnumerable<ITokenValueConverter> converters;

    public CompositeTokenValueConverterImpl(IEnumerable<ITokenValueConverter> converters) {
        this.converters = converters ?? throw new ArgumentNullException(nameof(converters));
    }

    public bool TryConvert(ITokenMatch matchedToken, object? value, out object? mapped) {
        foreach (var converter in converters) {
            if (converter.TryConvert(matchedToken, value, out mapped)) return true;
        }
        mapped = null;
        return false;
    }
}

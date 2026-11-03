namespace StringTokenFormatter;

/// <summary>
/// Provides token value converter factories for custom value conversion.
/// </summary>
public static class TokenValueConverterExtras
{
    /// <summary>
    /// Creates a converter that uses <see cref="object.ToString"/> for values of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type whose values are converted to strings.</typeparam>
    /// <returns>A converter for values of type <typeparamref name="T"/>.</returns>
    public static TokenValueConverter ToStringConverter<T>() => TokenValueConverterFactory.ToStringConverter<T>();
}

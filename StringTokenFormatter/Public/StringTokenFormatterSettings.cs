namespace StringTokenFormatter;

public enum TokenResolutionPolicy
{
    /// <summary>
    /// Null or empty string are valid container returns.
    /// </summary>
    ResolveAll = 0,
    /// <summary>
    /// Null is invalid but empty string are valid container returns.
    /// </summary>
    IgnoreNull = 1,
    /// <summary>
    /// Null and empty string are invalid container returns.
    /// </summary>
    IgnoreNullOrEmpty = 2,
}
public enum UnresolvedTokenBehavior
{
    /// <summary>
    /// Unmatched tokens or invalid values (based on the policy) result in an `UnresolvedTokenException`.
    /// </summary>
    Throw = 0,
    /// <summary>
    /// Unmatched tokens or invalid values (based on the policy) result in the original token being output.
    /// </summary>
    LeaveUnresolved = 1,
}
public enum InvalidFormatBehavior
{
    /// <summary>
    /// `FormatException` errors are rethrown.
    /// </summary>
    Throw = 0,
    /// <summary>
    /// Formatting issues result in the token value being used.
    /// </summary>
    LeaveUnformatted = 1,
    /// <summary>
    /// Formatting issues result in the token marker being used.
    /// </summary>
    LeaveToken = 2,
}
public interface IInterpolatedStringSettings
{
    /// <summary>
    /// Gets the markers used for matching tokens. Default: CommonTokenSyntax.Curly
    /// </summary>
    public TokenSyntax Syntax { get; }
    /// <summary>
    /// Gets the behavior to use when a token cannot be found in the container. Default: `UnresolvedTokenBehavior.Throw`
    /// </summary>
    public UnresolvedTokenBehavior UnresolvedTokenBehavior { get; }
    /// <summary>
    /// Gets the collection of Value Converters.
    /// </summary>
    public IReadOnlyCollection<TokenValueConverter> ValueConverters { get; }
    /// <summary>
    /// Gets the culture format. Default: `CultureInfo.CurrentUICulture`
    /// </summary>
    public IFormatProvider FormatProvider { get; }
    /// <summary>
    /// Gets the behavior to use when `FormatException` errors are thrown. Default: `InvalidFormatBehavior.Throw`
    /// </summary>
    public InvalidFormatBehavior InvalidFormatBehavior { get; }
    /// <summary>
    /// Token prefix for starting conditional block. Default: `if:`
    /// </summary>
    public string ConditionStartToken { get; }
    /// <summary>
    /// Token prefix for ending conditional block. Default: `ifend:`
    /// </summary>
    public string ConditionEndToken { get; }
}
public interface ITokenValueContainerSettings
{
    /// <summary>
    /// Gets the comparer for matching token names. Default: `StringComparer.OrdinalIgnoreCase`
    /// </summary>
    public StringComparer NameComparer { get; }
    /// <summary>
    /// Gets the policy to use when container values are null or empty string. Default: `TokenResolutionPolicy.ResolveAll`
    /// </summary>
    public TokenResolutionPolicy TokenResolutionPolicy { get; }
}
public record StringTokenFormatterSettings : ITokenValueContainerSettings, IInterpolatedStringSettings
{
    public static StringTokenFormatterSettings Global { get; set; } = new();

    public StringComparer NameComparer { get; init; } = StringComparer.OrdinalIgnoreCase;
    public TokenResolutionPolicy TokenResolutionPolicy { get; init; } = TokenResolutionPolicy.ResolveAll;

    public TokenSyntax Syntax { get; init; } = CommonTokenSyntax.Curly;
    public UnresolvedTokenBehavior UnresolvedTokenBehavior { get; init; } = UnresolvedTokenBehavior.Throw;
    public IReadOnlyCollection<TokenValueConverter> ValueConverters {
        get { return valueConverters ?? defaultValueConverters; }
        init { valueConverters = value; }
    }
    public IFormatProvider FormatProvider { get; init; } = CultureInfo.CurrentUICulture;
    public InvalidFormatBehavior InvalidFormatBehavior { get; init; } = InvalidFormatBehavior.Throw;
    public string ConditionStartToken { get; init; } = "if:";
    public string ConditionEndToken { get; init; } = "ifend:";

    private IReadOnlyCollection<TokenValueConverter> valueConverters = defaultValueConverters;

    private static readonly IReadOnlyCollection<TokenValueConverter> defaultValueConverters = new List<TokenValueConverter>
    {
        TokenValueConverters.FromNull(),
        TokenValueConverters.FromPrimitives(),
        TokenValueConverters.FromLazy<string>(),
        TokenValueConverters.FromLazy<object>(),
        TokenValueConverters.FromFunc<string>(),
        TokenValueConverters.FromFunc<object>(),
        TokenValueConverters.FromTokenFunc<string>(),
        TokenValueConverters.FromTokenFunc<object>()
    }.AsReadOnly();
}
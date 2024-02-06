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
    /// Gets the collection of Block Commands.
    /// </summary>
    public IReadOnlyCollection<IBlockCommand> BlockCommands { get; }
    /// <summary>
    /// Gets the comparer for matching token names. Default: `StringComparer.OrdinalIgnoreCase`
    /// </summary>
    public StringComparer NameComparer { get; }
    /// <summary>
    /// Gets the collection of Block Commands.
    /// </summary>
    public IReadOnlyCollection<FormatterDefinition> FormatterDefinitions { get; }
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
public interface IHierarchicalTokenValueContainerSettings : ITokenValueContainerSettings
{
    /// <summary>
    /// Gets the delimiter when splitting hierarchical token names. Default: `.`
    /// </summary>
    public string HierarchicalDelimiter { get; }
}
public record StringTokenFormatterSettings : ITokenValueContainerSettings, IInterpolatedStringSettings, IHierarchicalTokenValueContainerSettings
{
    /// <summary>
    /// Initial settings from which custom settings can be derived.
    /// </summary>
    public static StringTokenFormatterSettings Default { get; } = new();
    /// <summary>
    /// Used when settings are not explicitly passed to StringTokenFormatter methods.
    /// </summary>
    public static StringTokenFormatterSettings Global { get; set; } = Default;

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
    public IReadOnlyCollection<IBlockCommand> BlockCommands {
        get { return blockCommands ?? defaultBlockCommands; }
        init { blockCommands = value; }
    }

    public string HierarchicalDelimiter { get; init; } = ".";

    public IReadOnlyCollection<FormatterDefinition> FormatterDefinitions {
        get { return formatterDefinitions ?? defaultFormatterDefinitions; }
        init { formatterDefinitions = value; }
    }

    private IReadOnlyCollection<TokenValueConverter> valueConverters = defaultValueConverters;
    private static readonly IReadOnlyCollection<TokenValueConverter> defaultValueConverters = new List<TokenValueConverter>
    {
        TokenValueConverterFactory.NullConverter(),
        TokenValueConverterFactory.PrimitiveConverter(),
        TokenValueConverterFactory.LazyConverter<string>(),
        TokenValueConverterFactory.LazyConverter<object>(),
        TokenValueConverterFactory.FuncConverter<string>(),
        TokenValueConverterFactory.FuncConverter<object>(),
        TokenValueConverterFactory.TokenFuncConverter<string>(),
        TokenValueConverterFactory.TokenFuncConverter<object>()
    }.AsReadOnly();

    private IReadOnlyCollection<IBlockCommand> blockCommands = defaultBlockCommands;
    private static readonly IReadOnlyCollection<IBlockCommand> defaultBlockCommands = new List<IBlockCommand>
    {
        BlockCommandFactory.Conditional,
        BlockCommandFactory.Loop,
    }.AsReadOnly();

    private static readonly IReadOnlyCollection<FormatterDefinition> defaultFormatterDefinitions = Array.Empty<FormatterDefinition>();
    private IReadOnlyCollection<FormatterDefinition> formatterDefinitions = defaultFormatterDefinitions;
}
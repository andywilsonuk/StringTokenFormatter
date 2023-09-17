using StringTokenFormatter.Impl;
using System.Globalization;

namespace StringTokenFormatter;

public enum TokenResolutionPolicy
{
    ResolveAll = 0,
    IgnoreNull = 1,
    IgnoreNullOrEmpty = 2,
}
public enum UnresolvedTokenBehavior
{
    Throw = 0,
    LeaveUnresolved = 1,
}
public interface IIInterpolatedStringSettings
{
    public TokenSyntax Syntax { get; }
    public UnresolvedTokenBehavior UnresolvedTokenBehavior { get; }
    public IReadOnlyCollection<Func<TokenValuePair, TryGetResult>> ValueConverters { get; }
    public IFormatProvider FormatProvider { get; }
}
public interface ITokenValueContainerSettings
{
    public StringComparer NameComparer { get; }
    public TokenResolutionPolicy TokenResolutionPolicy { get; }
}
public record StringTokenFormatterSettings : ITokenValueContainerSettings, IIInterpolatedStringSettings
{
    public static StringTokenFormatterSettings Global { get; set; } = new();

    public StringComparer NameComparer { get; init; } = StringComparer.OrdinalIgnoreCase;
    public TokenResolutionPolicy TokenResolutionPolicy { get; init; } = TokenResolutionPolicy.ResolveAll;

    public TokenSyntax Syntax { get; init; } = CommonTokenSyntax.Curly;
    public UnresolvedTokenBehavior UnresolvedTokenBehavior { get; init; } = UnresolvedTokenBehavior.Throw;
    public IReadOnlyCollection<Func<TokenValuePair, TryGetResult>> ValueConverters { get; init; } = new List<Func<TokenValuePair, TryGetResult>>
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
    public IFormatProvider FormatProvider { get; init; } = CultureInfo.CurrentUICulture;
}
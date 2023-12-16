#if NET8_0_OR_GREATER
using System.Collections.Frozen;
using System.Text.RegularExpressions;

namespace StringTokenFormatter.Impl;

public static partial class CommonTokenSyntaxRegexStore
{
    [GeneratedRegex(@"(\{\{)|(\{.*?\})", RegexOptions.Singleline | RegexOptions.CultureInvariant)]
    private static partial Regex GetCurlyRegex();

    [GeneratedRegex(@"(\$\{\{)|(\$\{.*?\})", RegexOptions.Singleline | RegexOptions.CultureInvariant)]
    private static partial Regex GetDollarCurlyRegex();

    [GeneratedRegex(@"(\(\()|(\(.*?\))", RegexOptions.Singleline | RegexOptions.CultureInvariant)]
    private static partial Regex GetRoundRegex();

    [GeneratedRegex(@"(\$\(\()|(\(.*?\))", RegexOptions.Singleline | RegexOptions.CultureInvariant)]
    private static partial Regex GetDollarRoundRegex();

    [GeneratedRegex(@"(\$\$\()|(\$\(.*?\))", RegexOptions.Singleline | RegexOptions.CultureInvariant)]
    private static partial Regex GetDollarRoundAlternativeRegex();

    private static FrozenDictionary<TokenSyntax, Regex> store = new Dictionary<TokenSyntax, Regex>()
    {
        [CommonTokenSyntax.Curly] = GetCurlyRegex(),
        [CommonTokenSyntax.DollarCurly] = GetDollarCurlyRegex(),
        [CommonTokenSyntax.Round] = GetRoundRegex(),
        [CommonTokenSyntax.DollarRound] = GetDollarRoundRegex(),
        [CommonTokenSyntax.DollarRoundAlternative] = GetDollarRoundAlternativeRegex(),
    }.ToFrozenDictionary();

    private static LinkedList<(TokenSyntax Syntax, Regex Regex)> customStore = new();
    public static void AddCustom(TokenSyntax syntax, Regex regex) => customStore.AddLast((syntax, regex));

    public static Regex? GetRegex(TokenSyntax syntax)
    {
        if (store.TryGetValue(syntax, out Regex? regex)) { return regex; }
        var c = customStore.FirstOrDefault(x => x.Syntax == syntax);
        if (c == default) { return null; }
        return c.Regex;
    }
}
#endif
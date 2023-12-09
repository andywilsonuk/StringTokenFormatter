using System.Text.RegularExpressions;

namespace StringTokenFormatter.Impl;

public static partial class CommonTokenSyntaxRegexStore
{
#if NET7_0_OR_GREATER
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

    private static Dictionary<TokenSyntax, Regex> store = new()
    {
        [CommonTokenSyntax.Curly] = GetCurlyRegex(),
        [CommonTokenSyntax.DollarCurly] = GetDollarCurlyRegex(),
        [CommonTokenSyntax.Round] = GetRoundRegex(),
        [CommonTokenSyntax.DollarRound] = GetDollarRoundRegex(),
        [CommonTokenSyntax.DollarRoundAlternative] = GetDollarRoundAlternativeRegex(),
    };

    public static Regex? GetRegex(TokenSyntax syntax) => store.TryGetValue(syntax, out Regex? regex) ? regex : null;
    public static void AddCustom(TokenSyntax syntax, Regex regex) => store.Add(syntax, regex);
# endif
}
namespace StringTokenFormatter.Impl;

public sealed class ExpanderValueFormatter
{
    private readonly StringComparer tokenNameComparer;
    private readonly Dictionary<int, FormatterDefinition> definitions;

    internal ExpanderValueFormatter(IEnumerable<FormatterDefinition> definitions, StringComparer tokenNameComparer)
    {
        this.tokenNameComparer = Guard.NotNull(tokenNameComparer, nameof(tokenNameComparer));
        this.definitions = CreateDictionary(Guard.NotNull(definitions, nameof(definitions)), tokenNameComparer);
    }

    private static Dictionary<int, FormatterDefinition> CreateDictionary(IEnumerable<FormatterDefinition> source, StringComparer tokenNameComparer)
    {
        var d = new Dictionary<int, FormatterDefinition>();
        foreach (var definition in source)
        {
            int hashCode = FormatterDefinition.GenerateHashCode(tokenNameComparer, definition);
            if (d.ContainsKey(hashCode)) { throw new TokenContainerException($"A definition with the same properties already exists, duplicate is: {definition}"); }
            d.Add(hashCode, definition);
        }
        return d;
    }

    public bool TryFormat(object value, string tokenName, string formatString, out string formattedValue)
    {
        var valueType = value.GetType();

        int tokenNameAndFormatStringHash = FormatterDefinition.GenerateHashCode(tokenNameComparer, valueType, tokenName, formatString);
        if (TryFormat(tokenNameAndFormatStringHash, value, formatString, out formattedValue)) { return true; }

        int tokenNameHash = FormatterDefinition.GenerateHashCode(tokenNameComparer, valueType, tokenName, string.Empty);
        if (TryFormat(tokenNameHash, value, formatString, out formattedValue)) { return true; }

        int formatStringHash = FormatterDefinition.GenerateHashCode(tokenNameComparer, valueType, string.Empty, formatString);
        if (TryFormat(formatStringHash, value, formatString, out formattedValue)) { return true; }

        var typeOnlyHash = FormatterDefinition.GenerateHashCode(tokenNameComparer, valueType, string.Empty, string.Empty);
        if (TryFormat(typeOnlyHash, value, formatString, out formattedValue)) { return true; }

        return false;
    }

    private bool TryFormat(int tryHash, object value, string formatString, out string formattedValue)
    {
        if (definitions.TryGetValue(tryHash, out FormatterDefinition? definition))
        {
            formattedValue = definition.Formatter.DynamicInvoke(value, formatString) as string ?? string.Empty;
            return true;
        }
        formattedValue = string.Empty;
        return false;
    }
}

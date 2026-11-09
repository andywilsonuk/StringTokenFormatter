using System.Reflection;
using FormatterAdapter = System.Func<object, string, string, string>;

namespace StringTokenFormatter.Impl;

public sealed class ExpanderValueFormatter
{
    private readonly StringComparer tokenNameComparer;
    private readonly StringComparer formatStringComparer = StringComparer.OrdinalIgnoreCase;
    private readonly ILookup<Type, FormatterDefinition> definitions;
    private readonly FormatterAdapterFactory formatterAdapterFactory;

    internal ExpanderValueFormatter(IEnumerable<FormatterDefinition> definitions, StringComparer tokenNameComparer)
    {
        this.tokenNameComparer = Guard.NotNull(tokenNameComparer, nameof(tokenNameComparer));
        FormatterDefinition[] definitionsArray = [.. Guard.NotNull(definitions, nameof(definitions))];
        this.definitions = definitionsArray.ToLookup(x => x.RequiredType);
        formatterAdapterFactory = FormatterAdapterFactory.Create(definitionsArray);
    }

    public bool TryFormat(object value, string tokenName, string formatString, out string formattedValue) =>
        TryFormat(value.GetType(), tokenName, formatString, value, out formattedValue);

    private bool TryFormat(Type valueType, string tokenName, string formatString, object value, out string formattedValue)
    {
        try
        {
            if (TryGetBestDefinition(valueType, tokenName, formatString, out FormatterDefinition? definition))
            {
                if (formatterAdapterFactory.TryGetAdapter(definition, out FormatterAdapter? adapter))
                {
                    formattedValue = adapter(value, formatString, tokenName) ?? string.Empty;
                }
                else
                {
                    Delegate formatter = definition.Formatter;
                    int parameterCount = formatter.GetType().GetMethod("Invoke")!.GetParameters().Length;
                    formattedValue = (parameterCount switch
                    {
                        2 => formatter.DynamicInvoke(value, formatString),
                        3 => formatter.DynamicInvoke(value, formatString, tokenName),
                        _ => throw new FormatException($"Formatter must accept two or three arguments, but accepts {parameterCount}"),
                    }) as string ?? string.Empty;
                }
                return true;
            }
            formattedValue = string.Empty;
            return false;
        }
        catch (TargetInvocationException ex)
        {
            throw new FormatException($"Formatter failed for value '{value}' with formatString '{formatString}'", ex.InnerException ?? ex);
        }
    }

    private bool TryGetBestDefinition(Type valueType, string actualTokenName, string actualFormatString, [NotNullWhen(true)] out FormatterDefinition? definition)
    {
        if (!definitions.Contains(valueType))
        {
            definition = null;
            return false;
        }
        int matchScore = int.MinValue;
        FormatterDefinition? matchDefinition = null;
        foreach (FormatterDefinition candidate in definitions[valueType])
        {
            int candidateScore = 0;
            if (candidate.RequiredTokenName == string.Empty)
            {
                candidateScore += 2;
            }
            else if (tokenNameComparer.Equals(candidate.RequiredTokenName, actualTokenName))
            {
                candidateScore += 20;
            }
            else
            {
                continue;
            }
            if (candidate.RequiredFormatString == string.Empty)
            {
                candidateScore += 1;
            }
            else if (formatStringComparer.Equals(candidate.RequiredFormatString, actualFormatString))
            {
                candidateScore += 10;
            }
            else
            {
                continue;
            }
            if (candidateScore > matchScore)
            {
                matchScore = candidateScore;
                matchDefinition = candidate;
            }
            else if (candidateScore == matchScore)
            {
                throw new FormatException($"Duplicate formatter defined '{candidate}'");
            }
        }
        definition = matchDefinition;
        return matchScore != int.MinValue;
    }
}

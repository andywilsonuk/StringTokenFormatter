namespace StringTokenFormatter.Impl;

public delegate string FormatterFunction<T>(T value, string formatString);

public sealed class FormatterDefinition
{
    private FormatterDefinition(Type requiredType, string requiredTokenName, string requiredFormatString, Delegate formatter)
    {
        RequiredType = Guard.NotNull(requiredType, nameof(requiredType));
        RequiredTokenName = Guard.NotNull(requiredTokenName, nameof(requiredTokenName));
        RequiredFormatString = Guard.NotNull(requiredFormatString, nameof(requiredFormatString));
        Formatter = Guard.NotNull(formatter, nameof(formatter));
    }

    public Type RequiredType { get; init; }
    public string RequiredTokenName { get; init; }
    public string RequiredFormatString { get; init; }
    public Delegate Formatter { get; init; }

    public static FormatterDefinition ForTypeOnly<T>(FormatterFunction<T> formatFunction) where T : notnull => new(typeof(T), string.Empty, string.Empty, formatFunction);
    public static FormatterDefinition ForTokenName<T>(string tokenName, FormatterFunction<T> formatFunction) where T : notnull => new(typeof(T), tokenName, string.Empty, formatFunction);
    public static FormatterDefinition ForFormatString<T>(string formatString, FormatterFunction<T> formatFunction) where T : notnull => new(typeof(T), string.Empty, formatString, formatFunction);
    public static FormatterDefinition ForTokenNameAndFormatString<T>(string tokenName, string formatString, FormatterFunction<T> formatFunction) where T : notnull => new(typeof(T), tokenName, formatString, formatFunction);

    public override string ToString() => $"Type:{RequiredType.Name},TokenName:{RequiredTokenName},FormatString:{RequiredFormatString}";

    public override int GetHashCode() => ToString().GetHashCode();
}
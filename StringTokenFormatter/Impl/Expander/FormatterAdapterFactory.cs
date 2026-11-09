using System.Reflection;
using System.Runtime.CompilerServices;
using FormatterAdapter = System.Func<object, string, string, string>;

namespace StringTokenFormatter.Impl;

internal sealed class FormatterAdapterFactory
{
    private static readonly MethodInfo TwoArgumentAdapterFactory =
        typeof(FormatterAdapterFactory).GetMethod(nameof(CreateTwoArgumentAdapter), BindingFlags.Static | BindingFlags.NonPublic)!;
    private static readonly MethodInfo ThreeArgumentAdapterFactory =
        typeof(FormatterAdapterFactory).GetMethod(nameof(CreateThreeArgumentAdapter), BindingFlags.Static | BindingFlags.NonPublic)!;
    private static readonly FormatterAdapterFactory Empty = new(
        new Dictionary<FormatterDefinition, FormatterAdapter>(FormatterDefinitionReferenceComparer.Instance));

    private readonly Dictionary<FormatterDefinition, FormatterAdapter> adapters;

    private FormatterAdapterFactory(Dictionary<FormatterDefinition, FormatterAdapter> adapters)
    {
        this.adapters = adapters;
    }

    public static FormatterAdapterFactory Create(IEnumerable<FormatterDefinition> definitions)
    {
        Dictionary<FormatterDefinition, FormatterAdapter> adapters =
            new(FormatterDefinitionReferenceComparer.Instance);
        foreach (FormatterDefinition definition in definitions)
        {
            FormatterAdapter? adapter = CreateAdapter(definition);
            if (adapter is not null)
            {
                adapters.Add(definition, adapter);
            }
        }
        return adapters.Count == 0 ? Empty : new FormatterAdapterFactory(adapters);
    }

    public bool TryGetAdapter(FormatterDefinition definition, [NotNullWhen(true)] out FormatterAdapter? adapter) =>
        adapters.TryGetValue(definition, out adapter);

    private static FormatterAdapter? CreateAdapter(FormatterDefinition definition)
    {
        Type delegateType = definition.Formatter.GetType();
        if (!delegateType.IsGenericType)
        {
            return null;
        }

        Type genericDefinition = delegateType.GetGenericTypeDefinition();
        Type[] arguments = delegateType.GetGenericArguments();
        if (genericDefinition == typeof(Func<,,>)
            && arguments[1] == typeof(string)
            && arguments[2] == typeof(string)
            && arguments[0].IsAssignableFrom(definition.RequiredType))
        {
            return (FormatterAdapter)TwoArgumentAdapterFactory
                .MakeGenericMethod(arguments[0])
                .Invoke(null, [definition.Formatter])!;
        }
        if (genericDefinition == typeof(Func<,,,>)
            && arguments[1] == typeof(string)
            && arguments[2] == typeof(string)
            && arguments[3] == typeof(string)
            && arguments[0].IsAssignableFrom(definition.RequiredType))
        {
            return (FormatterAdapter)ThreeArgumentAdapterFactory
                .MakeGenericMethod(arguments[0])
                .Invoke(null, [definition.Formatter])!;
        }
        return null;
    }

    private static FormatterAdapter CreateTwoArgumentAdapter<T>(Delegate formatter)
    {
        Func<T, string, string> typedFormatter = (Func<T, string, string>)formatter;
        return (value, formatString, _) => Invoke(typedFormatter, (T)value, formatString);
    }

    private static FormatterAdapter CreateThreeArgumentAdapter<T>(Delegate formatter)
    {
        Func<T, string, string, string> typedFormatter = (Func<T, string, string, string>)formatter;
        return (value, formatString, tokenName) => Invoke(typedFormatter, (T)value, formatString, tokenName);
    }

    private static string Invoke<T>(Func<T, string, string> formatter, T value, string formatString)
    {
        try
        {
            return formatter(value, formatString);
        }
        catch (Exception exception)
        {
            throw new TargetInvocationException(exception);
        }
    }

    private static string Invoke<T>(Func<T, string, string, string> formatter, T value, string formatString, string tokenName)
    {
        try
        {
            return formatter(value, formatString, tokenName);
        }
        catch (Exception exception)
        {
            throw new TargetInvocationException(exception);
        }
    }

    private sealed class FormatterDefinitionReferenceComparer : IEqualityComparer<FormatterDefinition>
    {
        public static FormatterDefinitionReferenceComparer Instance { get; } = new();

        public bool Equals(FormatterDefinition? x, FormatterDefinition? y) => ReferenceEquals(x, y);

        public int GetHashCode(FormatterDefinition obj) => RuntimeHelpers.GetHashCode(obj);
    }
}

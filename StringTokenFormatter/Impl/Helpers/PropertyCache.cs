using System.Linq.Expressions;
using System.Reflection;
using System.Collections.Concurrent;
using System.Collections;

namespace StringTokenFormatter.Impl;

public static class PropertyCache<T>
{
    public record PropertyPairs(string PropertyName, Func<T, object> GetValue);

    public static readonly IReadOnlyCollection<PropertyPairs> Properties = GetPropertyPairs();
    public static int Count => Properties.Count;

    private static List<PropertyPairs> GetPropertyPairs() => [.. (
        from prop in GetPublicProperties(typeof(T))
        let getMethod = prop.GetGetMethod()
        where getMethod != null && getMethod.GetParameters().Length == 0
        let getterFn = CreateGetter(getMethod)
        select new PropertyPairs(prop.Name, getterFn)
    )];

    private const BindingFlags bindingFilter = BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy;

    private static IReadOnlyCollection<PropertyInfo> GetPublicProperties(Type type) =>
        type.IsInterface ? GetInterfaceProperties(type) : type.GetProperties(bindingFilter);

    private static HashSet<PropertyInfo> GetInterfaceProperties(Type type)
    {
        HashSet<PropertyInfo> propertyInfos = [];
        HashSet<Type> considered = [];
        Queue<Type> queue = new();
        considered.Add(type);
        queue.Enqueue(type);

        while (queue.Count > 0)
        {
            Type subType = queue.Dequeue();

            foreach (Type subInterface in subType.GetInterfaces())
            {
                if (considered.Contains(subInterface)) { continue; }
                considered.Add(subInterface);
                queue.Enqueue(subInterface);
            }

            PropertyInfo[] typeProperties = subType.GetProperties(bindingFilter);
            propertyInfos.UnionWith(typeProperties);
        }

        return propertyInfos;
    }

    private static Func<T, object> CreateGetter(MethodInfo Getter)
    {
        ParameterExpression instance = Expression.Parameter(typeof(T), "instance");
        UnaryExpression exp = Expression.Convert(Expression.Call(instance, Getter), typeof(object));
        ParameterExpression[] parameters = [instance];
        Func<T, object> ret = Expression.Lambda<Func<T, object>>(exp, parameters).Compile();
        return ret;
    }
}

internal static class NestedPropertyCache
{
    internal sealed record PropertyPair(string PropertyName, Func<object, object?> GetValue);

    private sealed record GenericDictionaryInfo(PropertyInfo KeyProperty, PropertyInfo ValueProperty);

    private static readonly ConcurrentDictionary<Type, IReadOnlyCollection<PropertyPair>> cache = new();
    private static readonly ConcurrentDictionary<Type, GenericDictionaryInfo?> genericDictionaryInfos = new();
    private const BindingFlags bindingFilter = BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy;

    public static IReadOnlyCollection<PropertyPair> GetProperties(Type type) => cache.GetOrAdd(type, CreatePropertyPairs);

    private static IReadOnlyCollection<PropertyPair> CreatePropertyPairs(Type type) => [.. (
        from prop in type.GetProperties(bindingFilter)
        let getMethod = prop.GetGetMethod()
        where getMethod != null && getMethod.GetParameters().Length == 0
        let getter = CreateGetter(getMethod)
        select new PropertyPair(prop.Name, getter)
    )];

    private static Func<object, object?> CreateGetter(MethodInfo getMethod)
    {
        ParameterExpression instance = Expression.Parameter(typeof(object), "instance");
        UnaryExpression exp = Expression.Convert(Expression.Call(Expression.Convert(instance, getMethod.DeclaringType!), getMethod), typeof(object));
        return Expression.Lambda<Func<object, object?>>(exp, instance).Compile();
    }

    public static bool TryGetChildValue(object parent, string name, StringComparer nameComparer, out object? value)
    {
        if (parent is IDictionary dictionary)
        {
            foreach (DictionaryEntry entry in dictionary)
            {
                if (entry.Key is string key && nameComparer.Equals(key, name))
                {
                    value = entry.Value;
                    return true;
                }
            }
            value = null;
            return false;
        }

        // Handles string-keyed dictionaries that don't implement the non-generic IDictionary,
        // such as FrozenDictionary<string, T> and ImmutableDictionary<string, T>.
        GenericDictionaryInfo? genericDictionary = GetGenericDictionaryInfo(parent.GetType());
        if (genericDictionary is not null)
        {
            foreach (object entry in (IEnumerable)parent)
            {
                string key = (string)genericDictionary.KeyProperty.GetValue(entry)!;
                if (nameComparer.Equals(key, name))
                {
                    value = genericDictionary.ValueProperty.GetValue(entry);
                    return true;
                }
            }
            value = null;
            return false;
        }

        PropertyPair? match = null;
        foreach (PropertyPair property in GetProperties(parent.GetType()))
        {
            if (!nameComparer.Equals(property.PropertyName, name)) { continue; }
            if (match is not null) { throw new TokenContainerException($"The object has multiple properties matching '{name}'"); }
            match = property;
        }
        if (match is null)
        {
            value = null;
            return false;
        }
        value = match.GetValue(parent);
        return true;
    }

    private static GenericDictionaryInfo? GetGenericDictionaryInfo(Type type) =>
        genericDictionaryInfos.GetOrAdd(type, CreateGenericDictionaryInfo);

    private static GenericDictionaryInfo? CreateGenericDictionaryInfo(Type type)
    {
        Type? dictionaryInterface = type.GetInterfaces()
            .Where(i => i.IsGenericType && i.GetGenericArguments()[0] == typeof(string))
            .FirstOrDefault(i =>
            {
                Type definition = i.GetGenericTypeDefinition();
                return definition == typeof(IDictionary<,>) || definition == typeof(IReadOnlyDictionary<,>);
            });
        if (dictionaryInterface is null) { return null; }
        Type pairType = typeof(KeyValuePair<,>).MakeGenericType(dictionaryInterface.GetGenericArguments());
        return new GenericDictionaryInfo(
            pairType.GetProperty(nameof(KeyValuePair<string, object>.Key))!,
            pairType.GetProperty(nameof(KeyValuePair<string, object>.Value))!);
    }
}
using System.Linq.Expressions;
using System.Reflection;

namespace StringTokenFormatter.Impl;

/// <summary>
/// Converts only the properties exposed by {T} (but not any members on derived classes) to a token value container.
/// </summary>
/// <typeparam name="T">A type indicating the exact properties that will be used for formatting.</typeparam>
public class ObjectTokenValueContainer<T> : ITokenValueContainer
{
    private static readonly PropertyCache<T> propertyCache = new();
    private readonly IDictionary<string, NonLockingLazy<object>> dictionary;
    private readonly ITokenContainerSettings settings;

    public ObjectTokenValueContainer(T source, ITokenContainerSettings settings)
    {
        if (source == null) { throw new ArgumentNullException(nameof(source)); }
        this.settings = settings ?? throw new ArgumentNullException(nameof(settings));

        dictionary = propertyCache.GetPairs().ToDictionary(p => p.Property.Name, p => new NonLockingLazy<object>(() => p.Getter(source)));
    }

    public TryGetResult TryMap(string token) =>
        dictionary.TryGetValue(token, out var value) && settings.TokenResolutionPolicy.Satisfies(value.Value) ? TryGetResult.Success(value.Value) : default;
}

public class PropertyCache<T>
{
    public record PropertyPairs(PropertyInfo Property, Func<T, object> Getter);

    const BindingFlags bindingFilter = BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy;
    private readonly Lazy<List<PropertyPairs>> propertyCache = new(GetPropertyPairs);

    public IEnumerable<PropertyPairs> GetPairs() => propertyCache.Value;

    private static List<PropertyPairs> GetPropertyPairs() => (
        from prop in GetPublicProperties(typeof(T))
        let getMethod = prop.GetGetMethod()
        where getMethod != null && getMethod.GetParameters().Length == 0
        let getterFn = CreateGetter(getMethod)
        select new PropertyPairs(prop, getterFn)
    ).ToList();

    private static IEnumerable<PropertyInfo> GetPublicProperties(Type type) =>
        type.IsInterface ? GetInterfaceProperties(type) : type.GetProperties(bindingFilter);

    private static IEnumerable<PropertyInfo> GetInterfaceProperties(Type type)
    {
        var propertyInfos = new HashSet<PropertyInfo>();
        var considered = new HashSet<Type>();
        var queue = new Queue<Type>();
        considered.Add(type);
        queue.Enqueue(type);

        while (queue.Count > 0)
        {
            var subType = queue.Dequeue();
            foreach (var subInterface in subType.GetInterfaces())
            {
                if (considered.Contains(subInterface)) { continue; }
                considered.Add(subInterface);
                queue.Enqueue(subInterface);
            }

            var typeProperties = subType.GetProperties(bindingFilter);
            propertyInfos.UnionWith(typeProperties);
        }

        return propertyInfos;
    }

    private static Func<T, object> CreateGetter(MethodInfo Getter)
    {
        var instance = Expression.Parameter(typeof(T), "instance");
        var ex = Expression.Convert(Expression.Call(instance, Getter), typeof(object));
        var parameters = new ParameterExpression[] { instance };
        var ret = Expression.Lambda<Func<T, object>>(ex, parameters).Compile();
        return ret;
    }
}
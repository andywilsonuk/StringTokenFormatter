using System.Linq.Expressions;
using System.Reflection;

namespace StringTokenFormatter.Impl;

public static class PropertyCache<T>
{
    public record PropertyPairs(string PropertyName, Func<T, object> GetValue);

    public static readonly IReadOnlyCollection<PropertyPairs> Properties = GetPropertyPairs();
    public static int Count => Properties.Count;

    private static List<PropertyPairs> GetPropertyPairs() => (
        from prop in GetPublicProperties(typeof(T))
        let getMethod = prop.GetGetMethod()
        where getMethod != null && getMethod.GetParameters().Length == 0
        let getterFn = CreateGetter(getMethod)
        select new PropertyPairs(prop.Name, getterFn)
    ).ToList();

    private const BindingFlags bindingFilter = BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy;

    private static IReadOnlyCollection<PropertyInfo> GetPublicProperties(Type type) =>
        type.IsInterface ? GetInterfaceProperties(type) : type.GetProperties(bindingFilter);

    private static HashSet<PropertyInfo> GetInterfaceProperties(Type type)
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
        var exp = Expression.Convert(Expression.Call(instance, Getter), typeof(object));
        var parameters = new ParameterExpression[] { instance };
        var ret = Expression.Lambda<Func<T, object>>(exp, parameters).Compile();
        return ret;
    }
}
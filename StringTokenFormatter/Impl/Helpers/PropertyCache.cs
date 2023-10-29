using System.Linq.Expressions;
using System.Reflection;

namespace StringTokenFormatter.Impl;

public class PropertyCache<T>
{
    public record PropertyPairs(PropertyInfo Property, Func<T, object> Getter);

    const BindingFlags bindingFilter = BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy;
    private readonly List<PropertyPairs> propertyCache = GetPropertyPairs();

    public IEnumerable<PropertyPairs> GetPairs() => propertyCache;

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
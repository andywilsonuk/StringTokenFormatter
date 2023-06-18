using System.Collections.Concurrent;
using System.Linq.Expressions;

namespace StringTokenFormatter.Impl.TokenValueContainers;

static class ObjectPropertiesTokenValueContainerFactoryImpl {

    //Default constructor
    public static ObjectPropertiesTokenValueContainerImpl<T> Create<T>(T tokenValueObject, ITokenNameComparer nameComparer) => new(tokenValueObject, nameComparer);

    //Create a generic using reflection super fast!
    public static ITokenValueContainer Create(object tokenValueObject, ITokenNameComparer? nameComparer = default) {
        if (tokenValueObject == null) throw new ArgumentNullException(nameof(tokenValueObject));

        var Factory = FactoryCache.GetOrAdd(tokenValueObject.GetType(), x => GenerateFactory(x));

        var ret = Factory(tokenValueObject, nameComparer);

        return ret;
    }

    private static readonly ConcurrentDictionary<Type, Func<object, ITokenNameComparer?, ITokenValueContainer>> FactoryCache = new();

    private static Func<object, ITokenNameComparer?, ITokenValueContainer> GenerateFactory(Type T) {

        var InstanceType = typeof(ObjectPropertiesTokenValueContainerImpl<>).MakeGenericType(T);
        var Constructor = InstanceType.GetConstructor(new[] { T, typeof(ITokenNameComparer) });

        var tokenValueObjectParameter = Expression.Parameter(typeof(object));
        var parserParameter = Expression.Parameter(typeof(ITokenNameComparer));

        var parameters = new ParameterExpression[] { tokenValueObjectParameter, parserParameter };

        //Call the constructor using the our two parameters
        var ex =
            Expression.New(Constructor, 
                Expression.Convert(tokenValueObjectParameter, T), //We have to convert our TokenValueObject to a generic T.
                parserParameter
                );
            ;

        var ret = Expression.Lambda<Func<object, ITokenNameComparer?, ITokenValueContainer>>(ex, parameters)
            .Compile()
            ;
        
        return ret;
    }

}

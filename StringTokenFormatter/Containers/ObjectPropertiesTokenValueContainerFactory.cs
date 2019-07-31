using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;

namespace StringTokenFormatter {
    static class ObjectPropertiesTokenValueContainerFactory {

        //Default constructor
        public static ObjectPropertiesTokenValueContainer<T> Create<T>(T tokenValueObject, ITokenParser parser = default) {
            return new ObjectPropertiesTokenValueContainer<T>(tokenValueObject, parser);
        }

        //Create a generic using reflection super fast!
        public static ITokenValueContainer Create(Object tokenValueObject, ITokenParser parser = default) {
            if (tokenValueObject == null) throw new ArgumentNullException(nameof(tokenValueObject));

            var Factory = FactoryCache.GetOrAdd(tokenValueObject.GetType(), x => GenerateFactory(x));

            var ret = Factory(tokenValueObject, parser);

            return ret;
        }

        private static ConcurrentDictionary<Type, Func<object, ITokenParser, ITokenValueContainer>> FactoryCache = new ConcurrentDictionary<Type, Func<object, ITokenParser, ITokenValueContainer>>();
        private static Func<object, ITokenParser, ITokenValueContainer> GenerateFactory(Type T) {

            var InstanceType = typeof(ObjectPropertiesTokenValueContainer<>).MakeGenericType(T);
            var Constructor = InstanceType.GetConstructor(new[] { T, typeof(ITokenParser) });

            var tokenValueObjectParameter = Expression.Parameter(typeof(object));
            var parserParameter = Expression.Parameter(typeof(ITokenParser));

            var parameters = new ParameterExpression[] { tokenValueObjectParameter, parserParameter };

            //Call the constructor using the our two parameters
            var ex =
                Expression.New(Constructor, 
                    Expression.Convert(tokenValueObjectParameter, T), //We have to convert our TokenValueObject to a generic T.
                    parserParameter
                    );
                ;

            var ret = Expression.Lambda<Func<object, ITokenParser, ITokenValueContainer>>(ex, parameters)
                .Compile()
                ;
            
            return ret;
        }

    }
}

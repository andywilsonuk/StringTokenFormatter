using StringTokenFormatter.Impl.TokenValueContainers;

namespace StringTokenFormatter.Impl;


internal class TokenValueContainerFactoryImpl : ITokenValueContainerFactory {

    protected ITokenNameComparer NameComparer { get; }

    public TokenValueContainerFactoryImpl(ITokenNameComparer nameComparer) {
        this.NameComparer = nameComparer;
    }

    public virtual ITokenValueContainer FromObject<T>(T values) => ObjectPropertiesTokenValueContainerFactoryImpl.Create(values, NameComparer);

    public virtual ITokenValueContainer FromObject(object values) => ObjectPropertiesTokenValueContainerFactoryImpl.Create(values, NameComparer);

    public virtual ITokenValueContainer FromValue<T>(string name, T value) => new SingleTokenValueContainerImpl<T>(name, value, NameComparer);

    public virtual ITokenValueContainer FromFunc<T>(Func<string, ITokenNameComparer, TryGetResult> values) => new FuncTokenValueContainerImpl<T>(values, NameComparer);

    public virtual ITokenValueContainer FromFunc<T>(Func<string, ITokenNameComparer, T> values) => new FuncTokenValueContainerImpl<T>(values, NameComparer);

    public virtual ITokenValueContainer FromFunc<T>(Func<string, T> values) => new FuncTokenValueContainerImpl<T>(values, NameComparer);

    public virtual ITokenValueContainer FromDictionary<T>(IEnumerable<KeyValuePair<string, T>> values) => new DictionaryTokenValueContainerImpl<T>(values, NameComparer);

    public virtual ITokenValueContainer Combine(IEnumerable<ITokenValueContainer> containers) => new CompositeTokenValueContainerImpl(containers);

    public virtual ITokenValueContainer Combine(params ITokenValueContainer[] containers) => new CompositeTokenValueContainerImpl(containers);

    public virtual ITokenValueContainer Empty() => EmptyTokenValueContainerImpl.Instance;

    public virtual ITokenValueContainer IgnoreNullTokenValues(ITokenValueContainer This) => new IgnoreNullOrEmptyTokenValueContainerImpl(This);

    public virtual ITokenValueContainer IgnoreNullOrEmptyTokenValues(ITokenValueContainer This) => new IgnoreNullOrEmptyTokenValueContainerImpl(This);

}
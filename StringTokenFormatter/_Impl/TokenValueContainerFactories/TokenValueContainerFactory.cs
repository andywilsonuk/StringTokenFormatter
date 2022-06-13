using StringTokenFormatter.Impl.TokenValueContainers;
using System;
using System.Collections.Generic;

namespace StringTokenFormatter.Impl {

    internal class TokenValueContainerFactoryImpl : ITokenValueContainerFactory {

        protected ITokenNameComparer NameComparer { get; }

        public TokenValueContainerFactoryImpl(ITokenNameComparer nameComparer) {
            this.NameComparer = nameComparer;
        }

        public virtual ITokenValueContainer FromObject<T>(T values) {
            return ObjectPropertiesTokenValueContainerFactoryImpl.Create(values, NameComparer);
        }

        public virtual ITokenValueContainer FromObject(object values) {
            return ObjectPropertiesTokenValueContainerFactoryImpl.Create(values, NameComparer);
        }

        public virtual ITokenValueContainer FromValue<T>(string name, T value) {
            return new SingleTokenValueContainerImpl<T>(name, value, NameComparer);
        }

        public virtual ITokenValueContainer FromFunc<T>(Func<string, ITokenNameComparer, TryGetResult> values) {
            return new FuncTokenValueContainerImpl<T>(values, NameComparer);
        }

        public virtual ITokenValueContainer FromFunc<T>(Func<string, ITokenNameComparer, T> values) {
            return new FuncTokenValueContainerImpl<T>(values, NameComparer);
        }

        public virtual ITokenValueContainer FromFunc<T>(Func<string, T> values) {
            return new FuncTokenValueContainerImpl<T>(values, NameComparer);
        }

        public virtual ITokenValueContainer FromDictionary<T>(IEnumerable<KeyValuePair<string, T>> values) {
            return new DictionaryTokenValueContainerImpl<T>(values, NameComparer);
        }

        public virtual ITokenValueContainer Combine(IEnumerable<ITokenValueContainer> containers) {
            return new CompositeTokenValueContainerImpl(containers);
        }

        public virtual ITokenValueContainer Combine(params ITokenValueContainer[] containers) {
            return new CompositeTokenValueContainerImpl(containers);
        }

        public virtual ITokenValueContainer Empty() {
            return EmptyTokenValueContainerImpl.Instance;
        }

        public virtual ITokenValueContainer IgnoreNullTokenValues(ITokenValueContainer This) {
            return new IgnoreNullOrEmptyTokenValueContainerImpl(This);
        }

        public virtual ITokenValueContainer IgnoreNullOrEmptyTokenValues(ITokenValueContainer This) {
            return new IgnoreNullOrEmptyTokenValueContainerImpl(This);
        }

    }
}
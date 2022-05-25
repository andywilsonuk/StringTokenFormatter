using StringTokenFormatter.Impl.TokenValueContainers;
using StringTokenFormatter.Impl.TokenNameComparers;
using System;
using System.Collections.Generic;
using StringTokenFormatter.Impl;

namespace StringTokenFormatter.Impl.TokenValueContainerFactories {

    public class TokenValueContainerFactory : ITokenValueContainerFactory {

        protected ITokenNameComparer nameComparer { get; }

        public TokenValueContainerFactory(ITokenNameComparer nameComparer) {
            this.nameComparer = nameComparer;
        }

        public virtual ITokenValueContainer FromObject<T>(T values) {
            return ObjectPropertiesTokenValueContainerFactory.Create(values, nameComparer);
        }

        public virtual ITokenValueContainer FromObject(object values) {
            return ObjectPropertiesTokenValueContainerFactory.Create(values, nameComparer);
        }

        public virtual ITokenValueContainer FromValue<T>(string name, T value) {
            return new SingleTokenValueContainer<T>(name, value, nameComparer);
        }

        public virtual ITokenValueContainer FromFunc<T>(Func<string, ITokenNameComparer, TryGetResult> values) {
            return new FuncTokenValueContainer<T>(values, nameComparer);
        }

        public virtual ITokenValueContainer FromFunc<T>(Func<string, ITokenNameComparer, T> values) {
            return new FuncTokenValueContainer<T>(values, nameComparer);
        }

        public virtual ITokenValueContainer FromFunc<T>(Func<string, T> values) {
            return new FuncTokenValueContainer<T>(values, nameComparer);
        }

        public virtual ITokenValueContainer FromDictionary<T>(IEnumerable<KeyValuePair<string, T>> values) {
            return new DictionaryTokenValueContainer<T>(values, nameComparer);
        }

        public virtual ITokenValueContainer Combine(IEnumerable<ITokenValueContainer> containers) {
            return new CompositeTokenValueContainer(containers);
        }

        public virtual ITokenValueContainer Combine(params ITokenValueContainer[] containers) {
            return new CompositeTokenValueContainer(containers);
        }

        public virtual ITokenValueContainer Empty() {
            return EmptyTokenValueContainer.Instance;
        }

        public virtual ITokenValueContainer IgnoreNullTokenValues(ITokenValueContainer This) {
            return new IgnoreNullOrEmptyTokenValueContainer(This);
        }

        public virtual ITokenValueContainer IgnoreNullOrEmptyTokenValues(ITokenValueContainer This) {
            return new IgnoreNullOrEmptyTokenValueContainer(This);
        }

    }
}
using StringTokenFormatter.Impl.TokenValueContainers;
using StringTokenFormatter.Impl.TokenNameComparers;
using System;
using System.Collections.Generic;
using StringTokenFormatter.Impl;

namespace StringTokenFormatter {

    public static class TokenValueContainer {


        public static ITokenValueContainer FromObject<T>(T values) {
            return FromObject(values, InterpolationSettings.Default);
        }

        public static ITokenValueContainer FromObject<T>(T values, IInterpolationSettings Settings) {
            return Settings.TokenValueContainerFactory.FromObject(values);
        }


        public static ITokenValueContainer FromObject(object values) {
            return FromObject(values, InterpolationSettings.Default);
        }

        public static ITokenValueContainer FromObject(object values, IInterpolationSettings Settings) {
            return Settings.TokenValueContainerFactory.FromObject(values);
        }

        public static ITokenValueContainer FromValue<T>(string name, T value) {
            return FromValue(name, value, InterpolationSettings.Default);
        }

        public static ITokenValueContainer FromValue<T>(string name, T value, IInterpolationSettings Settings) {
            return Settings.TokenValueContainerFactory.FromValue(name, value);
        }

        public static ITokenValueContainer FromFunc(Func<string, ITokenNameComparer, TryGetResult> values) {
            return FromFunc(values, InterpolationSettings.Default);
        }

        public static ITokenValueContainer FromFunc(Func<string, ITokenNameComparer, TryGetResult> values, IInterpolationSettings Settings) {
            return Settings.TokenValueContainerFactory.FromFunc(values);
        }

        public static ITokenValueContainer FromFunc<T>(Func<string, ITokenNameComparer, T> values) {
            return FromFunc(values, InterpolationSettings.Default);
        }
        public static ITokenValueContainer FromFunc<T>(Func<string, ITokenNameComparer, T> values, IInterpolationSettings Settings) {
            return Settings.TokenValueContainerFactory.FromFunc(values);
        }

        public static ITokenValueContainer FromFunc<T>(Func<string, T> values) {
            return FromFunc(values, InterpolationSettings.Default);
        }

        public static ITokenValueContainer FromFunc<T>(Func<string, T> values, IInterpolationSettings Settings) {
            return Settings.TokenValueContainerFactory.FromFunc(values);
        }

        public static ITokenValueContainer FromDictionary<T>(IEnumerable<KeyValuePair<string, T>> values) {
            return FromDictionary(values, InterpolationSettings.Default);
        }

        public static ITokenValueContainer FromDictionary<T>(IEnumerable<KeyValuePair<string, T>> values, IInterpolationSettings Settings) {
            return Settings.TokenValueContainerFactory.FromDictionary(values);
        }

        public static ITokenValueContainer Combine(IEnumerable<ITokenValueContainer> containers) {
            return Combine(containers, InterpolationSettings.Default);
        }

        public static ITokenValueContainer Combine(IEnumerable<ITokenValueContainer> containers, IInterpolationSettings Settings) {
            return Settings.TokenValueContainerFactory.Combine(containers);
        }

        public static ITokenValueContainer Combine(params ITokenValueContainer[] containers) {
            return Combine(InterpolationSettings.Default, containers);
        }

        public static ITokenValueContainer Combine(IInterpolationSettings Settings, params ITokenValueContainer[] containers) {
            return Settings.TokenValueContainerFactory.Combine(containers);
        }

        public static ITokenValueContainer Empty() {
            return Empty(InterpolationSettings.Default);
        }

        public static ITokenValueContainer Empty(IInterpolationSettings Settings) {
            return EmptyTokenValueContainer.Instance;
        }

        public static ITokenValueContainer IgnoreNullTokenValues(ITokenValueContainer This) {
            return IgnoreNullTokenValues(This, InterpolationSettings.Default);
        }

        public static ITokenValueContainer IgnoreNullTokenValues(ITokenValueContainer This, IInterpolationSettings Settings) {
            return Settings.TokenValueContainerFactory.IgnoreNullTokenValues(This);
        }

        public static ITokenValueContainer IgnoreNullOrEmptyTokenValues(ITokenValueContainer This) {
            return IgnoreNullOrEmptyTokenValues(This, InterpolationSettings.Default);
        }

        public static ITokenValueContainer IgnoreNullOrEmptyTokenValues(ITokenValueContainer This, IInterpolationSettings Settings) {
            return Settings.TokenValueContainerFactory.IgnoreNullOrEmptyTokenValues(This);
        }

    }
}
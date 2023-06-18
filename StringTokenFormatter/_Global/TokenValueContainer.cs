using StringTokenFormatter.Impl.TokenValueContainers;

namespace StringTokenFormatter;


public static class TokenValueContainer {


    public static ITokenValueContainer FromObject<T>(T values) => FromObject(values, InterpolationSettings.Default);

    public static ITokenValueContainer FromObject<T>(T values, IInterpolationSettings Settings) => Settings.TokenValueContainerFactory.FromObject(values);


    public static ITokenValueContainer FromObject(object values) => FromObject(values, InterpolationSettings.Default);

    public static ITokenValueContainer FromObject(object values, IInterpolationSettings Settings) => Settings.TokenValueContainerFactory.FromObject(values);

    public static ITokenValueContainer FromValue<T>(string name, T value) => FromValue(name, value, InterpolationSettings.Default);

    public static ITokenValueContainer FromValue<T>(string name, T value, IInterpolationSettings Settings) => Settings.TokenValueContainerFactory.FromValue(name, value);

    public static ITokenValueContainer FromFunc(Func<string, ITokenNameComparer, TryGetResult> values) => FromFunc(values, InterpolationSettings.Default);

    public static ITokenValueContainer FromFunc(Func<string, ITokenNameComparer, TryGetResult> values, IInterpolationSettings Settings) => Settings.TokenValueContainerFactory.FromFunc(values);

    public static ITokenValueContainer FromFunc<T>(Func<string, ITokenNameComparer, T> values) => FromFunc(values, InterpolationSettings.Default);
    public static ITokenValueContainer FromFunc<T>(Func<string, ITokenNameComparer, T> values, IInterpolationSettings Settings) => Settings.TokenValueContainerFactory.FromFunc(values);

    public static ITokenValueContainer FromFunc<T>(Func<string, T> values) => FromFunc(values, InterpolationSettings.Default);

    public static ITokenValueContainer FromFunc<T>(Func<string, T> values, IInterpolationSettings Settings) => Settings.TokenValueContainerFactory.FromFunc(values);

    public static ITokenValueContainer FromDictionary<T>(IEnumerable<KeyValuePair<string, T>> values) => FromDictionary(values, InterpolationSettings.Default);

    public static ITokenValueContainer FromDictionary<T>(IEnumerable<KeyValuePair<string, T>> values, IInterpolationSettings Settings) => Settings.TokenValueContainerFactory.FromDictionary(values);

    public static ITokenValueContainer Combine(IEnumerable<ITokenValueContainer> containers) => Combine(containers, InterpolationSettings.Default);

    public static ITokenValueContainer Combine(IEnumerable<ITokenValueContainer> containers, IInterpolationSettings Settings) => Settings.TokenValueContainerFactory.Combine(containers);

    public static ITokenValueContainer Combine(params ITokenValueContainer[] containers) => Combine(InterpolationSettings.Default, containers);

    public static ITokenValueContainer Combine(IInterpolationSettings Settings, params ITokenValueContainer[] containers) => Settings.TokenValueContainerFactory.Combine(containers);

    public static ITokenValueContainer Empty() => Empty(InterpolationSettings.Default);

    public static ITokenValueContainer Empty(IInterpolationSettings Settings) => EmptyTokenValueContainerImpl.Instance;

    public static ITokenValueContainer IgnoreNullTokenValues(ITokenValueContainer This) => IgnoreNullTokenValues(This, InterpolationSettings.Default);

    public static ITokenValueContainer IgnoreNullTokenValues(ITokenValueContainer This, IInterpolationSettings Settings) => Settings.TokenValueContainerFactory.IgnoreNullTokenValues(This);

    public static ITokenValueContainer IgnoreNullOrEmptyTokenValues(ITokenValueContainer This) => IgnoreNullOrEmptyTokenValues(This, InterpolationSettings.Default);

    public static ITokenValueContainer IgnoreNullOrEmptyTokenValues(ITokenValueContainer This, IInterpolationSettings Settings) => Settings.TokenValueContainerFactory.IgnoreNullOrEmptyTokenValues(This);

}
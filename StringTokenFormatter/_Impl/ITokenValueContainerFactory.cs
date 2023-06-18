namespace StringTokenFormatter;

public interface ITokenValueContainerFactory {
    ITokenValueContainer FromObject<T>(T values);

    ITokenValueContainer FromObject(object values);

    ITokenValueContainer FromValue<T>(string name, T value);

    ITokenValueContainer FromFunc<T>(Func<string, ITokenNameComparer, T> values);

    ITokenValueContainer FromFunc<T>(Func<string, T> values);

    ITokenValueContainer FromDictionary<T>(IEnumerable<KeyValuePair<string, T>> values);

    ITokenValueContainer Combine(IEnumerable<ITokenValueContainer> containers);

    ITokenValueContainer Combine(params ITokenValueContainer[] containers);

    ITokenValueContainer Empty();

    ITokenValueContainer IgnoreNullTokenValues(ITokenValueContainer This);

    ITokenValueContainer IgnoreNullOrEmptyTokenValues(ITokenValueContainer This);
}
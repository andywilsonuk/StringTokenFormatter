#if NET8_0_OR_GREATER
using System.Collections.Frozen;
#endif

namespace StringTokenFormatter.Impl;

internal static class DictionaryExtensions
{
    public static IDictionary<TKey, TValue> ToFrozenDictionaryIfSupported<TKey, TValue>(this IDictionary<TKey, TValue> dictionary)
        where TKey : notnull
    {
#if NET8_0_OR_GREATER
        return dictionary.ToFrozenDictionary();
#else
        return dictionary;
#endif
    }
}

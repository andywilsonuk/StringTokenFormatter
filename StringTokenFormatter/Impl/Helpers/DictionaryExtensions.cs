#if NET8_0_OR_GREATER
using System.Collections.Frozen;
#endif

namespace StringTokenFormatter.Impl;

internal static class DictionaryExtensions
{
    extension<TKey, TValue>(IDictionary<TKey, TValue> dictionary) where TKey : notnull
    {
        public IDictionary<TKey, TValue> ToFrozenDictionaryIfSupported()
        {
#if NET8_0_OR_GREATER
            return dictionary.ToFrozenDictionary();
#else
            return dictionary;
#endif
        }
    }
}

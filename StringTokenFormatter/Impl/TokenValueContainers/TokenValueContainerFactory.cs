﻿namespace StringTokenFormatter.Impl;

public static class TokenValueContainerFactory
{
    public static TokenValueContainer<T> FromDictionary<T>(ITokenValueContainerSettings settings, IEnumerable<TokenValue<T>> source) => new(source, settings);
    public static ObjectTokenValueContainer<T> FromObject<T>(ITokenValueContainerSettings settings, T source) => new(source, settings);
    public static TokenValueContainer<T> FromSingle<T>(ITokenValueContainerSettings settings, string token, T value) => new(new TokenValue<T>[] { new(token, value) }, settings);
    public static FuncTokenValueContainer<T> FromFunc<T>(ITokenValueContainerSettings settings, Func<string, T> func) => new(func, settings);
    public static CompositeTokenValueContainer FromCombination(ITokenValueContainerSettings settings, IEnumerable<ITokenValueContainer> containers) => new(containers, settings);
    public static CompositeTokenValueContainer FromCombination(ITokenValueContainerSettings settings, params ITokenValueContainer[] containers) => new(containers, settings);
}
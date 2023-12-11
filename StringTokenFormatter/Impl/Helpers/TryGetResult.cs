using System.Runtime.CompilerServices;

namespace StringTokenFormatter.Impl;

public readonly record struct TryGetResult(bool IsSuccess, object? Value)
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TryGetResult Success(object? Value) => new(true, Value);
}
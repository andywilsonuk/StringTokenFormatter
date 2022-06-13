using System.Runtime.CompilerServices;

namespace StringTokenFormatter {
    public readonly record struct TryGetResult {
        public bool IsSuccess { get; init; }
        public object? Value { get; init; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TryGetResult Success(object? Value) {
            var ret = new TryGetResult()
            {
                IsSuccess = true,
                Value = Value,
            };

            return ret;
        }

        public void Deconstruct(out bool IsSuccess, out object? Value) {
            IsSuccess = this.IsSuccess;
            Value = this.Value;
        }

    }
}
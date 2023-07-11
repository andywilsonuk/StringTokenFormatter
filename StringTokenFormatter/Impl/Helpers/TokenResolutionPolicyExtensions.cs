namespace StringTokenFormatter.Impl;

public static class TokenResolutionPolicyExtensions
{
    public static bool Satisfies(this TokenResolutionPolicy policy, object? value) => policy switch
    {
        TokenResolutionPolicy.ResolveAll => true,
        TokenResolutionPolicy.IgnoreNull when value is null => false,
        TokenResolutionPolicy.IgnoreNullOrEmpty when value is string s && s.Length == 0 => false,
        _ => true,
    };
}

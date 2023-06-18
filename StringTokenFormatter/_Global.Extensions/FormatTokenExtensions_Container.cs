namespace StringTokenFormatter; 
public static class TokenValueContainerExtensions {
    public static string FormatContainer(this ITokenValueContainer container, string input) => input.FormatContainer(container);

    public static string FormatContainer(this ITokenValueContainer container, string input, IInterpolationSettings Settings) => input.FormatContainer(container, Settings);

    public static string FormatContainer(this ITokenValueContainer container, IInterpolatedString input) => input.FormatContainer(container);

    public static string FormatContainer(this ITokenValueContainer container, IInterpolatedString input, IInterpolationSettings Settings) => input.FormatContainer(container, Settings);

}

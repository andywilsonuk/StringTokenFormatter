namespace StringTokenFormatter; 
public interface IInterpolationSettings {
    ITokenSyntax TokenSyntax { get; }
    IInterpolatedStringParser InterpolatedStringParser { get; }

    ITokenNameComparer TokenNameComparer { get; }

    ITokenValueContainerFactory TokenValueContainerFactory { get; }

    ITokenValueConverter TokenValueConverter { get; }
    ITokenValueFormatter TokenValueFormatter { get; }
}

namespace StringTokenFormatter;

public interface IInterpolatedString {
    
    IEnumerable<IInterpolatedStringSegment> Segments { get; }

    string FormatContainer(
        ITokenValueContainer container, 
        ITokenValueConverter valueConverter, 
        ITokenValueFormatter valueFormatter
        );

}
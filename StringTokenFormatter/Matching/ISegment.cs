using System.Threading.Tasks;

namespace StringTokenFormatter {

    public interface ISegment {
        string Original { get; }

        string? Evaluate(ITokenValueContainer container, ITokenValueFormatter formatter, ITokenValueConverter converter);

        Task<string?> EvaluateAsync(ITokenValueContainerAsync container, ITokenValueFormatter formatter, ITokenValueConverter converter);

    }

}

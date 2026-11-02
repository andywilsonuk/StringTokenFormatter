using System.Text;

namespace StringTokenFormatter;

public static class InterpolatedStringResolverTraceReport
{
    /// <summary>
    /// Formats the trace events as a human-readable, one-event-per-line report.
    /// </summary>
    public static string ToReport(this InterpolatedStringResolverTrace trace)
    {
        Guard.NotNull(trace, nameof(trace));

        StringBuilder report = new();
        foreach (InterpolatedStringResolverTraceEvent traceEvent in trace.Events)
        {
            switch (traceEvent)
            {
                case SegmentProcessingTraceEvent segment:
                    report.Append("Segment ")
                        .Append(segment.SegmentIndex)
                        .Append(' ')
                        .Append(segment.Status)
                        .Append(" [")
                        .Append(segment.SegmentType);
                    if (segment.Command is not null)
                    {
                        report.Append(", ")
                            .Append(segment.Command);
                    }
                    if (segment.ExceptionType is not null)
                    {
                        report.Append(", ")
                            .Append(segment.ExceptionType);
                    }
                    report.Append("]: ")
                        .Append(Quote(segment.Segment))
                        .AppendLine();
                    break;
                case TokenLookupTraceEvent lookup:
                    report.Append("Lookup ")
                        .Append(lookup.Purpose)
                        .Append(' ')
                        .Append(Quote(lookup.TokenName))
                        .Append(" via ")
                        .Append(lookup.Source)
                        .Append(": ")
                        .Append(lookup.IsSuccess ? "succeeded" : "not found")
                        .AppendLine();
                    break;
                case TextTraceEvent text:
                    report.Append("Text: ")
                        .Append(Quote(text.Text))
                        .AppendLine();
                    break;
            }
        }
        return report.ToString();
    }

    private static string Quote(string value) => $"\"{value.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\r", "\\r").Replace("\n", "\\n").Replace("\t", "\\t")}\"";
}

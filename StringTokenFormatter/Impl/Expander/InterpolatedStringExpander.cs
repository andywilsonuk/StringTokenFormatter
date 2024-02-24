namespace StringTokenFormatter.Impl;

public static class InterpolatedStringExpander
{
    public static string Expand(InterpolatedString interpolatedString, ITokenValueContainer container) => Expand(interpolatedString, container, null);

    internal static string Expand(InterpolatedString interpolatedString, ITokenValueContainer container, ExpanderValueFormatter? formatter = null)
    {
        Guard.NotNull(interpolatedString, nameof(interpolatedString));
        Guard.NotNull(container, nameof(container));

        var settings = Guard.NotNull(interpolatedString.Settings, nameof(interpolatedString.Settings)).Validate();
        formatter ??= new ExpanderValueFormatter(interpolatedString.Settings.FormatterDefinitions, interpolatedString.Settings.NameComparer);

        var iterator = new ExpandedStringIterator(interpolatedString.Segments);
        var builder = new ExpandedStringBuilder(formatter, settings.FormatProvider);
        var commands = new ExpanderCommands(settings.Commands);
        var context = new ExpanderContext(iterator, builder, container, settings, commands);
        commands.Init(context);
        IterateSegments(context);
        commands.Finished(context);
        return builder.ExpandedString();
    }

    private static void IterateSegments(ExpanderContext context)
    {
        while (context.SegmentIterator.MoveNext())
        {
            context.SegmentHandled = false;
            context.Commands.ExecuteUntil(command => command.Evaluate(context), () => context.SegmentHandled);
            if (context.SegmentHandled) { continue; }

            throw new ExpanderException($"Unhandled segment {context.SegmentIterator.Current}");
        }
    }
}

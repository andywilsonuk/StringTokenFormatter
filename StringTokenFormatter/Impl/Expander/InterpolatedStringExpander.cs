namespace StringTokenFormatter.Impl;

public static class InterpolatedStringExpander
{
    public static string Expand(InterpolatedString interpolatedString, ITokenValueContainer container)
    {
        Guard.NotNull(interpolatedString, nameof(interpolatedString));
        Guard.NotNull(container, nameof(container));

        var settings = interpolatedString.Settings;
        var iterator = new ExpandedStringIterator(interpolatedString.Segments);
        var formatter = new ExpanderValueFormatter(settings.FormatterDefinitions, settings.NameComparer);
        var builder = new ExpandedStringBuilder(formatter, settings.FormatProvider);
        var context = new ExpanderContext(iterator, builder, container, settings, settings.BlockCommands);
        IterateSegments(context);
        return builder.ExpandedString();
    }

    private static void IterateSegments(ExpanderContext context)
    {
        var iterator = context.SegmentIterator;
        while (iterator.MoveNext())
        {
            context.SkipRemainingBlockCommands = false;

            foreach (var command in context.Commands)
            {
                command.Evaluate(context);
                if (context.SkipRemainingBlockCommands) { break; }
            }
            if (!context.SkipRemainingBlockCommands)
            {
                context.EvaluateCurrentSegment();
            }
        }
        foreach (var command in context.Commands)
        {
            command.Finished(context);
        }
    }
}

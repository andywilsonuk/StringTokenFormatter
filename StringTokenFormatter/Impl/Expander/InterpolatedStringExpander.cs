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
        var context = new ExpanderContext(iterator, builder, container, settings, settings.BlockCommands);
        BlockCommandsInit(context);
        IterateSegments(context);
        BlockCommandsFinished(context);
        return builder.ExpandedString();
    }

    private static void BlockCommandsInit(ExpanderContext context)
    {
        foreach (var command in context.Commands)
        {
            command.Init(context);
        }
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
                context.EvaluateSegment(context.SegmentIterator.Current);
            }
        }
    }

    private static void BlockCommandsFinished(ExpanderContext context)
    {
        foreach (var command in context.Commands)
        {
            command.Finished(context);
        }
    }
}

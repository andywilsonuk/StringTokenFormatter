namespace StringTokenFormatter.Impl;

public static class InterpolatedStringExpander
{
    private static readonly IExpanderCommand[] standardCommands = new IExpanderCommand[] { ExpanderCommandFactory.StandardToken, ExpanderCommandFactory.StandardLiteral };

    public static string Expand(InterpolatedString interpolatedString, ITokenValueContainer container) => Expand(interpolatedString, container, null);

    internal static string Expand(InterpolatedString interpolatedString, ITokenValueContainer container, ExpanderValueFormatter? formatter = null)
    {
        Guard.NotNull(interpolatedString, nameof(interpolatedString));
        Guard.NotNull(container, nameof(container));

        var settings = Guard.NotNull(interpolatedString.Settings, nameof(interpolatedString.Settings)).Validate();
        formatter ??= new ExpanderValueFormatter(interpolatedString.Settings.FormatterDefinitions, interpolatedString.Settings.NameComparer);

        var iterator = new ExpandedStringIterator(interpolatedString.Segments);
        var builder = new ExpandedStringBuilder(formatter, settings.FormatProvider);
        var allCommands = settings.Commands.Concat(standardCommands).ToList();
        var context = new ExpanderContext(iterator, builder, container, settings, allCommands);
        InitCommands(context);
        IterateSegments(context);
        FinishCommands(context);
        return builder.ExpandedString();
    }

    private static void InitCommands(ExpanderContext context)
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
            context.SegmentHandled = false;

            foreach (var command in context.Commands)
            {
                command.Evaluate(context);
                if (context.SegmentHandled) { break; }
            }
            if (context.SegmentHandled) { continue; }

            throw new ExpanderException($"Unhandled segment type {context.SegmentIterator.Current}");
        }
    }

    private static void FinishCommands(ExpanderContext context)
    {
        foreach (var command in context.Commands)
        {
            command.Finished(context);
        }
    }
}

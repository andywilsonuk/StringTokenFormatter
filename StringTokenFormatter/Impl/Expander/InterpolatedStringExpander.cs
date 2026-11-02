namespace StringTokenFormatter.Impl;

public static class InterpolatedStringExpander
{
    public static string Expand(InterpolatedString interpolatedString, ITokenValueContainer container) => Expand(interpolatedString, container, null);

    internal static string Expand(InterpolatedString interpolatedString, ITokenValueContainer container, ExpanderValueFormatter? formatter = null)
    {
        Guard.NotNull(interpolatedString, nameof(interpolatedString));
        Guard.NotNull(container, nameof(container));

        IInterpolatedStringSettings settings = Guard.NotNull(interpolatedString.Settings, nameof(interpolatedString.Settings)).Validate();
        formatter ??= new ExpanderValueFormatter(interpolatedString.Settings.FormatterDefinitions, interpolatedString.Settings.NameComparer);

        ExpandedStringIterator iterator = new(interpolatedString.Segments);
        ExpandedStringBuilder builder = new(formatter, settings.FormatProvider);
        ExpanderCommands commands = new(settings.Commands);
        ExpanderContext context = new(iterator, builder, container, settings, commands);
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

            throw new ExpanderException($"Unhandled segment '{context.SegmentIterator.Current.Raw}'");
        }
    }
}

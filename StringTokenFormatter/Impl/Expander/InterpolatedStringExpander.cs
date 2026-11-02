namespace StringTokenFormatter.Impl;

public static class InterpolatedStringExpander
{
    public static string Expand(InterpolatedString interpolatedString, ITokenValueContainer container) => Expand(interpolatedString, container, null, null);

    public static string Expand(InterpolatedString interpolatedString, ITokenValueContainer container, InterpolatedStringResolverTrace trace) =>
        Expand(interpolatedString, container, null, Guard.NotNull(trace, nameof(trace)));

    internal static string Expand(InterpolatedString interpolatedString, ITokenValueContainer container, ExpanderValueFormatter? formatter = null, InterpolatedStringResolverTrace? trace = null)
    {
        Guard.NotNull(interpolatedString, nameof(interpolatedString));
        Guard.NotNull(container, nameof(container));

        IInterpolatedStringSettings settings = Guard.NotNull(interpolatedString.Settings, nameof(interpolatedString.Settings)).Validate();
        formatter ??= new ExpanderValueFormatter(interpolatedString.Settings.FormatterDefinitions, interpolatedString.Settings.NameComparer);
        trace ??= InterpolatedStringResolverTrace.NoOp;

        ExpandedStringIterator iterator = new(interpolatedString.Segments);
        ExpandedStringBuilder builder = new(formatter, settings.FormatProvider);
        ExpanderCommands commands = new(settings.Commands);
        ExpanderContext context = new(iterator, builder, container, settings, commands, trace);
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
            context.Trace.RecordSegmentStarted(context.SegmentIterator.CurrentIndex, context.SegmentIterator.Current);
            IExpanderCommand? handledBy = null;
            try
            {
                context.Commands.ExecuteUntil(command =>
                {
                    command.Evaluate(context);
                    if (context.SegmentHandled) { handledBy = command; }
                }, () => context.SegmentHandled);
                if (!context.SegmentHandled)
                {
                    throw new ExpanderException($"Unhandled segment '{context.SegmentIterator.Current.Raw}'");
                }
                context.Trace.RecordSegmentCompleted(context.SegmentIterator.CurrentIndex, context.SegmentIterator.Current, handledBy);
            }
            catch (Exception ex)
            {
                context.Trace.RecordSegmentFailed(context.SegmentIterator.CurrentIndex, context.SegmentIterator.Current, handledBy, ex.GetType().Name);
                throw;
            }
        }
    }
}

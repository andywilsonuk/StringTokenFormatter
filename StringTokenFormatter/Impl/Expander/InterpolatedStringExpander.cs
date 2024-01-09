namespace StringTokenFormatter.Impl;

public static class InterpolatedStringExpander
{
    public static string Expand(InterpolatedString interpolatedString, ITokenValueContainer container)
    {
        Guard.NotNull(interpolatedString, nameof(interpolatedString));
        Guard.NotNull(container, nameof(container));

        var settings = interpolatedString.Settings;
        var builder = new ExpandedStringBuilder(settings.FormatProvider);
        var definedCommands = new List<ICommandBlock>()
        {
            new ConditionalBlock(),
        };
        var commands = definedCommands.Select(x => (Key: x.StartCommandName, Value: x))
            .Concat(definedCommands.Select(x => (Key: x.EndCommandName, Value: x)))
            .ToDictionary(x => x.Key, x => x.Value, InterpolatedStringBlockSegment.CommandComparer);
        // TODO: handle duplicate keys
        var context = new ExpanderContext(builder, container, settings, commands);

        var enumerator = interpolatedString.Segments.GetEnumerator();
        while (enumerator.MoveNext())
        {
            context.SkipActiveBlocks = false;
            context.CurrentSegment = enumerator.Current;

            foreach (var command in context.ActiveBlocks)
            {
                command.Evaluate(context);
                if (context.SkipActiveBlocks) { break; }
            }
            if (!context.SkipActiveBlocks)
            {
                context.EvaluateCurrentSegment();
            }
        }
        if (context.ActiveBlocks.Count > 0)
        {
            throw new ExpanderException($"Missing end command {context.ActiveBlocks.Last().EndCommandName}");
        }
        return builder.ExpandedString();
    }
}

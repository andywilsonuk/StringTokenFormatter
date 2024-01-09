namespace StringTokenFormatter.Impl;

public static class InterpolatedStringExpander
{
    public static string Expand(InterpolatedString interpolatedString, ITokenValueContainer container)
    {
        Guard.NotNull(interpolatedString, nameof(interpolatedString));
        Guard.NotNull(container, nameof(container));

        var settings = interpolatedString.Settings;
        var builder = new ExpandedStringBuilder(settings.FormatProvider);
        var commands = BuildCommandsLookup(settings.BlockCommands);
        var context = new ExpanderContext(builder, container, settings, commands);
        IterateSegments(interpolatedString, context);
        return builder.ExpandedString();
    }

    private static Dictionary<string, IBlockCommand> BuildCommandsLookup(IEnumerable<IBlockCommand> definedCommands)
    {
        var lookup = new Dictionary<string, IBlockCommand>(InterpolatedStringBlockSegment.CommandComparer);
        foreach (var command in definedCommands)
        {
            if (lookup.ContainsKey(command.StartCommandName))
            {
                throw new ExpanderException($"Command block with start {command.StartCommandName} has already been defined");
            }
            if (lookup.ContainsKey(command.EndCommandName))
            {
                throw new ExpanderException($"Command block with end {command.EndCommandName} has already been defined");
            }
            lookup.Add(command.StartCommandName, command);
            lookup.Add(command.EndCommandName, command);
        }
        return lookup;
    }

    private static void IterateSegments(InterpolatedString interpolatedString, ExpanderContext context)
    {
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
    }
}

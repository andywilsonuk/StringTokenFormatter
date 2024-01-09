namespace StringTokenFormatter.Impl;

public static class ExpanderContextExtensions
{
    public static bool TryGetTokenValue(this ExpanderContext context, string token, out object? value)
    {
        var containerMatch = context.Container.TryMap(token);
        if (containerMatch.IsSuccess)
        {
            var containerValue = containerMatch.Value;
            var converter = context.Settings.ValueConverters.Select(fn => fn(containerValue, token)).FirstOrDefault(x => x.IsSuccess);
            if (converter != default)
            {
                value = converter.Value;
                return true;
            }
            throw new MissingValueConverterException($"No matching value converter found for token '{token}' with container value {containerValue}");
        }
        else if (context.Settings.UnresolvedTokenBehavior == UnresolvedTokenBehavior.Throw)
        {
            throw new UnresolvedTokenException($"Token '{token}' was not found within the container");
        }
        value = null;
        return false;
    }

    public static void EvaluateCurrentSegment(this ExpanderContext context)
    {
        var segment = context.CurrentSegment ?? throw new ExpanderException("Context current segment is expected not to be null");
        if (segment is InterpolatedStringTokenSegment tokenSegment)
        {
            context.StringBuilder.AppendTokenValue(context, tokenSegment);
            return;
        }
        if (segment is InterpolatedStringBlockSegment blockSegment)
        {
            if (!context.DefinedCommands.TryGetValue(blockSegment.Command, out var command))
            {
                throw new ExpanderException($"Command {blockSegment.Command} has not been defined");
            }
            if (command.StartCommandName == blockSegment.Command)
            {
                command.Start(context, blockSegment);
                context.ActiveBlocks.Push(command);
            }
            else
            {
                var lastCommand = context.ActiveBlocks.Pop();
                if (command != lastCommand) { throw new ExpanderException($"Command {command.EndCommandName} does not match expected command {lastCommand.EndCommandName}"); }
                command.End(context, blockSegment);
            }
            return;
        }
        context.StringBuilder.AppendLiteral(segment.Raw);
    }
}
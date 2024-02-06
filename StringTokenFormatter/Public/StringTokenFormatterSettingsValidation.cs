namespace StringTokenFormatter;

public static class StringTokenFormatterSettingsExtensions
{
    /// <summary>
    /// Asserts that the settings are properly configured
    /// </summary>
    public static IInterpolatedStringSettings Validate(this IInterpolatedStringSettings settings)
    {
        Guard.NotNull(settings, nameof(settings));
        Guard.NotNull(settings.Syntax, nameof(settings.Syntax)).Validate();
        Guard.IsDefined(settings.UnresolvedTokenBehavior, nameof(settings.UnresolvedTokenBehavior));
        Guard.NotEmpty(settings.ValueConverters, nameof(settings.ValueConverters));
        Guard.NotNull(settings.FormatProvider, nameof(settings.FormatProvider));
        Guard.IsDefined(settings.InvalidFormatBehavior, nameof(settings.InvalidFormatBehavior));
        Guard.NotNull(settings.BlockCommands, nameof(settings.BlockCommands));
        Guard.NotNull(settings.NameComparer, nameof(settings.NameComparer));
        Guard.NotNull(settings.FormatterDefinitions, nameof(settings.FormatterDefinitions));
        return settings;
    }

    /// <summary>
    /// Asserts that the settings are properly configured
    /// </summary>
    public static ITokenValueContainerSettings Validate(this ITokenValueContainerSettings settings)
    {
        Guard.NotNull(settings, nameof(settings));
        Guard.NotNull(settings.NameComparer, nameof(settings.NameComparer));
        Guard.IsDefined(settings.TokenResolutionPolicy, nameof(settings.TokenResolutionPolicy));
        return settings;
    }

    /// <summary>
    /// Asserts that the settings are properly configured
    /// </summary>
    public static IHierarchicalTokenValueContainerSettings Validate(this IHierarchicalTokenValueContainerSettings settings)
    {
        Guard.NotNull(settings, nameof(settings));
        Guard.NotEmpty(settings.HierarchicalDelimiter, nameof(settings.HierarchicalDelimiter));
        return settings;
    }

    /// <summary>
    /// Asserts that the settings are properly configured
    /// </summary>
    public static StringTokenFormatterSettings Validate(this StringTokenFormatterSettings settings)
    {
        Validate((IInterpolatedStringSettings)settings);
        Validate((ITokenValueContainerSettings)settings);
        Validate((IHierarchicalTokenValueContainerSettings)settings);
        return settings;
    }
}

namespace StringTokenFormatter;

public static class StringTokenFormatterSettingsExtensions
{
    extension(IInterpolatedStringSettings settings)
    {
        /// <summary>
        /// Asserts that the settings are properly configured
        /// </summary>
        public IInterpolatedStringSettings Validate()
        {
            Guard.NotNull(settings, nameof(settings));
            Guard.NotNull(settings.Syntax, nameof(settings.Syntax)).Validate();
            Guard.IsDefined(settings.UnresolvedTokenBehavior, nameof(settings.UnresolvedTokenBehavior));
            if (settings is IUnresolvedTokenReplacementSettings replacementSettings)
            {
                Guard.NotNull(replacementSettings.UnresolvedTokenReplacement, nameof(replacementSettings.UnresolvedTokenReplacement));
                Guard.NotNull(replacementSettings.UnresolvedTokenReplacementFormatter, nameof(replacementSettings.UnresolvedTokenReplacementFormatter));
            }
            Guard.NotEmpty(settings.ValueConverters, nameof(settings.ValueConverters));
            Guard.NotNull(settings.FormatProvider, nameof(settings.FormatProvider));
            Guard.IsDefined(settings.InvalidFormatBehavior, nameof(settings.InvalidFormatBehavior));
            Guard.NotNull(settings.Commands, nameof(settings.Commands));
            Guard.NotNull(settings.NameComparer, nameof(settings.NameComparer));
            Guard.NotNull(settings.FormatterDefinitions, nameof(settings.FormatterDefinitions));
            if (new HashSet<int>(settings.FormatterDefinitions.Select(x => x.GetHashCode())).Count != settings.FormatterDefinitions.Count) { throw new ArgumentException($"Duplicate Formatter Definition detected"); }
            return settings;
        }
    }

    extension(ITokenValueContainerSettings settings)
    {
        /// <summary>
        /// Asserts that the settings are properly configured
        /// </summary>
        public ITokenValueContainerSettings Validate()
        {
            Guard.NotNull(settings, nameof(settings));
            Guard.NotNull(settings.NameComparer, nameof(settings.NameComparer));
            if (settings is INestedTokenValueContainerSettings nestedSettings)
            {
                Guard.IsDefined(nestedSettings.ResolveNestedValues, nameof(nestedSettings.ResolveNestedValues));
            }
            return settings;
        }
    }

    extension(ICompositeTokenValueContainerSettings settings)
    {
        /// <summary>
        /// Asserts that the settings are properly configured
        /// </summary>
        public ICompositeTokenValueContainerSettings Validate()
        {
            Guard.IsDefined(settings.TokenResolutionPolicy, nameof(settings.TokenResolutionPolicy));
            return settings;
        }
    }

    extension(IHierarchicalTokenValueContainerSettings settings)
    {
        /// <summary>
        /// Asserts that the settings are properly configured
        /// </summary>
        public IHierarchicalTokenValueContainerSettings Validate()
        {
            Guard.NotNull(settings, nameof(settings));
            Guard.NotEmpty(settings.HierarchicalDelimiter, nameof(settings.HierarchicalDelimiter));
            return settings;
        }
    }

    extension(StringTokenFormatterSettings settings)
    {
        /// <summary>
        /// Asserts that the settings are properly configured
        /// </summary>
        public StringTokenFormatterSettings Validate()
        {
            Validate((IInterpolatedStringSettings)settings);
            Validate((ITokenValueContainerSettings)settings);
            Validate((ICompositeTokenValueContainerSettings)settings);
            Validate((IHierarchicalTokenValueContainerSettings)settings);
            return settings;
        }
    }
}

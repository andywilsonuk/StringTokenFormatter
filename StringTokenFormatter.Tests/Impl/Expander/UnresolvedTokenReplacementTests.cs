namespace StringTokenFormatter.Tests;

public class UnresolvedTokenReplacementTests
{
    [Fact]
    public void DefaultSettings_UnresolvedTokenStillThrows()
    {
        var settings = StringTokenFormatterSettings.Default;

        Assert.Equal(UnresolvedTokenBehavior.Throw, settings.UnresolvedTokenBehavior);
        Assert.Empty(settings.UnresolvedTokenReplacement);
        Assert.Throws<UnresolvedTokenException>(() => Expand("{missing}", settings));
    }

    [Theory]
    [InlineData(UnresolvedTokenBehavior.Throw)]
    [InlineData(UnresolvedTokenBehavior.LeaveUnresolved)]
    [InlineData(UnresolvedTokenBehavior.Replace)]
    public void DefaultSelector_ReturnsConfiguredBehavior(UnresolvedTokenBehavior behavior)
    {
        var selector = StringTokenFormatterSettings.Default.UnresolvedTokenReplacementFormatter;

        Assert.Equal<UnresolvedTokenBehavior?>(behavior, selector("missing", behavior));
    }

    [Fact]
    public void ReplaceWithDefaultReplacement_OutputsEmptyString()
    {
        var settings = StringTokenFormatterSettings.Default with
        {
            UnresolvedTokenBehavior = UnresolvedTokenBehavior.Replace,
        };

        var actual = Expand("before {missing} after", settings);

        Assert.Equal("before  after", actual);
    }

    [Theory]
    [InlineData("fallback")]
    [InlineData("{another}")]
    public void ReplaceWithCustomReplacement_OutputsReplacementLiterally(string replacement)
    {
        var settings = StringTokenFormatterSettings.Default with
        {
            UnresolvedTokenBehavior = UnresolvedTokenBehavior.Replace,
            UnresolvedTokenReplacement = replacement,
        };

        var actual = Expand("before {missing} after", settings);

        Assert.Equal("before " + replacement + " after", actual);
    }

    [Theory]
    [InlineData("replace", "fallback")]
    [InlineData("leave", "{leave}")]
    [InlineData("throw", null)]
    public void PerTokenSelector_ChoosesReplacementLeaveOrThrow(string tokenName, string? expected)
    {
        var calls = new List<string>();
        var settings = StringTokenFormatterSettings.Default with
        {
            UnresolvedTokenBehavior = UnresolvedTokenBehavior.LeaveUnresolved,
            UnresolvedTokenReplacement = "fallback",
            UnresolvedTokenReplacementFormatter = (token, defaultBehavior) =>
            {
                calls.Add(token);
                Assert.Equal(UnresolvedTokenBehavior.LeaveUnresolved, defaultBehavior);
                return token switch
                {
                    "replace" => UnresolvedTokenBehavior.Replace,
                    "leave" => UnresolvedTokenBehavior.LeaveUnresolved,
                    "throw" => UnresolvedTokenBehavior.Throw,
                    _ => null,
                };
            },
        };

        if (expected == null)
        {
            var exception = Assert.Throws<UnresolvedTokenException>(() => Expand("{" + tokenName + "}", settings));
            Assert.Contains(tokenName, exception.Message);
        }
        else
        {
            Assert.Equal(expected, Expand("{" + tokenName + "}", settings));
        }
        Assert.Equal(new[] { tokenName }, calls);
    }

    [Fact]
    public void PerTokenSelector_OverridesGlobalReplaceWithinOneTemplate()
    {
        var settings = StringTokenFormatterSettings.Default with
        {
            UnresolvedTokenBehavior = UnresolvedTokenBehavior.Replace,
            UnresolvedTokenReplacement = "fallback",
            UnresolvedTokenReplacementFormatter = (token, _) => token == "leave"
                ? UnresolvedTokenBehavior.LeaveUnresolved
                : null,
        };

        var actual = Expand("{replace}|{leave}", settings);

        Assert.Equal("fallback|{leave}", actual);
    }

    [Theory]
    [InlineData(UnresolvedTokenBehavior.Throw, null)]
    [InlineData(UnresolvedTokenBehavior.LeaveUnresolved, "{missing}")]
    [InlineData(UnresolvedTokenBehavior.Replace, "fallback")]
    public void NullSelectorResult_UsesConfiguredDefault(UnresolvedTokenBehavior behavior, string? expected)
    {
        var calls = 0;
        var settings = StringTokenFormatterSettings.Default with
        {
            UnresolvedTokenBehavior = behavior,
            UnresolvedTokenReplacement = "fallback",
            UnresolvedTokenReplacementFormatter = (token, defaultBehavior) =>
            {
                calls++;
                Assert.Equal("missing", token);
                Assert.Equal(behavior, defaultBehavior);
                return null;
            },
        };

        if (expected == null)
        {
            Assert.Throws<UnresolvedTokenException>(() => Expand("{missing}", settings));
        }
        else
        {
            Assert.Equal(expected, Expand("{missing}", settings));
        }
        Assert.Equal(1, calls);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("resolved")]
    public void ResolvedToken_DoesNotInvokeSelector(string? value)
    {
        var settings = StringTokenFormatterSettings.Default with
        {
            UnresolvedTokenBehavior = UnresolvedTokenBehavior.Replace,
            UnresolvedTokenReplacementFormatter = (_, _) => throw new InvalidOperationException("Selector must not run"),
        };
        var container = new BasicContainer().Add("value", value);

        var actual = Expand("{value}", settings, container);

        Assert.Equal(value ?? string.Empty, actual);
    }

    [Fact]
    public void LegacySettings_ResolvedTokensRenderWithoutReplacementProperties()
    {
        IInterpolatedStringSettings settings = new LegacySettings();
        var container = new BasicContainer().Add("value", 42);

        var actual = Expand("{value:D4}", settings, container);

        Assert.False(settings is IUnresolvedTokenReplacementSettings);
        Assert.Equal("0042", actual);
        Assert.Throws<UnresolvedTokenException>(() => Expand("{missing}", settings));
    }

    [Fact]
    public void LegacySettings_LeaveUnresolvedStillOutputsRawToken()
    {
        IInterpolatedStringSettings settings = new LegacySettings
        {
            UnresolvedTokenBehavior = UnresolvedTokenBehavior.LeaveUnresolved,
        };

        var actual = Expand("{missing,8:format}", settings);

        Assert.Equal("{missing,8:format}", actual);
    }

    [Fact]
    public void LegacySettings_SelectingReplaceUsesEmptyReplacement()
    {
        IInterpolatedStringSettings settings = new LegacySettings
        {
            UnresolvedTokenBehavior = UnresolvedTokenBehavior.Replace,
        };

        var actual = Expand("before {missing} after", settings);

        Assert.False(settings is IUnresolvedTokenReplacementSettings);
        Assert.Equal("before  after", actual);
    }

    [Fact]
    public void CustomOptionalSettings_UsesReplacementAndSelector()
    {
        IInterpolatedStringSettings settings = new ReplacementSettings
        {
            UnresolvedTokenReplacement = "custom",
            UnresolvedTokenReplacementFormatter = (_, _) => UnresolvedTokenBehavior.Replace,
        };

        var actual = Expand("{missing}", settings);

        Assert.Equal("custom", actual);
    }

    [Fact]
    public void NullReplacement_ValidationThrowsForConcreteAndInterfaceSettings()
    {
        var settings = StringTokenFormatterSettings.Default with
        {
            UnresolvedTokenReplacement = null!,
        };

        var concreteException = Assert.Throws<ArgumentException>(() => settings.Validate());
        var interfaceException = Assert.Throws<ArgumentException>(() => ((IInterpolatedStringSettings)settings).Validate());

        Assert.Equal(nameof(settings.UnresolvedTokenReplacement), concreteException.ParamName);
        Assert.Equal(nameof(settings.UnresolvedTokenReplacement), interfaceException.ParamName);
    }

    [Fact]
    public void NullSelector_ValidationThrowsForConcreteAndInterfaceSettings()
    {
        var settings = StringTokenFormatterSettings.Default with
        {
            UnresolvedTokenReplacementFormatter = null!,
        };

        var concreteException = Assert.Throws<ArgumentException>(() => settings.Validate());
        var interfaceException = Assert.Throws<ArgumentException>(() => ((IInterpolatedStringSettings)settings).Validate());

        Assert.Equal(nameof(settings.UnresolvedTokenReplacementFormatter), concreteException.ParamName);
        Assert.Equal(nameof(settings.UnresolvedTokenReplacementFormatter), interfaceException.ParamName);
    }

    [Theory]
    [InlineData(true, "UnresolvedTokenReplacement")]
    [InlineData(false, "UnresolvedTokenReplacementFormatter")]
    public void InvalidCustomOptionalSettings_ValidationThrows(bool nullReplacement, string expectedParameter)
    {
        IInterpolatedStringSettings settings = new ReplacementSettings
        {
            UnresolvedTokenReplacement = nullReplacement ? null! : string.Empty,
            UnresolvedTokenReplacementFormatter = nullReplacement ? (_, behavior) => behavior : null!,
        };

        var exception = Assert.Throws<ArgumentException>(() => settings.Validate());

        Assert.Equal(expectedParameter, exception.ParamName);
    }

    [Fact]
    public void Validation_DoesNotInvokeSelector()
    {
        var settings = StringTokenFormatterSettings.Default with
        {
            UnresolvedTokenReplacementFormatter = (_, _) => throw new InvalidOperationException("Selector must not run"),
        };

        Assert.Same(settings, settings.Validate());
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(100)]
    public void InvalidSelectorResult_ThrowsArgumentException(int value)
    {
        var settings = StringTokenFormatterSettings.Default with
        {
            UnresolvedTokenReplacementFormatter = (_, _) => (UnresolvedTokenBehavior)value,
        };

        var exception = Assert.Throws<ArgumentException>(() => Expand("{missing}", settings));

        Assert.Equal(nameof(settings.UnresolvedTokenReplacementFormatter), exception.ParamName);
    }

    [Theory]
    [InlineData("{missing,4}", "   x")]
    [InlineData("{missing,-4}", "x   ")]
    [InlineData("{missing:any}", "x")]
    public void Replacement_UsesStandardFormattingAndAlignment(string source, string expected)
    {
        var settings = StringTokenFormatterSettings.Default with
        {
            UnresolvedTokenBehavior = UnresolvedTokenBehavior.Replace,
            UnresolvedTokenReplacement = "x",
        };

        var actual = Expand(source, settings);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Replacement_UsesCustomFormatterBeforeAlignment()
    {
        var settings = StringTokenFormatterSettings.Default with
        {
            UnresolvedTokenBehavior = UnresolvedTokenBehavior.Replace,
            UnresolvedTokenReplacement = "fallback",
            FormatterDefinitions = new[]
            {
                FormatterDefinition.ForTokenNameAndFormatString("missing", "upper",
                    (string value, string _) => value.ToUpperInvariant()),
            },
        };

        var actual = Expand("{missing,10:upper}", settings);

        Assert.Equal("  FALLBACK", actual);
    }

    [Fact]
    public void EmptyReplacement_IsStillAlignedAsResolvedValue()
    {
        var settings = StringTokenFormatterSettings.Default with
        {
            UnresolvedTokenBehavior = UnresolvedTokenBehavior.Replace,
        };

        var actual = Expand("before{missing,4}after", settings);

        Assert.Equal("before    after", actual);
    }

    [Theory]
    [InlineData(TokenResolutionPolicy.ResolveAll, null, "", 0)]
    [InlineData(TokenResolutionPolicy.ResolveAll, "", "", 0)]
    [InlineData(TokenResolutionPolicy.IgnoreNull, null, "fallback", 1)]
    [InlineData(TokenResolutionPolicy.IgnoreNull, "", "", 0)]
    [InlineData(TokenResolutionPolicy.IgnoreNullOrEmpty, null, "fallback", 1)]
    [InlineData(TokenResolutionPolicy.IgnoreNullOrEmpty, "", "fallback", 1)]
    [InlineData(TokenResolutionPolicy.IgnoreNullOrEmpty, "resolved", "resolved", 0)]
    public void CompositeFiltering_InvokesSelectorOnlyWhenNoValueSatisfiesPolicy(
        TokenResolutionPolicy policy, string? value, string expected, int expectedCalls)
    {
        var calls = new List<string>();
        var settings = StringTokenFormatterSettings.Default with
        {
            TokenResolutionPolicy = policy,
            UnresolvedTokenReplacement = "fallback",
            UnresolvedTokenReplacementFormatter = (token, _) =>
            {
                calls.Add(token);
                return UnresolvedTokenBehavior.Replace;
            },
        };
        var container = TokenValueContainerFactory.FromCombination(settings,
            new BasicContainer().Add("value", value), new BasicContainer());

        var actual = Expand("{value}", settings, container);

        Assert.Equal(expected, actual);
        Assert.Equal(expectedCalls, calls.Count);
        Assert.All(calls, token => Assert.Equal("value", token));
    }

    [Fact]
    public void CompositeFiltering_LaterValidContainerPrecedesReplacement()
    {
        var settings = StringTokenFormatterSettings.Default with
        {
            TokenResolutionPolicy = TokenResolutionPolicy.IgnoreNullOrEmpty,
            UnresolvedTokenReplacementFormatter = (_, _) => throw new InvalidOperationException("Selector must not run"),
        };
        var container = TokenValueContainerFactory.FromCombination(settings,
            new BasicContainer().Add("value", null),
            new BasicContainer().Add("value", string.Empty),
            new BasicContainer().Add("value", "last"));

        var actual = Expand("{value}", settings, container);

        Assert.Equal("last", actual);
    }

    [Fact]
    public void CompositeFiltering_SelectorRunsAfterEveryContainerIsTried()
    {
        var attempts = new List<string>();
        var settings = StringTokenFormatterSettings.Default with
        {
            TokenResolutionPolicy = TokenResolutionPolicy.IgnoreNull,
            UnresolvedTokenReplacement = "fallback",
            UnresolvedTokenReplacementFormatter = (_, _) =>
            {
                attempts.Add("selector");
                return UnresolvedTokenBehavior.Replace;
            },
        };
        var first = TokenValueContainerFactory.FromFunc<object?>(settings, _ =>
        {
            attempts.Add("first");
            return null;
        });
        var second = TokenValueContainerFactory.FromFunc<object?>(settings, _ =>
        {
            attempts.Add("second");
            return null;
        });
        var container = TokenValueContainerFactory.FromCombination(settings, first, second);

        var actual = Expand("{missing}", settings, container);

        Assert.Equal("fallback", actual);
        Assert.Equal(new[] { "first", "second", "selector" }, attempts);
    }

    [Fact]
    public void LoopSequenceAndPseudoTokens_InvokeSelectorOnlyForMissingChild()
    {
        var calls = new List<string>();
        var settings = StringTokenFormatterSettings.Default with
        {
            UnresolvedTokenReplacement = "fallback",
            UnresolvedTokenReplacementFormatter = (token, _) =>
            {
                calls.Add(token);
                return UnresolvedTokenBehavior.Replace;
            },
        };
        var sequence = TokenValueContainerFactory.FromSequence(settings, "Iterator", new[]
        {
            new BasicContainer().Add("Name", "first"),
            new BasicContainer(),
        });
        var container = TokenValueContainerFactory.FromCombination(settings, sequence);

        var actual = Expand("{:loop,Iterator}{::loopiteration}{Iterator.Name};{:loopend}", settings, container);

        Assert.Equal("1first;2fallback;", actual);
        Assert.Equal(new[] { "Iterator.Name" }, calls);
    }

    [Fact]
    public void UnknownPseudoToken_UsesSelectorWithFullTokenName()
    {
        var calls = new List<string>();
        var settings = StringTokenFormatterSettings.Default with
        {
            UnresolvedTokenReplacement = "fallback",
            UnresolvedTokenReplacementFormatter = (token, _) =>
            {
                calls.Add(token);
                return UnresolvedTokenBehavior.Replace;
            },
        };

        var actual = Expand("{::unknown}", settings);

        Assert.Equal("fallback", actual);
        Assert.Equal(new[] { "::unknown" }, calls);
    }

    [Fact]
    public void MapCommand_UsesReplacementAsInputToMapping()
    {
        var settings = StringTokenFormatterSettings.Default with
        {
            UnresolvedTokenBehavior = UnresolvedTokenBehavior.Replace,
            UnresolvedTokenReplacement = "fallback",
        };

        var actual = Expand("{:map,missing:fallback=matched}", settings);

        Assert.Equal("matched", actual);
    }

    [Fact]
    public void UnresolvedLoopHeader_PreservesExistingCommandBehavior()
    {
        var settings = StringTokenFormatterSettings.Default with
        {
            UnresolvedTokenBehavior = UnresolvedTokenBehavior.Replace,
            UnresolvedTokenReplacementFormatter = (_, _) => throw new InvalidOperationException("Selector must not run"),
        };

        var actual = Expand("{:loop,missing}body{:loopend}", settings);

        Assert.Equal("{:loop,missing}", actual);
    }

    [Theory]
    [InlineData("true", "yes|")]
    [InlineData("false", "|no")]
    public void ConditionalCommands_ConvertReplacementBeforeApplyingNegation(string replacement, string expected)
    {
        var selectorCalls = new List<string>();
        var converterCalls = new List<string>();
        TokenValueConverter converter = (value, tokenName) =>
        {
            Assert.Equal("missing", tokenName);
            Assert.Equal(replacement, value);
            converterCalls.Add(tokenName);
            return TryGetResult.Success(bool.Parse((string)value!));
        };
        var settings = StringTokenFormatterSettings.Default with
        {
            UnresolvedTokenReplacement = replacement,
            UnresolvedTokenReplacementFormatter = (tokenName, _) =>
            {
                selectorCalls.Add(tokenName);
                return UnresolvedTokenBehavior.Replace;
            },
            ValueConverters = new[] { converter },
        };

        var actual = Expand("{:if,missing}yes{:ifend}|{:if,!missing}no{:ifend}", settings);

        Assert.Equal(expected, actual);
        Assert.Equal(new[] { "missing", "missing" }, selectorCalls);
        Assert.Equal(new[] { "missing", "missing" }, converterCalls);
    }

    private static string Expand(string source, IInterpolatedStringSettings settings, ITokenValueContainer? container = null) =>
        InterpolatedStringExpander.Expand(InterpolatedStringParser.Parse(source, settings), container ?? new BasicContainer());

    private class LegacySettings : IInterpolatedStringSettings
    {
        public TokenSyntax Syntax => StringTokenFormatterSettings.Default.Syntax;
        public UnresolvedTokenBehavior UnresolvedTokenBehavior { get; set; } = UnresolvedTokenBehavior.Throw;
        public IReadOnlyCollection<TokenValueConverter> ValueConverters => StringTokenFormatterSettings.Default.ValueConverters;
        public IFormatProvider FormatProvider => CultureInfo.InvariantCulture;
        public InvalidFormatBehavior InvalidFormatBehavior => InvalidFormatBehavior.Throw;
        public IReadOnlyCollection<IExpanderCommand> Commands => StringTokenFormatterSettings.Default.Commands;
        public StringComparer NameComparer => StringComparer.OrdinalIgnoreCase;
        public IReadOnlyCollection<FormatterDefinition> FormatterDefinitions => StringTokenFormatterSettings.Default.FormatterDefinitions;
    }

    private sealed class ReplacementSettings : LegacySettings, IUnresolvedTokenReplacementSettings
    {
        public string UnresolvedTokenReplacement { get; set; } = string.Empty;
        public Func<string, UnresolvedTokenBehavior, UnresolvedTokenBehavior?> UnresolvedTokenReplacementFormatter { get; set; } = (_, behavior) => behavior;
    }
}

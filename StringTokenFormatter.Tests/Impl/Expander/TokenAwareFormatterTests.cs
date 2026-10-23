using System.Reflection;

namespace StringTokenFormatter.Tests;

public class TokenAwareFormatterTests
{
    [Theory]
    [InlineData("Type")]
    [InlineData("TokenName")]
    [InlineData("FormatString")]
    [InlineData("TokenNameAndFormatString")]
    public void TokenAwareFactory_ReceivesValueFormatStringAndActualTokenName(string factory)
    {
        var definition = CreateDefinition(factory, (int value, string formatString, string tokenName) =>
            $"{value}:{formatString}:{tokenName}");

        var actual = Expand(definition);

        Assert.Equal("42:custom:amount", actual);
    }

    [Theory]
    [InlineData("Type")]
    [InlineData("TokenName")]
    [InlineData("FormatString")]
    [InlineData("TokenNameAndFormatString")]
    public void LegacyFactory_StillReceivesValueAndFormatString(string factory)
    {
        var definition = CreateDefinition(factory, (int value, string formatString) => $"{value}:{formatString}");

        var actual = Expand(definition);

        Assert.Equal("42:custom", actual);
    }

    [Theory]
    [InlineData("Type")]
    [InlineData("TokenName")]
    [InlineData("FormatString")]
    [InlineData("TokenNameAndFormatString")]
    public void TokenAwareFactory_RejectsNullCallback(string factory)
    {
        Assert.Throws<ArgumentException>(() =>
            CreateDefinition(factory, (Func<int, string, string, string>)null!));
    }

    [Fact]
    public void TokenAwareCallback_ReceivesEmptyFormatString()
    {
        var definition = FormatterDefinition.ForType((int value, string formatString, string tokenName) =>
            $"{value}:{formatString}:{tokenName}");

        var actual = Expand(definition, tokenName: "other", formatString: string.Empty);

        Assert.Equal("42::other", actual);
    }

    [Theory]
    [InlineData("Type")]
    [InlineData("TokenName")]
    [InlineData("FormatString")]
    [InlineData("TokenNameAndFormatString")]
    public void TokenAwareCallbackReturningNull_ProducesEmptyString(string factory)
    {
        var definition = CreateDefinition(factory, (int value, string formatString, string tokenName) => null!);

        Assert.Empty(Expand(definition));
    }

    [Fact]
    public void LegacyCallbackReturningNull_StillProducesEmptyString()
    {
        var definition = FormatterDefinition.ForType((int value, string formatString) => null!);

        Assert.Empty(Expand(definition));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void MixedCallbackArities_PreserveSelectionPrecedence(bool tokenAwareSpecifics)
    {
        var segments = new SegmentBuilder()
            .Token("plain", string.Empty, string.Empty)
            .Literal("|")
            .Token("amount", string.Empty, string.Empty)
            .Literal("|")
            .Token("plain", string.Empty, "custom")
            .Literal("|")
            .Token("amount", string.Empty, "special")
            .Literal("|")
            .Token("amount", string.Empty, "custom")
            .Build();
        var settings = StringTokenFormatterSettings.Default with
        {
            FormatterDefinitions = new List<FormatterDefinition>
            {
                tokenAwareSpecifics
                    ? FormatterDefinition.ForType((int value, string formatString) => "type")
                    : FormatterDefinition.ForType((int value, string formatString, string tokenName) => "type"),
                tokenAwareSpecifics
                    ? FormatterDefinition.ForTokenName("amount", (int value, string formatString, string tokenName) => "name")
                    : FormatterDefinition.ForTokenName("amount", (int value, string formatString) => "name"),
                tokenAwareSpecifics
                    ? FormatterDefinition.ForFormatString("custom", (int value, string formatString) => "format")
                    : FormatterDefinition.ForFormatString("custom", (int value, string formatString, string tokenName) => "format"),
                tokenAwareSpecifics
                    ? FormatterDefinition.ForTokenNameAndFormatString("amount", "special", (int value, string formatString, string tokenName) => "both")
                    : FormatterDefinition.ForTokenNameAndFormatString("amount", "special", (int value, string formatString) => "both"),
            }
        };
        var values = new BasicContainer();
        values.Add("plain", 42);
        values.Add("amount", 42);
        var interpolatedString = new InterpolatedString(segments, settings);

        var actual = InterpolatedStringExpander.Expand(interpolatedString, values);

        Assert.Equal("type|name|format|both|name", actual);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void EquallySpecificLegacyAndTokenAwareDefinitions_StillFailAsDuplicates(bool tokenAwareFirst)
    {
        var legacy = FormatterDefinition.ForType((int value, string formatString) => "legacy");
        var tokenAware = FormatterDefinition.ForType((int value, string formatString, string tokenName) => "token-aware");
        var settings = StringTokenFormatterSettings.Default with
        {
            FormatterDefinitions = tokenAwareFirst ? new[] { tokenAware, legacy } : new[] { legacy, tokenAware },
        };
        var segments = new SegmentBuilder().Token("amount", string.Empty, "custom").Build();
        var values = new BasicContainer();
        values.Add("amount", 42);

        var exception = Assert.Throws<ArgumentException>(() =>
            InterpolatedStringExpander.Expand(new InterpolatedString(segments, settings), values));

        Assert.Contains("Duplicate Formatter Definition detected", exception.Message);
    }

    [Fact]
    public void NonMatchingTokenAwareDefinition_StillUsesFormatProvider()
    {
        var definition = FormatterDefinition.ForTokenName("other", (int value, string formatString, string tokenName) => "wrong");

        var actual = Expand(definition, formatString: "D4");

        Assert.Equal("0042", actual);
    }

    [Theory]
    [InlineData(false, InvalidFormatBehavior.Throw)]
    [InlineData(false, InvalidFormatBehavior.LeaveUnformatted)]
    [InlineData(false, InvalidFormatBehavior.LeaveToken)]
    [InlineData(true, InvalidFormatBehavior.Throw)]
    [InlineData(true, InvalidFormatBehavior.LeaveUnformatted)]
    [InlineData(true, InvalidFormatBehavior.LeaveToken)]
    public void ThrowingCallback_RespectsInvalidFormatBehavior(bool tokenAware, InvalidFormatBehavior behavior)
    {
        var failure = new InvalidOperationException("Formatter failure");
        var definition = tokenAware
            ? FormatterDefinition.ForType((int value, string formatString, string tokenName) => throw failure)
            : FormatterDefinition.ForType((int value, string formatString) => throw failure);

        if (behavior == InvalidFormatBehavior.Throw)
        {
            var exception = Assert.Throws<TokenValueFormatException>(() => Expand(definition, behavior));
            var formatException = Assert.IsType<FormatException>(exception.InnerException);
            Assert.Same(failure, formatException.InnerException);
        }
        else
        {
            var actual = Expand(definition, behavior);

            Assert.Equal(behavior == InvalidFormatBehavior.LeaveToken ? "{amount:custom}" : "42", actual);
        }
    }

    [Fact]
    public void ClosedStaticLegacyDelegate_UsesDelegateArityInsteadOfTargetMethodArity()
    {
        var method = typeof(TokenAwareFormatterTests).GetMethod(nameof(FormatClosedStaticLegacy), BindingFlags.NonPublic | BindingFlags.Static)!;
        var callback = (Func<int, string, string>)Delegate.CreateDelegate(typeof(Func<int, string, string>), "bound:", method);
        var definition = FormatterDefinition.ForType(callback);
        Assert.Equal(3, callback.GetMethodInfo().GetParameters().Length);

        var actual = Expand(definition);

        Assert.Equal("bound:42:custom", actual);
    }

    [Fact]
    public void ClosedStaticTokenAwareDelegate_UsesDelegateArityInsteadOfTargetMethodArity()
    {
        var method = typeof(TokenAwareFormatterTests).GetMethod(nameof(FormatClosedStaticTokenAware), BindingFlags.NonPublic | BindingFlags.Static)!;
        var callback = (Func<int, string, string, string>)Delegate.CreateDelegate(typeof(Func<int, string, string, string>), "bound:", method);
        var definition = FormatterDefinition.ForType(callback);
        Assert.Equal(4, callback.GetMethodInfo().GetParameters().Length);

        var actual = Expand(definition);

        Assert.Equal("bound:42:custom:amount", actual);
    }

    [Fact]
    public void OpenInstanceTokenAwareDelegate_UsesDelegateArityInsteadOfTargetMethodArity()
    {
        var method = typeof(string).GetMethod(nameof(string.Replace), new[] { typeof(string), typeof(string) })!;
        var callback = (Func<string, string, string, string>)Delegate.CreateDelegate(typeof(Func<string, string, string, string>), method);
        var settings = StringTokenFormatterSettings.Default with
        {
            FormatterDefinitions = new[] { FormatterDefinition.ForType(callback) },
        };
        var segments = new SegmentBuilder().Token("amount", string.Empty, "custom").Build();
        var values = new BasicContainer();
        values.Add("amount", "replace custom");
        Assert.Equal(2, callback.GetMethodInfo().GetParameters().Length);

        var actual = InterpolatedStringExpander.Expand(new InterpolatedString(segments, settings), values);

        Assert.Equal("replace amount", actual);
    }

    [Theory]
    [InlineData(0, InvalidFormatBehavior.Throw)]
    [InlineData(0, InvalidFormatBehavior.LeaveUnformatted)]
    [InlineData(0, InvalidFormatBehavior.LeaveToken)]
    [InlineData(1, InvalidFormatBehavior.Throw)]
    [InlineData(1, InvalidFormatBehavior.LeaveUnformatted)]
    [InlineData(1, InvalidFormatBehavior.LeaveToken)]
    [InlineData(4, InvalidFormatBehavior.Throw)]
    [InlineData(4, InvalidFormatBehavior.LeaveUnformatted)]
    [InlineData(4, InvalidFormatBehavior.LeaveToken)]
    public void UnsupportedDelegateArity_ProducesFormatExceptionAndRespectsInvalidFormatBehavior(int arity, InvalidFormatBehavior behavior)
    {
        Delegate callback = arity switch
        {
            0 => new Func<string>(() => "wrong"),
            1 => new Func<int, string>(value => "wrong"),
            4 => new Func<int, string, string, string, string>((value, formatString, tokenName, extra) => "wrong"),
            _ => throw new ArgumentOutOfRangeException(nameof(arity)),
        };
        var definition = FormatterDefinition.ForType((int value, string formatString) => "unused");
        // The public init accessor can be assigned through reflection without the factory's typed signature.
        typeof(FormatterDefinition).GetProperty(nameof(FormatterDefinition.Formatter))!.SetValue(definition, callback);

        if (behavior == InvalidFormatBehavior.Throw)
        {
            var exception = Assert.Throws<TokenValueFormatException>(() => Expand(definition, behavior));
            var formatException = Assert.IsType<FormatException>(exception.InnerException);
            Assert.Contains("two or three arguments", formatException.Message);
            Assert.Contains($"accepts {arity}", formatException.Message);
        }
        else
        {
            var actual = Expand(definition, behavior);

            Assert.Equal(behavior == InvalidFormatBehavior.LeaveToken ? "{amount:custom}" : "42", actual);
        }
    }

    private static FormatterDefinition CreateDefinition(string factory, Func<int, string, string, string> callback) => factory switch
    {
        "Type" => FormatterDefinition.ForType(callback),
        "TokenName" => FormatterDefinition.ForTokenName("AMOUNT", callback),
        "FormatString" => FormatterDefinition.ForFormatString("CUSTOM", callback),
        "TokenNameAndFormatString" => FormatterDefinition.ForTokenNameAndFormatString("AMOUNT", "CUSTOM", callback),
        _ => throw new ArgumentOutOfRangeException(nameof(factory)),
    };

    private static FormatterDefinition CreateDefinition(string factory, Func<int, string, string> callback) => factory switch
    {
        "Type" => FormatterDefinition.ForType(callback),
        "TokenName" => FormatterDefinition.ForTokenName("AMOUNT", callback),
        "FormatString" => FormatterDefinition.ForFormatString("CUSTOM", callback),
        "TokenNameAndFormatString" => FormatterDefinition.ForTokenNameAndFormatString("AMOUNT", "CUSTOM", callback),
        _ => throw new ArgumentOutOfRangeException(nameof(factory)),
    };

    private static string Expand(FormatterDefinition definition, InvalidFormatBehavior behavior = InvalidFormatBehavior.Throw,
        string tokenName = "amount", string formatString = "custom")
    {
        var settings = StringTokenFormatterSettings.Default with
        {
            FormatterDefinitions = new[] { definition },
            InvalidFormatBehavior = behavior,
            FormatProvider = CultureInfo.InvariantCulture,
            NameComparer = StringComparer.OrdinalIgnoreCase,
        };
        var segments = new SegmentBuilder().Token(tokenName, string.Empty, formatString).Build();
        var values = new BasicContainer();
        values.Add(tokenName, 42);

        return InterpolatedStringExpander.Expand(new InterpolatedString(segments, settings), values);
    }

    private static string FormatClosedStaticLegacy(string prefix, int value, string formatString) =>
        $"{prefix}{value}:{formatString}";

    private static string FormatClosedStaticTokenAware(string prefix, int value, string formatString, string tokenName) =>
        $"{prefix}{value}:{formatString}:{tokenName}";
}

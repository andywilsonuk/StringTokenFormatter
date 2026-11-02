namespace StringTokenFormatter.Tests;

public class StringTokenFormatterSettingsValidationTests
{
    [Theory]
    [InlineData("{", "}", "{")] // escape same as start
    [InlineData("", "}", "{")]
    [InlineData("{", "", "{")]
    [InlineData("{", "}", "")]
    [InlineData("{", "{", "{{")]
    public void BadSyntax_Throws(string start, string end, string escape)
    {
        StringTokenFormatterSettings settings = StringTokenFormatterSettings.Default with
        {
            Syntax = new TokenSyntax(start, end, escape),
        };

        Assert.Throws<ArgumentException>(() => settings.Validate());
    }

    [Fact]
    public void BadUnresolvedTokenBehavior_Throws()
    {
        StringTokenFormatterSettings settings = StringTokenFormatterSettings.Default with
        {
            UnresolvedTokenBehavior = (UnresolvedTokenBehavior)100,
        };

        Assert.Throws<ArgumentException>(() => settings.Validate());
    }

    [Fact]
    public void EmptyValueConverters_Throws()
    {
        StringTokenFormatterSettings settings = StringTokenFormatterSettings.Default with
        {
            ValueConverters = [],
        };

        Assert.Throws<ArgumentException>(() => settings.Validate());
    }

    [Fact]
    public void BadInvalidFormatBehavior_Throws()
    {
        StringTokenFormatterSettings settings = StringTokenFormatterSettings.Default with
        {
            InvalidFormatBehavior = (InvalidFormatBehavior)100,
        };

        Assert.Throws<ArgumentException>(() => settings.Validate());
    }

    [Fact]
    public void BadTokenResolutionPolicy_Throws()
    {
        StringTokenFormatterSettings settings = StringTokenFormatterSettings.Default with
        {
            TokenResolutionPolicy = (TokenResolutionPolicy)100,
        };

        Assert.Throws<ArgumentException>(() => settings.Validate());
    }

    [Fact]
    public void BadHierarchicalDelimiter_Throws()
    {
        StringTokenFormatterSettings settings = StringTokenFormatterSettings.Default with
        {
            HierarchicalDelimiter = string.Empty,
        };

        Assert.Throws<ArgumentException>(() => settings.Validate());
    }

    [Fact]
    public void DuplicateFormatDefinition_Throws()
    {
        StringTokenFormatterSettings settings = StringTokenFormatterSettings.Default with
        {
            FormatterDefinitions = [
                FormatterDefinition.ForType<int>((_1, _2) => "1"),
                FormatterDefinition.ForType<int>((_1, _2) => "2"),
            ],
        };

        Assert.Throws<ArgumentException>(() => settings.Validate());
    }
}
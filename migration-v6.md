# Migration from v6 to v7
Version 7 represents a massive upgrade of the codebase with loads of simplification. Depending on how you were using the library in v6 will determine how easy it is to upgrade. This document lays out common use-cases and their v7 equivalents.

Whilst this document attempts to outline the migration for common use-cases, there will clearly be unexpected use-cases which have not been catered for. Raise an Issue if you're having trouble upgrading.

The library is still targeting .net framework 4.8 and .net 6 which represent the lowest LTS version of those frameworks.

Settings have been rationalized, and there is now a single static Parser and an Expander, the former to convert a string to an `InterpolatedString` and the latter to expand the `InterpolatedString` using values from an `ITokenValueContainer`.

Most of the library now uses the `public` accessor, although some of the classes are in the `StringTokenFormatter.Impl` namespace. This is to allow people to use the library to suit their own needs.

## String and Uri extensions
The overloaded `FormatToken` has been split into it's corresponding use-case; this was to reduce the likelihood of getting compiler errors by slightly misconfiguring the parameters.

Each existing overload has a new overload equivalent which takes the same parameters. `InterpolationSettings` has been replaced with `StringTokenFormatterSettings` which has a completely new structure, see the [settings migration notes](#settings) below.

- `string.FormatToken<T>(T)` -> `FormatFromObject`
- `string.FormatToken<T>(T, InterpolationSettings)` -> `FormatFromObject`
- `string.FormatToken(object)` -> `FormatFromObject`
- `string.FormatToken(object, InterpolationSettings)` -> `FormatFromObject`
- `string.FormatToken(string, object)` -> `FormatFromSingle`
- `string.FormatToken(string, object, IInterpolationSettings)` -> `FormatFromSingle`
- `string.FormatToken<T>(string, T)` -> `FormatFromSingle`
- `string.FormatToken<T>(string, T, IInterpolationSettings)` -> `FormatFromSingle`
- `string.FormatToken<T>(Func<string, TokenNameComparer, T>)` -> `FormatFromFunc`
- `string.FormatToken<T>(Func<string, TokenNameComparer, T>, IInterpolationSettings)` -> `FormatFromFunc`
- `string.FormatToken<T>(Func<string, T>)` -> `FormatFromFunc`
- `string.FormatToken<T>(Func<string, T>, InterpolationSettings)` -> `FormatFromFunc`
- `string.FormatDictionary<T>(Enumerable<KeyValuePair<string, T>>)` -> `FormatFromPairs`
- `string.FormatDictionary<T>(Enumerable<KeyValuePair<string, T>>, IInterpolationSettings)` -> `FormatFromPairs`
- `string.FormatContainer(ITokenValueContainer)` -> `FormatFromContainer`
- `string.FormatContainer(ITokenValueContainer, InterpolationSettings)` -> `FormatFromContainer`

## Settings

The `StringTokenFormatterSettings` class replaces the `IInterpolationSettings` and its associated `InterpolationSettingsBuilder`. Additionally, there is now a `StringTokenFormatterSettings.Global` which can be used to override the default behaviour. Instances of `StringTokenFormatterSettings` can be passed to extension method overloads.

The settings record is immutable and so the `with` keyword is used to mutate the settings, so for example to replace the global settings, something like the following can be used:

```C#
StringTokenFormatterSettings.Global = StringTokenFormatterSettings.Global with { Syntax = CommonTokenSyntax.Round };
```

It should be noted that whilst overriding the global is a convenient action, it can cause side effects by other code using this library. Library implementations should not update `Global`. Alternately, consider creating an instance of `InterpolatedStringResolver` which takes the settings object in its constructor and provides the common methods for expanding from different `ITokenValueContainer` implementations.

Strictly speaking the settings object is split into two parts, `ITokenValueContainerSettings` and `IInterpolatedStringSettings`, the former being used by implementation of `ITokenValueContainer` and the latter being used the Parser and Expander.

See the [readme](/README.md) for the default property values for the global `StringTokenFormatterSettings` instance.

## ITokenValueContainer
Has been changed to take in just the name of the token instead of the `ITokenMatch` interface. In reality that is the only value that containers ever used so implementations should be updates to expect the single string parameter.

The [settings object](#settings) has been extended to handle null and empty string values for matched values, a simple way to check for this is to use the extension `settings.TokenResolutionPolicy.Satisfies(value)`; have a look at one of the build-in containers for a working example. This removes the need for the wrapper containers `EmptyTokenValueContainerImpl`, `IgnoreNullTokenValueContainerImpl` and `IgnoreNullOrEmptyTokenValueContainerImpl` which can be configured on the `TokenResolutionPolicy` property.

## ITokenValueConverter
The list of converters is now handled as part of the [settings object](#settings) and is used by the Expander. The property `ValueConverters` contains the list of converters which are implemented as functions `TryGetResult (object? value, string tokenName)`.

## ITokenValueFormatter 
The wrapper interface has been removed; instead the [settings object](#settings) has a `FormatProvider` property which takes an implementation of `IFormatProvider` being either be a standard `CultureInfo` implementation or a custom implementation of the interface (which could obviously encaptulate a `CultureInfo` implementation ).

## ITokenNameComparer 
Has been removed in favor of using the build-in `StringComparer`; this is set on the `NameComparer` property of the [settings object](#settings).

## IInterpolatedString extensions
Have been removed in favor of calling `InterpolatedStringExpander.Expand` directly or creating an instance of `InterpolatedStringResolver`.

## string.ToInterpolatedString
This can now be achieved by calling `InterpolatedStringParser.Parse` directly.
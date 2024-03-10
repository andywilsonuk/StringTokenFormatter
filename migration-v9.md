# Migration from v8 to v9

See the release notes for more information on new features.

- Introduced [Polysharp](https://github.com/Sergio0694/PolySharp) to lower .NET requirements to: .NET 5 and later, .NET Standard 2.0
- Added `TokenValueFormatException` to handle formatting errors
- Created `string.Format` parity to improve performance
- Classes within the `StringTokenFormatter.Impl` namespace have undergone a number of improvements

## Settings

- `BlockCommands` has been replaced with the more flexible `Commands`
  - `Map` and `Standard` commands added by default as well as the previous block commands `Conditional` and `Loop`
- Added `FormatterDefinitions` property to `IInterpolatedStringSettings` 
- `IInterpolatedStringSettings` now includes `NameComparer` property (previously only on `ITokenValueContainerSettings`)
- `TokenResolutionPolicy` property moved to `ICompositeTokenValueContainerSettings` as it is now only used by `CompositeTokenValueContainer`
- Included additional Value Converters by default, so that a token value of any type can be returned. Previously only `Func<string>` or  `Func<object>` could be used, now for example `Func<int>` is supported. That applies to functions that take token name as input as well
- Better settings validation is now enforced
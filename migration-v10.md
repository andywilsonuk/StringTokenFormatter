# Migration from v9 to v10

See the [v10 release notes](release-notes-v10.md) for a summary of new features and improvements.

## Breaking changes

### `InterpolatedString` segments

`StringTokenFormatter.Impl.InterpolatedString.Segments` now has the type `IReadOnlyList<InterpolatedStringSegment>` instead of `IReadOnlyCollection<InterpolatedStringSegment>`. `IReadOnlyList<T>` still implements `IReadOnlyCollection<T>`, so assigning `Segments` to an `IReadOnlyCollection<InterpolatedStringSegment>` continues to work. Code that depends on the exact property type may need updating.

The public constructor has also been replaced by factory methods. Use `InterpolatedString.Create(segments, settings)` to create an instance with segments, or `InterpolatedString.CreateEmpty(settings)` for an empty instance. `Create` takes an owned, read-only copy of the supplied segments, so later changes to the source collection do not change the interpolated string.

This type is in the `StringTokenFormatter.Impl` namespace; applications that use the resolver API without directly constructing or inspecting `InterpolatedString` do not need to change their code for this update.

## New optional behavior

Nested object-property and string-keyed dictionary lookups are available through setting `ResolveNestedValues` to `NestedValueResolution.Nested`. The default remains `NestedValueResolution.Direct`, preserving v9 lookup behavior. The configured `HierarchicalDelimiter` separates nested property or key names.

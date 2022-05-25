# StringTokenFormatter v6.x
High-speed extension methods to create interpolated strings at runtime and replace tokens within.  Ie:
			```
			var Client = new {
				FirstName = "John",
				LastName = "Smith",
			};
			
			var Message = "Hello {FirstName} {LastName}".FormatToken(Client);
			```

Available on NuGet at https://www.nuget.org/packages/StringTokenFormatter/

To get started include the ```using``` statement so that the extension methods are available:
```C#
using StringTokenFormatter;
```

Tokens with formatting and alignment can be specified in the same way as string.Format for example: ```{token,10:D4}``` see this page on MSDN: http://msdn.microsoft.com/en-us/library/system.string.format(v=vs.110).aspx#FormatItem

## For Objects and Structs: ```FormatToken```
### Usage 1: Using an object's properties to resolve token values
```C#
string original = "start {middle} end";
var tokenValues = new { Middle = "centre" };
string result = original.FormatToken(tokenValues);
Assert.Equal("start centre end", result);
```

### Usage 2: Using a ````Func<>```` to resolve token values
```C#
string original = "start {middle} end";
Func<string, object> func = (token) => { return "centre"; };
string result = original.FormatToken("middle", func);
Assert.Equal("start centre end", result);
```

### Usage 3: Using a single name/value to resolve token values
```C#
string original = "start {middle} end";
string result = original.FormatToken("middle", "centre");
Assert.Equal("start centre end", result);
```

### Usage 4: Constraining parameters by Type
```C#

private interface ITestInterface {
    int First { get; set; }
}

public class TestClass : ITestInterface {
    public int First { get; set; }
    public string Second { get; set; }
}

public void Interface_Excludes_Class_Members() {
    var Test = new TestClass() {
        First= 1,
        Second = "Second"
    };

    var pattern = "{First} {Second}";
    var actual = pattern.FormatToken<ITestInterface>(Test);
    var expected = "1 {Second}";

    Assert.Equal(expected, actual);

}

```


## For Dictionaries and Key/Value Pairs: ```FormatDictionary```

### Usage 1: Using a dictionary's values to resolve token values
```C#
string original = "start {middle} end";
var tokenValues = new Dictionary<string, object> { { "middle", "centre" } };
string result = original.FormatDictionary(tokenValues);
Assert.Equal("start centre end", result);
```

### Usage 2: Using an IEnumerable<KeyValuePair<String, Object>> to resolve token values
```C#
string original = "start {middle} end";
var tokenValues = new[]{ 
	new KeyValuePair<string, Object>("middle", "centre")
};
string result = original.FormatDictionary(tokenValues);
Assert.Equal("start centre end", result);
```


## For High-performance: ```ToInterpolatedString``` then ```FormatContainer```

```C#
/*
When working in a tight loop, you will typically have either a specific format that you
want to use with multiple objects or a single object you want to use with multiple formats.

With a slightly different calling convention you can optimize for reuse.
*/


//Create our pattern once so we can reuse it without having to reparse the string each time.
var Pattern = "{Person_Name} is {Person_Age} and is related to {Brother_Name} who is {Brother_Age}"
	.ToInterpolatedString()
	;

//Create our container once so we can reuse it as well.
var PropertiesContainer = TokenValueContainer.FromObject(new {
	Person_Name = "John",
	Person_Age=21
});

var DictionaryContainer = TokenValueContainer.FromDictionary(new Dictionary<string, object>(){
	["Brother_Name"] = "Joe",
	["Brother_Age"] = 25,
});

//For the sake of this example, we'll use a composite container to merge two together
var Composite = TokenValueContainer.Combine(PropertiesContainer, DictionaryContainer);

//Do this in a tight loop
for(int i = 0; i < 100_000; ++i) {
	//Either of these work
	string result1 = Composite.Format(Pattern);
	string result2 = Pattern.Format(Composite);
	
	Assert.Equal("John is 21 and is related to Joe who is 25", result1);
	Assert.Equal("John is 21 and is related to Joe who is 25", result2);
}

```

## For URIs
All of the string overloads also exist for URIs so you can use them as follows:

``` C#
Uri original = new Uri("http://temp.org/{endpoint}?id={id}");
var tokenValues = new Dictionary<string, object> 
{ 
  { "endpoint", "people" },
  { "id", 10 }
};
Uri expected = new Uri("http://temp.org/people?id=10");

Uri actual = original.FormatToken(tokenValues);

Assert.Equal(expected, actual);
```


# Advanced Details
## Customizing Syntax and More
All interpolating methods accept an optional ```IInterpolationSettings``` parameter.
If it is not provided, ```InterpolationSettings.Default``` is used instead.

To customize how interpolation or parsing works, simply create your own instance of ```IInterpolationSettings```
and use it instead:
```
var Settings = new InterpolationSettingsBuilder() {
    TokenMarkers = TokenMarkers.DollarRound,
}.Build();

var Pattern = "This pattern uses new $(tokens)".ToInterpolatedString(Settings);
```

## Flow of Control
When formatting a string, the following is logical order things happen in:
1.  An ```IInterpolatedStringParser``` turns a ```string``` into an ```IInterpolatedString```
2.  An ```ITokenValueContainer``` provides values, generally by wrapping some other object.
3.  For each ```ITokenMatch``` in the ```IInterpolatedString```
    1. Ask the ```ITokenValueContainer``` for the value with the name of ```ITokenMatch.Token```
    2. If possible, transform the value into a "simpler" value using the ```ITokenValueConverter```
    3. Pass the value to the ```ITokenValueFormatter``` to format the value

## Customizing with ```InterpolationSettingsBuilder```
```InterpolationSettingsBuilder``` contains properties that you can use to further customize the interpolation process.

### ```InterpolationSettingsBuilder.TokenSyntax```
This controls what syntax is used for detecting tokens.
ie. ```{Token}```, ```$(Token)```, or something else.

```TokenSyntaxes.Default``` is the default which uses ```{Token}``` syntax

### ```InterpolationSettingsBuilder.TokenNameComparer```
This controls how token names are compared.
For example, are ```{Token}``` and ```{token}``` the same?

```TokenNameComparers.Default``` is the default which uses ```CurrentCultureIgnoreCase```.

### ```InterpolationSettingsBuilder.TokenValueConverter```
This controls any conversions that are done on token values.
For example, ```When a Func<> is provided, evaluate it```.

```TokenValueConverters.Default``` is the default which converts the following values:
* Null -> Null (no conversion - short circuit)
* Primitive (ie. String, int, etc) -> Primitive (no conversion - short circuit)
* Lazy<string> -> string (via Lazy.Value)
* Lazy<object> -> object (via Lazy.Value)
* Func<string> -> string (via Func())
* Func<object> -> object (via Func())
* Func<string, string> -> string (via Func(TokenName))
* Func<string, object> -> string (via Func(TokenName))

To create your own implementation, you'll generally use ```TokenValueConverters.Combine``` (which returns a ```CompositeTokenValueConverter```) along with your own implementation of the converters.
Note:  ```CompositeTokenValueConverter``` loops through all child converters until it finds a match, so for best performance, put the most common matches at the top.


### ```InterpolationSettingsBuilder.TokenValueFormatter```
This controls how values are formatted.
For example, ```1000.00``` or ```1000,00```

```TokenValueFormatters.Default``` is the default which uses ```CurrentUICulture```.


# Upgrading from v4.x
Version 6.x of StringTokenFormatter is a major upgrade and has a number of enhancements compared to 4.x.

Compared to Version 4.x, Version 6.x is:
* Faster and optimized for high-performance scenarios
* Tighter and optimized to reduce memory allocations and garbage collection
* More Flexible: Generics provide more flexibility and control
* More Consistent: Method overloads and their parameters have been standardized

If you are upgrading from Version 4.x, there are a few things you should be aware of.

## Class and Interface Renames
Nearly every class and interface has been moved or renamed to more clearly indicate what they are used for.
The biggest change is that ```SegmentedString``` is now ```InterpolatedString```.

|Version 4 (old name)	|Version 6 (new name)	|
|-----------------------|-----------------------|
|SegmentedString		|InterpolatedString		|

## ```ITokenValueContainer```s no longer detokenize
There were cases when a dictionary-based ```ITokenValueContainer``` would de-tokenize its parameters.
This meant that ```"token" = "value"``` and ```"{token} = "value"``` were equivalent.

In v6, all dictionary parameters should be provided as un-tokenized values.
For example:
```
var Values = new Dictionary<string, string>();
Values["name"] = "value";

var Message = "Hello {name}".FormatDictionary(Values);
```

## Customizing Token Syntax
In v4, there were mutable static properties that were used to change the default parsing.
Because these were global, multiple assemblies in the same application could conflict with each other.

This has been resolved in v6.

To customize token sytax, use ```InterpolationSettingsBuilder``` as described above.

## Overload Consolidation: ```FormatToken``` / ```FormatDictionary``` / ```FormatContainer```
In Version 4.x, there were multiple overloads of ```FormatToken``` with tons of optional parameters.
This made it difficult to understand what all the parameters were doing.  This has been simplified 
with ```IInterpolationSettings```.


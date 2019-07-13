# StringTokenFormatter v4.x
Provides high-speed extension methods for replacing tokens within strings (using the format '{name}') with their specified lookup value.

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


## For Tight Loops/Advanced Usage: ```FormatContainer```

```C#
/*
When working in a tight loop, you will typically have either a specific format that you
want to use with multiple objects or a single object you want to use with multiple formats.

With a slightly different calling convention you can optimize for reuse.
*/


//Create our pattern once so we can reuse it without having to reparse the string each time.
var Pattern = SegmentedString.Parse("{Person_Name} is {Person_Age} and is related to {Brother_Name} who is {Brother_Age}");

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

## Flow of Control
When formatting a string, the following is logical order things happen in:
1.  An ```ITokenParser``` turns a ```string``` into a ```SegmentedString```
2.  An ```ITokenValueContainer``` provides values, generally by wrapping some other object.
3.  For each ```IMatchedToken``` in the ```SegmentedString```
    1. Ask the ```ITokenValueContainer``` for the value with the name of ```IMatchedToken.Token```
    2. If possible, transform the value into a "simpler" value using the ```ITokenValueConverter```
    3. Pass the value to the ```ITokenValueFormatter``` to format the value

## Further Customisation
The extension methods wrap the functionality you will need and generally take three optional parameters:

### ```ITokenParser Parser = default```
This handles parsing a string into a segmented string.  If no value is provided, ```TokenParser.Default``` is used.
The default implementation uses a compiled regex to detect tokens in the form of {token}.

### ```ITokenValueConverter Converter = default```
This handles converting token values from one type to another.  If no value is provided, ````TokenValueConverter.Default``` is used.
The default implementation will do the following conversions:
* Null -> Null (short circuit)
* Primitive (ie. String, int, etc) -> Primitive (short circuit)
* Lazy<string> -> string (via Lazy.Value)
* Lazy<object> -> object (via Lazy.Value)
* Func<string> -> string (via Func())
* Func<object> -> object (via Func())
* Func<string, string> -> string (via Func(TokenName))
* Func<string, object> -> string (via Func(TokenName))

To create your own implementation, you'll generally use ```TokenValueConverter.Combine``` (which returns a ```CompositeTokenValueConverter```) along with your own implementation of the converters.
Note:  ```CompositeTokenValueConverter``` loops through all child converters until it finds a match, so for best performance, put the most common matches at the top.

### ```ITokenValueFormatter Formatter = default```
This handles generating the string representation of the token values.  If not value is provided, ```TokenValueFormatter.Default``` is used.
The default implementation uses the current culture to provide formatting information.  The following formatters are provided for your conveinance:
* ```TokenValueFormatter.CurrentCulture```
* ```TokenValueFormatter.CurrentUICulture```
* ```TokenValueFormatter.InstalledUICulture```
* ```TokenValueFormatter.InvariantCulture```

To create your own, either implement the ```ITokenValueFormatter``` interface or call ```TokenValueFormatter.From(IFormatProvider)```.

# Upgrading from v3.x
Version 4.x of StringTokenFormatter is a major upgrade and has a number of enhancements compared to 3.x.

Compared to Version 3.x, Version 4.x is:
* Faster and optimized for high-performance scenarios
* Tighter and optimized to reduce memory allocations and garbage collection
* More Flexible: Generics provide more flexibility and control
* More Consistent: Method overloads and their parameters have been standardized

If you are upgrading from Version 3.x, there are a few things you should be aware of.

## New Overloads: ```FormatToken``` / ```FormatDictionary``` / ```FormatContainer```
In Version 3.x, there were multiple overloads of ```FormatToken```.  The problem this caused
is that when performance-enhancing generics were added, generic extension methods take presidence over
interface extension methods.  Plainly stated, when you have ```FormatToken<T>(...)``` and
```FormatToken(IDictionary<string, object>)```, this second overload would never get called without
an explicit cast.  To resolve, that, the Dictionary-specific methods were moved to a new 
overload: ```FormatDictionary```.  Additionally, the ```FormatContainer``` method provides a
similar method for formatting but for containers.

## Extension Method Parameters
When upgrading to version 4.x, you will notice a consistent naming and ordering among the extension method
optional parameters.  You will also notice that the new order resembles the liklihood that you will change
the parameter.  They flow in the following order:
1. ```Formatter```
2. ```Converter```
3. ```Parser```

Whenever null is passed into one of these parameters, it will use the ```Default``` value.

Also, in Version 3, there were some overloads that took an ```IFormatProvider```.  In Version4, if you need
to specify an ```IFormatProvider``` you can use ```TokenValueFormatter.From(IFormatProvider)``` to convert
your ```IFormatProvider``` to an ```ITokenValueFormatter```.

## Class and Interface Renames
A number of classes and interfaces have been renamed to more clearly indicate what they are used for.
The list below is not exhaustive but it will give you a general view of the new names.


|Version 3 (old name)	|Version 4 (new name)	|
|-----------------------|-----------------------|
|ValueMapper			|TokenValueConverter	|
|ValueFormatter			|TokenValueFormatter	|
|TokenMatcher			|TokenParser			|

## Removed Classes
In Version3, there was a ```TokenReplacer``` class that served a number of different purposes.  The functionality
of this class still exists but has been refactored into multiple classes.

The 'Default' members have moved as follows:
DefaultMatcher -> ```TokenParser.Default```
DefaultFormatter -> ```TokenValueFormatter.Default```
DefaultMappers -> ```TokenValueConverter.Default```

The rest of the methods have been relocated to more appropriate locations making this class no longer necessary.
Additionally, by removing this class, multiple allocations were saved on each invocation.




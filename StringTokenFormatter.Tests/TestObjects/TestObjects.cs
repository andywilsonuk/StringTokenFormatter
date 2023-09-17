namespace StringTokenFormatter.Tests;

public static class TestFormats
{
    public static readonly string FirstSecondThird = "{" + nameof(IFirst.First) + "} {" + nameof(ISecond.Second) + "} {" + nameof(IThird.Third) + "}";
}

public class DerivedClass : TestClass, IExtra
{
    public string? Third { get; set; }
}

public interface IFirst
{
    int First { get; set; }
}

public interface ISecond
{
    string? Second { get; set; }
}

public interface IThird
{
    string Third { get; set; }
}

public interface IExtra : IFirst, ISecond
{
    string? Third { get; set; }
}

public class TestClass : IFirst
{
    public int First { get; set; }
    public string? Second { get; set; }
}

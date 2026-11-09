using System.IO;
using System.Runtime.CompilerServices;
using PublicApiGenerator;

namespace StringTokenFormatter.Tests;

// UPDATE_PUBLIC_API_SNAPSHOT=1 dotnet test --project StringTokenFormatter.Tests/StringTokenFormatter.Tests.csproj


public class PublicApiSnapshotTests
{
    [Fact]
    public void PublicApiMatchesSnapshot()
    {
        string actual = typeof(InterpolatedStringResolver).Assembly.GeneratePublicApi().ReplaceLineEndings("\n");
        string snapshotPath = GetSnapshotPath();

        if (Environment.GetEnvironmentVariable("UPDATE_PUBLIC_API_SNAPSHOT") == "1")
        {
            File.WriteAllText(snapshotPath, actual);
            return;
        }

        string expected = File.ReadAllText(snapshotPath).ReplaceLineEndings("\n");
        Assert.Equal(expected, actual);
    }

    private static string GetSnapshotPath([CallerFilePath] string sourcePath = "") =>
        Path.Combine(Path.GetDirectoryName(sourcePath) ?? throw new InvalidOperationException("Test source path is unavailable."), "PublicApiSnapshot.txt");
}

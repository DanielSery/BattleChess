using AwesomeAssertions;
using CrownsGuard.Maps.Utilities;

namespace CrownsGuard.Maps.Test.Utilities;

public class CompressionHelperTest
{
    [Fact]
    public void WhenCompressAndDecompressString_ReturnsOriginalString()
    {
        var inputString = "sample string";

        var compressed = CompressionHelper.Compress(inputString);
        var decompressed = CompressionHelper.Decompress(compressed);

        decompressed.Should().Be(inputString);
    }

    [Fact]
    public void WhenCompressAndDecompressEmptyString_ReturnsOriginalString()
    {
        var inputString = string.Empty;

        var compressed = CompressionHelper.Compress(inputString);
        var decompressed = CompressionHelper.Decompress(compressed);

        decompressed.Should().Be(inputString);
    }
}
using BattleChess3.Maps.Utilities;

namespace BattleChess3.Maps.Test.Utilities;

public class CompressionHelperTest
{
    [Fact]
    public void WhenCompressAndDecompressString_ReturnsOriginalString()
    {
        var inputString = "sample string";

        var compressed = CompressionHelper.Compress(inputString);
        var decompressed = CompressionHelper.Decompress(compressed);

        Assert.Equal(inputString, decompressed);
    }

    [Fact]
    public void WhenCompressAndDecompressEmptyString_ReturnsOriginalString()
    {
        var inputString = string.Empty;

        var compressed = CompressionHelper.Compress(inputString);
        var decompressed = CompressionHelper.Decompress(compressed);

        Assert.Equal(inputString, decompressed);
    }
}
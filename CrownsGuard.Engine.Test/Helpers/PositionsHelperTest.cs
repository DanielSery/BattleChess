using AwesomeAssertions;
using CrownsGuard.Engine.Helpers;

namespace CrownsGuard.Engine.Test.Helpers;

public class PositionsHelperTest
{
    [Theory]
    [InlineData((sbyte)1, (sbyte)2)]
    [InlineData((sbyte)-1, (sbyte)2)]
    [InlineData((sbyte)1, (sbyte)-2)]
    [InlineData((sbyte)-1, (sbyte)-2)]
    public void GetRelativeX_ShouldReturnCorrectValue(sbyte x, sbyte y)
    {
        // Arrange
        var source = new PositionsHelper.PositionHelpStruct(x, y).Position;
        
        // Act
        var result = PositionsHelper.GetRelativeX(source);

        // Assert
        result.Should().Be(x);
    }

    [Theory]
    [InlineData((sbyte)1, (sbyte)2)]
    [InlineData((sbyte)-1, (sbyte)2)]
    [InlineData((sbyte)1, (sbyte)-2)]
    [InlineData((sbyte)-1, (sbyte)-2)]
    public void GetRelativeY_ShouldReturnCorrectValue(sbyte x, sbyte y)
    {
        // Arrange
        var source = new PositionsHelper.PositionHelpStruct(x, y).Position;

        // Act
        var result = PositionsHelper.GetRelativeY(source);

        // Assert
        result.Should().Be(y);
    }
    

    [Theory]
    [InlineData(0b000000001, (byte)1)]
    [InlineData(0b000000111, (byte)7)]
    [InlineData(0b111111000, (byte)0)]
    public void GetAbsoluteX_ShouldReturnCorrectValue(int absolute, byte expected)
    {
        // Act
        var result = PositionsHelper.GetAbsoluteX(absolute);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(0b000001000, (short)1)]
    [InlineData(0b000111000, (short)7)]
    [InlineData(0b000000111, (short)0)]
    public void GetAbsoluteY_ShouldReturnCorrectValue(int absolute, short expected)
    {
        // Act
        var result = PositionsHelper.GetAbsoluteY(absolute);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData((byte)0, (byte)1, (sbyte)1, (sbyte)0)]
    [InlineData((byte)0, (byte)63, (sbyte)7, (sbyte)7)]
    [InlineData((byte)0, (byte)7, (sbyte)7, (sbyte)0)]
    [InlineData((byte)1, (byte)62, (sbyte)5, (sbyte)7)] 
    [InlineData((byte)31, (byte)32, (sbyte)-7, (sbyte)1)]
    [InlineData((byte)5, (byte)12, (sbyte)-1, (sbyte)1)]
    [InlineData((byte)62, (byte)1, (sbyte)-5, (sbyte)-7)]
    [InlineData((byte)63, (byte)0, (sbyte)-7, (sbyte)-7)]
    [InlineData((byte)63, (byte)62, (sbyte)-1, (sbyte)0)]
    [InlineData((byte)7, (byte)0, (sbyte)-7, (sbyte)0)]
    public void GetRelative_ShouldReturnCorrectValue(byte start, byte end, sbyte relX, sbyte relY)
    {
        // Act
        var result = PositionsHelper.GetRelative(start, end);

        // Assert
        var position = new PositionsHelper.PositionHelpStruct(result);
        position.X.Should().Be(relX);
        position.Y.Should().Be(relY);
    }

    [Theory]
    [InlineData(1, 2, (short)0x0201)]
    [InlineData(0, 0, (short)0x0000)]
    [InlineData(-1, -1, (short)-1)]
    public void GetRelativePosition_ShouldReturnCorrectValue(int x, int y, short expected)
    {
        // Act
        var result = PositionsHelper.GetRelativePosition(x, y);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData((byte)0, (ushort)0x0102, 10)]
    [InlineData((byte)0, (ushort)0x0103, 11)]
    [InlineData((byte)0, (ushort)0x0202, 18)]
    [InlineData((byte)0, (ushort)0x0208, -1)]
    [InlineData((byte)0, (ushort)0x0802, -1)]
    [InlineData((byte)0, (ushort)0xFF00, -1)]
    [InlineData((byte)0, (ushort)0x00FF, -1)]
    [InlineData((byte)3, (ushort)0x0102, 13)]
    [InlineData((byte)3, (ushort)0x0103, 14)]
    [InlineData((byte)3, (ushort)0x0202, 21)]
    [InlineData((byte)3, (ushort)0x0205, -1)]
    [InlineData((byte)3, (ushort)0x0802, -1)]
    [InlineData((byte)3, (ushort)0xFF00, -1)]
    [InlineData((byte)3, (ushort)0x00FC, -1)]
    [InlineData((byte)24, (ushort)0x0102, 34)]
    [InlineData((byte)24, (ushort)0x0103, 35)]
    [InlineData((byte)24, (ushort)0x0202, 42)]
    [InlineData((byte)24, (ushort)0x0208, -1)]
    [InlineData((byte)24, (ushort)0x0502, -1)]
    [InlineData((byte)24, (ushort)0xFC00, -1)]
    [InlineData((byte)24, (ushort)0x00FF, -1)]
    [InlineData((byte)63, (ushort)0xFFFF, 54)]
    [InlineData((byte)63, (ushort)0xFFFD, 52)]
    [InlineData((byte)63, (ushort)0xFDFF, 38)]
    [InlineData((byte)63, (ushort)0xF8FF, -1)]
    [InlineData((byte)63, (ushort)0xFFF8, -1)]
    [InlineData((byte)63, (ushort)0x0100, -1)]
    [InlineData((byte)63, (ushort)0x0001, -1)]
    [InlineData((byte)60, (ushort)0xFFFF, 51)]
    [InlineData((byte)60, (ushort)0xFFFD, 49)]
    [InlineData((byte)60, (ushort)0xFDFF, 35)]
    [InlineData((byte)60, (ushort)0xF8FF, -1)]
    [InlineData((byte)60, (ushort)0xFFFB, -1)]
    [InlineData((byte)60, (ushort)0x0100, -1)]
    [InlineData((byte)60, (ushort)0x0004, -1)]
    [InlineData((byte)39, (ushort)0xFFFF, 30)]
    [InlineData((byte)39, (ushort)0xFFFD, 28)]
    [InlineData((byte)39, (ushort)0xFDFF, 14)]
    [InlineData((byte)39, (ushort)0xFBFF, -1)]
    [InlineData((byte)39, (ushort)0xFFF8, -1)]
    [InlineData((byte)39, (ushort)0x0400, -1)]
    [InlineData((byte)39, (ushort)0x0001, -1)]
    public void GetWithOffset_ByteAbsoluteIndex_ShouldReturnCorrectValue(byte absoluteIndex, ushort relative,
        int expected)
    {
        // Act
        var result = absoluteIndex.GetWithOffset((short)relative);

        // Assert
        Assert.Equal(expected, result);
    }
}
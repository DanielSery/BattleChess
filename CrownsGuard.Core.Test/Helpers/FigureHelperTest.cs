using AwesomeAssertions;
using CrownsGuard.Core.Figures;
using CrownsGuard.Core.Helpers;

namespace CrownsGuard.Core.Test.Helpers;

public class FigureHelperTest
{
    [Theory]
    [InlineData(Figure.Knight, Figure.Knight)]
    [InlineData(Figure.Knight | Figure.IsWhite, Figure.Knight)]
    [InlineData(Figure.Knight | Figure.IsWhite | Figure.IsKing, Figure.Knight)]
    [InlineData(Figure.Empty, Figure.Empty)]
    [InlineData(Figure.MountedArcher | Figure.IsBlack, Figure.MountedArcher)]
    public void GetFigureType_ReturnsBaseFigure(Figure input, Figure expected)
    {
        // Act
        var result = input.GetFigureType();

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(Figure.Knight, Figure.Empty)]
    [InlineData(Figure.Knight | Figure.IsKing, Figure.IsKing)]
    [InlineData(Figure.Knight | Figure.IsWhite | Figure.IsKing, Figure.IsKing)]
    [InlineData(Figure.Knight | Figure.IsWhite, Figure.Empty)]
    [InlineData(Figure.Empty, Figure.Empty)]
    [InlineData(Figure.IsKing, Figure.IsKing)]
    [InlineData(Figure.MountedArcher | Figure.IsBlack | Figure.IsKing, Figure.IsKing)]
    [InlineData(Figure.MountedArcher | Figure.IsBlack, Figure.Empty)]
    public void GetIsKing_ReturnsIsKingFlagIfPresent(Figure input, Figure expected)
    {
        // Act
        var result = input.GetIsKing();

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    // Same color, same figure
    [InlineData(Figure.Knight | Figure.IsWhite, Figure.Queen | Figure.IsWhite, true)]
    // Same color, different figures
    [InlineData(Figure.MountedArcher | Figure.IsBlack, Figure.CamelArcher | Figure.IsBlack, true)]
    // Different color, same base figure
    [InlineData(Figure.Knight | Figure.IsWhite, Figure.Knight | Figure.IsBlack, false)]
    // Different color, different figures
    [InlineData(Figure.Knight | Figure.IsWhite, Figure.CamelArcher | Figure.IsBlack, false)]
    // Both neutral
    [InlineData(Figure.Wall, Figure.Trench, true)]
    // Neutral vs colored
    [InlineData(Figure.Wall, Figure.Knight | Figure.IsWhite, false)]
    [InlineData(Figure.Knight | Figure.IsBlack, Figure.Trench, false)]
    // Both empty
    [InlineData(Figure.Empty, Figure.Empty, true)]
    // Empty vs colored
    [InlineData(Figure.Empty, Figure.Knight | Figure.IsWhite, false)]
    [InlineData(Figure.Knight | Figure.IsBlack, Figure.Empty, false)]
    public void IsSameColor_ReturnsExpectedResult(Figure a, Figure b, bool expected)
    {
        // Act
        var result = a.IsSameColor(b);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    // Neutral figure (no color)
    [InlineData(Figure.Wall, Figure.Empty)]
    // White figure
    [InlineData(Figure.Knight | Figure.IsWhite, Figure.IsWhite)]
    // Black figure
    [InlineData(Figure.Knight | Figure.IsBlack, Figure.IsBlack)]
    // White king
    [InlineData(Figure.Knight | Figure.IsWhite | Figure.IsKing, Figure.IsWhite)]
    // Black king
    [InlineData(Figure.Knight | Figure.IsBlack | Figure.IsKing, Figure.IsBlack)]
    // Neutral king (should still be neutral)
    [InlineData(Figure.Wall | Figure.IsKing, Figure.Empty)]
    // Empty
    [InlineData(Figure.Empty, Figure.Empty)]
    // Only color flag (no base figure)
    [InlineData(Figure.IsWhite, Figure.IsWhite)]
    [InlineData(Figure.IsBlack, Figure.IsBlack)]
    public void GetFigureColor_ReturnsExpectedColor(Figure input, Figure expected)
    {
        // Act
        var result = input.GetFigureColor();

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    // Neutral figures (should be true)
    [InlineData(Figure.Empty, true)]
    [InlineData(Figure.Fire, true)]
    [InlineData(Figure.Wall, true)]
    [InlineData(Figure.Trench, true)]
    [InlineData(Figure.Explosives, true)]
    // Neutral figures with flags (should be false)
    [InlineData(Figure.Wall | Figure.IsKing, false)]
    [InlineData(Figure.Trench | Figure.IsWhite, false)]
    [InlineData(Figure.Explosives | Figure.IsBlack, false)]
    // Non-neutral figures (should be false)
    [InlineData(Figure.MountedKnight, false)]
    [InlineData(Figure.Knight, false)]
    [InlineData(Figure.Queen, false)]
    // Non-neutral figures with flags (should be false)
    [InlineData(Figure.Knight | Figure.IsWhite, false)]
    [InlineData(Figure.MountedKnight | Figure.IsBlack, false)]
    [InlineData(Figure.Knight | Figure.IsWhite | Figure.IsKing, false)]
    public void IsNeutralFigure_ReturnsExpectedResult(Figure input, bool expected)
    {
        // Act
        var result = input.IsNeutralFigure();

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    // Neutral figures (no color)
    [InlineData(Figure.Empty, true)]
    [InlineData(Figure.Wall, true)]
    [InlineData(Figure.Trench, true)]
    [InlineData(Figure.Explosives, true)]
    // Neutral figures with color flags (should be false)
    [InlineData(Figure.Wall | Figure.IsWhite, false)]
    [InlineData(Figure.Trench | Figure.IsBlack, false)]
    [InlineData(Figure.Explosives | Figure.IsWhite, false)]
    // Colored figures (should be false)
    [InlineData(Figure.Knight | Figure.IsWhite, false)]
    [InlineData(Figure.Knight | Figure.IsBlack, false)]
    [InlineData(Figure.MountedKnight | Figure.IsWhite, false)]
    // Only color flags (should be false)
    [InlineData(Figure.IsWhite, false)]
    [InlineData(Figure.IsBlack, false)]
    // Non-neutral, non-coloredlu figures (should be true)
    [InlineData(Figure.Knight, true)]
    [InlineData(Figure.MountedKnight, true)]
    public void IsNeutral_ReturnsExpectedResult(Figure input, bool expected)
    {
        // Act
        var result = input.IsNeutral();

        // Assert
        result.Should().Be(expected);
    }
}

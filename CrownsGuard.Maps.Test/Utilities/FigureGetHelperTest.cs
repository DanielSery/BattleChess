using CrownsGuard.Core.Figures;
using CrownsGuard.Game.Players;
using CrownsGuard.Maps.Utilities;
using AwesomeAssertions;

namespace CrownsGuard.Maps.Test.Utilities;

public class FigureGetHelperTest
{
    [Fact]
    public void GetFigure_WithWhiteKing_ReturnsFigureWithIsWhiteAndIsKing()
    {
        var result = FigureGetHelper.GetFigure(PlayerColor.White, true, Figure.Knight);

        result.Should().Be(Figure.Knight | Figure.IsWhite | Figure.IsKing);
    }

    [Fact]
    public void GetFigure_WithBlackKing_ReturnsFigureWithIsBlackAndIsKing()
    {
        var result = FigureGetHelper.GetFigure(PlayerColor.Black, true, Figure.Knight);

        result.Should().Be(Figure.Knight | Figure.IsBlack | Figure.IsKing);
    }

    [Fact]
    public void GetFigure_WithNeutralKing_ReturnsFigureWithIsKingOnly()
    {
        var result = FigureGetHelper.GetFigure(PlayerColor.Neutral, true, Figure.Knight);

        result.Should().Be(Figure.Knight | Figure.IsKing);
    }

    [Fact]
    public void GetFigure_WithWhiteNonKing_ReturnsFigureWithIsWhite()
    {
        var result = FigureGetHelper.GetFigure(PlayerColor.White, false, Figure.Knight);

        result.Should().Be(Figure.Knight | Figure.IsWhite);
    }

    [Fact]
    public void GetFigure_WithBlackNonKing_ReturnsFigureWithIsBlack()
    {
        var result = FigureGetHelper.GetFigure(PlayerColor.Black, false, Figure.Knight);

        result.Should().Be(Figure.Knight | Figure.IsBlack);
    }

    [Fact]
    public void GetFigure_WithNeutralNonKing_ReturnsFigureUnchanged()
    {
        var result = FigureGetHelper.GetFigure(PlayerColor.Neutral, false, Figure.Knight);

        result.Should().Be(Figure.Knight);
    }
}
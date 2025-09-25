using System;
using System.Linq;
using AwesomeAssertions;
using CrownsGuard.Core.Figures;
using CrownsGuard.Core.Helpers;
using CrownsGuard.Maps.BoardBlueprints;
using CrownsGuard.Maps.Utilities;
using CrownsGuard.Core;
using Xunit;
using CrownsGuard.Game.Players;

namespace CrownsGuard.Maps.Test.Utilities;

public class MapExtenderTest
{
    [Fact]
    public void ExtendFor2Players_Throws_WhenFiguresLengthNot16()
    {
        var board = new BoardBlueprint
        {
            Figures = new Figure[15],
            StartingPlayerColor = PlayerColor.White
        };

        Action act = () => board.ExtendFor2Players();

        act.Should().Throw<ArgumentException>().WithMessage("*16 tiles*");
    }

    [Fact]
    public void ExtendFor2Players_Throws_WhenNoWhiteKing()
    {
        var figures = Enumerable.Repeat(Figure.LegionarySword | Figure.IsWhite, 16).ToArray();
        var board = new BoardBlueprint
        {
            Figures = figures,
            StartingPlayerColor = PlayerColor.White
        };

        Action act = () => board.ExtendFor2Players();

        act.Should().Throw<ArgumentException>().WithMessage("*white king*");
    }

    [Fact]
    public void ExtendFor2Players_Throws_WhenMoreThanOneWhiteKing()
    {
        var figures = Enumerable.Repeat(Figure.LegionarySword | Figure.IsWhite, 16).ToArray();
        figures[0] = Figure.King | Figure.IsWhite | Figure.IsKing;
        figures[1] = Figure.King | Figure.IsWhite | Figure.IsKing;
        var board = new BoardBlueprint
        {
            Figures = figures,
            StartingPlayerColor = PlayerColor.White
        };

        Action act = () => board.ExtendFor2Players();

        act.Should().Throw<ArgumentException>().WithMessage("*white king*");
    }

    [Fact]
    public void ExtendFor2Players_Throws_WhenAnyBlackFigure()
    {
        var figures = Enumerable.Repeat(Figure.LegionarySword | Figure.IsWhite, 16).ToArray();
        figures[0] = Figure.King | Figure.IsWhite | Figure.IsKing;
        figures[2] = Figure.LegionarySword | Figure.IsBlack;
        var board = new BoardBlueprint
        {
            Figures = figures,
            StartingPlayerColor = PlayerColor.White
        };

        Action act = () => board.ExtendFor2Players();

        act.Should().Throw<ArgumentException>().WithMessage("*black figure*");
    }

    [Fact]
    public void ExtendFor2Players_ProducesBoardWithValidSize_ForValidInput()
    {
        // Arrange: 16 white figures, one is a king
        var figures = Enumerable.Repeat(Figure.LegionarySword | Figure.IsWhite, 16).ToArray();
        figures[4] = Figure.King | Figure.IsWhite | Figure.IsKing;
        var board = new BoardBlueprint
        {
            Figures = figures,
            StartingPlayerColor = PlayerColor.White
        };
        // Act
        var result = board.ExtendFor2Players();
        // Assert
        result.Figures.Should().HaveCount(Constants.FullBoardTilesCount);
    }

    [Fact]
    public void ExtendFor2Players_ProducesBoardWithWhiteAndBlackKing_ForValidInput()
    {
        // Arrange: 16 white figures, one is a king
        var figures = Enumerable.Repeat(Figure.LegionarySword | Figure.IsWhite, 16).ToArray();
        figures[4] = Figure.King | Figure.IsWhite | Figure.IsKing;
        var board = new BoardBlueprint
        {
            Figures = figures,
            StartingPlayerColor = PlayerColor.White
        };
        // Act
        var result = board.ExtendFor2Players();
        // Assert
        result.Figures.Count(f => f.GetFigureType() == Figure.King && f.IsWhite()).Should().Be(1);
        result.Figures.Count(f => f.GetFigureType() == Figure.King && f.IsBlack()).Should().Be(1);
    }

    [Fact]
    public void ExtendFor2Players_ProducesBoardWithSameNumberOfBlackWhite_ForValidInput()
    {
        // Arrange: 16 white figures, one is a king
        var figures = Enumerable.Repeat(Figure.LegionarySword | Figure.IsWhite, 16).ToArray();
        figures[4] = Figure.King | Figure.IsWhite | Figure.IsKing;
        var board = new BoardBlueprint
        {
            Figures = figures,
            StartingPlayerColor = PlayerColor.White
        };
        // Act
        var result = board.ExtendFor2Players();
        // Assert
        var whiteCount = result.Figures.Count(f => f.IsWhite());
        var blackCount = result.Figures.Count(f => f.IsBlack());
        whiteCount.Should().Be(blackCount);
    }
}
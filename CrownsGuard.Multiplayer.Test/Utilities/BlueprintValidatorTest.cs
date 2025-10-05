using System.Collections;
using AwesomeAssertions;
using CrownsGuard.Core.Figures;
using CrownsGuard.Multiplayer.Utilities;

namespace CrownsGuard.Multiplayer.Test.Utilities;

public class BlueprintValidatorTests
{
    [Fact]
    public void IsValid_IfNoBoard_ReturnsFalse()
    {
        // Arrange
        var figures = Array.Empty<Figure>();
        
        // Act
        var result = figures.IsValid(UnlockedFigures.DefaultUnlockedFigures);
        
        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_IfEmptyBoard_ReturnsFalse()
    {
        // Arrange
        var board = Enumerable.Repeat(Figure.Empty, 16).ToArray();

        // Act
        var result = board.IsValid(UnlockedFigures.DefaultUnlockedFigures);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsValid_IfWhiteKing_ReturnsTrue()
    {
        // Arrange
        var board = Enumerable.Repeat(Figure.Empty, 16).ToArray();
        board[0] = Figure.King | Figure.IsWhite | Figure.IsKing;
        
        // Act
        var result = board.IsValid(UnlockedFigures.DefaultUnlockedFigures); 
        
        // Assert
        Assert.True(result);   
    }

    [Fact]
    public void IsValid_IfQueenIsKing_ReturnsTrue()
    {
        // Arrange
        var board = Enumerable.Repeat(Figure.Empty, 16).ToArray();
        board[0] = Figure.Queen | Figure.IsWhite | Figure.IsKing;
        
        // Act
        var result = board.IsValid(UnlockedFigures.DefaultUnlockedFigures); 
        
        // Assert
        Assert.True(result);  
    }

    [Fact]
    public void IsValid_IfBlackFigure_ReturnsFalse()
    {
        // Arrange
        var board = Enumerable.Repeat(Figure.Empty, 16).ToArray();
        board[0] = Figure.King | Figure.IsWhite | Figure.IsKing;
        board[1] = Figure.King | Figure.IsBlack;
        
        // Act
        var result = board.IsValid(UnlockedFigures.DefaultUnlockedFigures);
        
        // Assert
        Assert.False(result);  
    }

    [Fact]
    public void IsValid_IfNeutralKing_ReturnsFalse()
    {
        // Arrange
        var board = Enumerable.Repeat(Figure.Empty, 16).ToArray();
        board[0] = Figure.King | Figure.IsWhite | Figure.IsKing;
        board[1] = Figure.Empty | Figure.IsKing;
        
        // Act
        var result = board.IsValid(UnlockedFigures.DefaultUnlockedFigures);
        
        // Assert
        Assert.False(result); 
    }

    [Fact]
    public void IsValid_IfLargeValue_ReturnsFalse()
    {
        // Arrange
        var board = Enumerable.Repeat(Figure.Empty, 16).ToArray();
        board[0] = Figure.King | Figure.IsWhite | Figure.IsKing;
        for (var i = 1; i <= 5; i++)
            board[i] = Figure.Queen | Figure.IsWhite;
        
        // Act
        var result = board.IsValid(UnlockedFigures.DefaultUnlockedFigures);
        
        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsValid_IfTwoKings_ReturnsFalse()
    {
        // Arrange
        var board = Enumerable.Repeat(Figure.Empty, 16).ToArray();
        board[0] = Figure.King | Figure.IsWhite | Figure.IsKing;
        board[1] = Figure.King | Figure.IsWhite | Figure.IsKing;
        
        // Act
        var result = board.IsValid(UnlockedFigures.DefaultUnlockedFigures);
        
        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsValid_IfNonNeutralUnitIsNeutral_ReturnsFalse()
    {
        // Arrange
        var board = Enumerable.Repeat(Figure.Empty, 16).ToArray();
        board[0] = Figure.Queen | Figure.IsWhite | Figure.IsKing;
        board[1] = Figure.Queen;
        
        // Act
        var result = board.IsValid(UnlockedFigures.DefaultUnlockedFigures);
        
        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsValid_IfContainingNotUnlockedFigure_ReturnsFalse()
    {
        // Arrange
        var board = Enumerable.Repeat(Figure.Empty, 16).ToArray();
        board[0] = Figure.Queen | Figure.IsWhite | Figure.IsKing;
        board[1] = Figure.LegionaryPike | Figure.IsWhite;
        
        // Act
        var result = board.IsValid(UnlockedFigures.DefaultUnlockedFigures);
        
        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsValid_WhenUnlockedSpecialFigure_ReturnsTrue()
    {
        // Arrange
        var board = Enumerable.Repeat(Figure.Empty, 16).ToArray();
        board[0] = Figure.Queen | Figure.IsWhite | Figure.IsKing;
        board[1] = Figure.LegionaryPike | Figure.IsWhite;
        
        var unlockedFigures = UnlockedFigures.DefaultUnlockedFigures.ToArray();
        var bitArray = new BitArray(unlockedFigures)
        {
            [(int)Figure.LegionaryPike] = true
        };
        bitArray.CopyTo(unlockedFigures, 0);
        
        // Act
        var result = board.IsValid(unlockedFigures);
        
        // Assert
        Assert.True(result);
    }
}
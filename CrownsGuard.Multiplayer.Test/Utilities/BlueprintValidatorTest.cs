using System.Collections;
using AwesomeAssertions;
using CrownsGuard.Core.Figures;
using CrownsGuard.Multiplayer.Utilities;

namespace CrownsGuard.Multiplayer.Test.Utilities;

public class BlueprintValidatorTests
{
    [Fact]
    public void ValidateMap_IfNullUnlockedFigures_UsesDefault()
    {
        // Arrange
        var board = Enumerable.Repeat(Figure.Empty, 16).ToArray();
        board[0] = Figure.King | Figure.IsWhite | Figure.IsKing;
        board[1] = Figure.LegionarySword | Figure.IsWhite;
        
        // Act
        var result = board.ValidateMap(null);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
    }
    
    [Fact]
    public void ValidateMap_IfNoBoard_ReturnsFalse()
    {
        // Arrange
        var figures = Array.Empty<Figure>();
        
        // Act
        var result = figures.ValidateMap(UnlockedFigures.DefaultUnlockedFigures);
        
        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.Errors[0].Message.Should().Be("Invalid number of figures in setup");   
    }

    [Fact]
    public void ValidateMap_IfEmptyBoard_ReturnsFalse()
    {
        // Arrange
        var board = Enumerable.Repeat(Figure.Empty, 16).ToArray();

        // Act
        var result = board.ValidateMap(UnlockedFigures.DefaultUnlockedFigures);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.Errors[0].Message.Should().Be("Setup does not contain king");   
    }

    [Fact]
    public void ValidateMap_IfWhiteKing_ReturnsTrue()
    {
        // Arrange
        var board = Enumerable.Repeat(Figure.Empty, 16).ToArray();
        board[0] = Figure.King | Figure.IsWhite | Figure.IsKing;
        
        // Act
        var result = board.ValidateMap(UnlockedFigures.DefaultUnlockedFigures); 
        
        // Assert
        result.IsSuccess.Should().BeTrue(); 
    }

    [Fact]
    public void ValidateMap_IfQueenIsKing_ReturnsTrue()
    {
        // Arrange
        var board = Enumerable.Repeat(Figure.Empty, 16).ToArray();
        board[0] = Figure.Queen | Figure.IsWhite | Figure.IsKing;
        
        // Act
        var result = board.ValidateMap(UnlockedFigures.DefaultUnlockedFigures); 
        
        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void ValidateMap_IfBlackFigure_ReturnsFalse()
    {
        // Arrange
        var board = Enumerable.Repeat(Figure.Empty, 16).ToArray();
        board[0] = Figure.King | Figure.IsWhite | Figure.IsKing;
        board[1] = Figure.King | Figure.IsBlack;
        
        // Act
        var result = board.ValidateMap(UnlockedFigures.DefaultUnlockedFigures);
        
        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.Errors[0].Message.Should().Be("Setup contains black figure");  
    }

    [Fact]
    public void ValidateMap_IfNeutralKing_ReturnsFalse()
    {
        // Arrange
        var board = Enumerable.Repeat(Figure.Empty, 16).ToArray();
        board[0] = Figure.King | Figure.IsWhite | Figure.IsKing;
        board[1] = Figure.Empty | Figure.IsKing;
        
        // Act
        var result = board.ValidateMap(UnlockedFigures.DefaultUnlockedFigures);
        
        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.Errors[0].Message.Should().Be("Setup contains player figure without player assigned"); 
    }

    [Fact]
    public void ValidateMap_IfLargeValue_ReturnsFalse()
    {
        // Arrange
        var board = Enumerable.Repeat(Figure.Empty, 16).ToArray();
        board[0] = Figure.King | Figure.IsWhite | Figure.IsKing;
        for (var i = 1; i <= 5; i++)
            board[i] = Figure.Queen | Figure.IsWhite;
        
        // Act
        var result = board.ValidateMap(UnlockedFigures.DefaultUnlockedFigures);
        
        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.Errors[0].Message.Should().Be("Setup total figures value is too high");
    }

    [Fact]
    public void ValidateMap_IfTwoKings_ReturnsFalse()
    {
        // Arrange
        var board = Enumerable.Repeat(Figure.Empty, 16).ToArray();
        board[0] = Figure.King | Figure.IsWhite | Figure.IsKing;
        board[1] = Figure.King | Figure.IsWhite | Figure.IsKing;
        
        // Act
        var result = board.ValidateMap(UnlockedFigures.DefaultUnlockedFigures);
        
        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.Errors[0].Message.Should().Be("Setup contains more than one king");
    }

    [Fact]
    public void ValidateMap_IfNonNeutralUnitIsNeutral_ReturnsFalse()
    {
        // Arrange
        var board = Enumerable.Repeat(Figure.Empty, 16).ToArray();
        board[0] = Figure.Queen | Figure.IsWhite | Figure.IsKing;
        board[1] = Figure.Queen;
        
        // Act
        var result = board.ValidateMap(UnlockedFigures.DefaultUnlockedFigures);
        
        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.Errors[0].Message.Should().Be("Setup contains player figure without player assigned");
    }

    [Fact]
    public void ValidateMap_IfContainingNotUnlockedFigure_ReturnsFalse()
    {
        // Arrange
        var board = Enumerable.Repeat(Figure.Empty, 16).ToArray();
        board[0] = Figure.Queen | Figure.IsWhite | Figure.IsKing;
        board[1] = Figure.LegionaryPike | Figure.IsWhite;
        
        // Act
        var result = board.ValidateMap(UnlockedFigures.DefaultUnlockedFigures);
        
        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.Errors[0].Message.Should().Be("Setup contains not unlocked figure");
    }

    [Fact]
    public void ValidateMap_WhenUnlockedSpecialFigure_ReturnsTrue()
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
        var result = board.ValidateMap(unlockedFigures);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
    }
}
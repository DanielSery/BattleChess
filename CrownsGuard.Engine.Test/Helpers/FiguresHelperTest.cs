using CrownsGuard.Core.Board;
using CrownsGuard.Core.Figures;
using CrownsGuard.Engine.Helpers;

namespace CrownsGuard.Engine.Test.Helpers;

public class FiguresHelperTest
{
    [Theory]
    [InlineData((byte)0, Figure.Knight)]
    [InlineData((byte)5, Figure.Mage)]
    [InlineData((byte)9, Figure.Wall)]
    public void CreateFigure_SetsBoardCell(byte targetIndex, Figure createdFigure)
    {
        // Arrange
        var board = new Figure[16].AsSpan();

        // Act
        board.CreateFigure(targetIndex, createdFigure, IgnoreEvent);

        // Assert
        Assert.Equal(createdFigure, board[targetIndex]);
    }

    [Theory]
    [InlineData((byte)0, Figure.Knight)]
    [InlineData((byte)5, Figure.Mage)]
    [InlineData((byte)9, Figure.Wall)]
    public void Die_RemovesFigure(byte targetIndex, Figure killedFigure)
    {
        // Arrange
        var board = new Figure[16].AsSpan();
        board[targetIndex] = killedFigure;

        // Act
        board.Die(targetIndex, IgnoreEvent);

        // Assert
        Assert.Equal(Figure.Empty, board[targetIndex]);
    }

    [Fact]
    public void Die_EmptyCell_RemainsEmpty()
    {
        // Arrange
        var board = new Figure[16].AsSpan();
        const byte targetIndex = 0;

        // Act
        board.Die(targetIndex, IgnoreEvent);

        // Assert
        Assert.Equal(Figure.Empty, board[targetIndex]);
    }
    
    [Theory]
    [InlineData((byte)0, (byte)1, Figure.Knight, Figure.Mage)]
    [InlineData((byte)5, (byte)9, Figure.Wall, Figure.Knight)]
    public void SwapTiles_SwapsFigures(byte sourceIndex, byte targetIndex, Figure sourceFigure, Figure targetFigure)
    {
        // Arrange
        var board = new Figure[16].AsSpan();
        board[sourceIndex] = sourceFigure;
        board[targetIndex] = targetFigure;

        // Act
        board.SwapTiles(sourceIndex, targetIndex, IgnoreEvent);

        // Assert
        Assert.Equal(targetFigure, board[sourceIndex]);
        Assert.Equal(sourceFigure, board[targetIndex]);
    }

    [Theory]
    [InlineData((byte)0, (byte)1, Figure.Knight)]
    [InlineData((byte)5, (byte)9, Figure.Wall)]
    public void SwapTiles_WithEmpty_SwapsFigures(byte sourceIndex, byte targetIndex, Figure sourceFigure)
    {
        // Arrange
        var board = new Figure[16].AsSpan();
        board[sourceIndex] = sourceFigure;

        // Act
        board.SwapTiles(sourceIndex, targetIndex, IgnoreEvent);

        // Assert
        Assert.Equal(Figure.Empty, board[sourceIndex]);
        Assert.Equal(sourceFigure, board[targetIndex]);
    }
    
    [Theory]
    [InlineData((byte)0, (byte)1, Figure.Knight, Figure.Mage)]
    [InlineData((byte)5, (byte)9, Figure.Wall, Figure.Knight)]
    public void MoveFigure_WithOccupiedTarget_KillsTargetAndMovesFigure(byte sourceIndex, byte targetIndex, Figure sourceFigure, Figure targetFigure)
    {
        // Arrange
        var board = new Figure[16].AsSpan();
        board[sourceIndex] = sourceFigure;
        board[targetIndex] = targetFigure;

        // Act
        board.MoveFigure(sourceIndex, targetIndex, IgnoreEvent);

        // Assert
        Assert.Equal(Figure.Empty, board[sourceIndex]);
        Assert.Equal(sourceFigure, board[targetIndex]);
    }

    [Theory]
    [InlineData((byte)0, (byte)1, Figure.Knight)]
    [InlineData((byte)5, (byte)9, Figure.Wall)]
    public void MoveFigure_WithEmptyTarget_MovesFigure(byte sourceIndex, byte targetIndex, Figure sourceFigure)
    {
        // Arrange
        var board = new Figure[16].AsSpan();
        board[sourceIndex] = sourceFigure;

        // Act
        board.MoveFigure(sourceIndex, targetIndex, IgnoreEvent);

        // Assert
        Assert.Equal(Figure.Empty, board[sourceIndex]);
        Assert.Equal(sourceFigure, board[targetIndex]);
    }
    
    [Theory]
    [InlineData((byte)0, (byte)1, Figure.Knight, Figure.Mage)]
    [InlineData((byte)5, (byte)9, Figure.Wall, Figure.Knight)]
    public void KillWithoutMove_RemovesTargetFigure(byte sourceIndex, byte targetIndex, Figure sourceFigure, Figure targetFigure)
    {
        // Arrange
        var board = new Figure[16].AsSpan();
        board[sourceIndex] = sourceFigure;
        board[targetIndex] = targetFigure;

        // Act
        board.KillWithoutMove(sourceIndex, targetIndex, IgnoreEvent);

        // Assert
        Assert.Equal(sourceFigure, board[sourceIndex]);
        Assert.Equal(Figure.Empty, board[targetIndex]);
    }

    [Theory]
    [InlineData((byte)0, (byte)1, Figure.Knight, Figure.Mage)]
    [InlineData((byte)5, (byte)9, Figure.Wall, Figure.Knight)]
    public void KillWithMove_WithOccupiedTarget_KillsTargetAndMovesFigure(byte sourceIndex, byte targetIndex, Figure sourceFigure, Figure targetFigure)
    {
        // Arrange
        var board = new Figure[16].AsSpan();
        board[sourceIndex] = sourceFigure;
        board[targetIndex] = targetFigure;

        // Act
        board.KillWithMove(sourceIndex, targetIndex, IgnoreEvent);

        // Assert
        Assert.Equal(Figure.Empty, board[sourceIndex]);
        Assert.Equal(sourceFigure, board[targetIndex]);
    }

    [Theory]
    [InlineData((byte)0, (byte)1, Figure.Knight)]
    [InlineData((byte)5, (byte)9, Figure.Wall)]
    public void KillWithMove_WithEmptyTarget_MovesFigure(byte sourceIndex, byte targetIndex, Figure sourceFigure)
    {
        // Arrange
        var board = new Figure[16].AsSpan();
        board[sourceIndex] = sourceFigure;

        // Act
        board.KillWithMove(sourceIndex, targetIndex, IgnoreEvent);

        // Assert
        Assert.Equal(Figure.Empty, board[sourceIndex]);
        Assert.Equal(sourceFigure, board[targetIndex]);
    }

    [Theory]
    [InlineData(Figure.Knight | Figure.IsWhite, Figure.Mage | Figure.IsBlack, Figure.Mage | Figure.IsWhite)]
    [InlineData(Figure.Knight | Figure.IsWhite, Figure.Mage | Figure.IsWhite, Figure.Mage | Figure.IsWhite)]
    [InlineData(Figure.Knight | Figure.IsBlack, Figure.Mage | Figure.IsWhite, Figure.Mage | Figure.IsBlack)]
    [InlineData(Figure.Knight | Figure.IsBlack, Figure.Mage | Figure.IsBlack, Figure.Mage | Figure.IsBlack)]
    [InlineData(Figure.Knight | Figure.IsBlack, Figure.Mage, Figure.Mage | Figure.IsBlack)]
    public void ConvertFigure_WithOccupiedTarget_ConvertsTargetFigure(Figure sourceFigure, Figure targetFigure, Figure finalFigure)
    {
        // Arrange
        const int sourceIndex = 0;
        const int targetIndex = 1;
        
        var board = new Figure[16].AsSpan();
        board[sourceIndex] = sourceFigure;
        board[targetIndex] = targetFigure;

        // Act
        board.ConvertUnit(sourceIndex, targetIndex, IgnoreEvent);

        // Assert
        Assert.Equal(finalFigure, board[targetIndex]);
    }
    
    [Theory]
    [InlineData(Figure.Knight, Figure.Mage)]
    [InlineData(Figure.Wall, Figure.Knight)]
    [InlineData(Figure.Mage | Figure.IsWhite, Figure.Knight | Figure.IsWhite)]
    [InlineData(Figure.Wall | Figure.IsBlack, Figure.Mage | Figure.IsBlack)]
    public void ChangeFigureType_ChangesFigureType(Figure originalFigure, Figure newFigure)
    {
        // Arrange
        const byte sourceIndex = 0;
        const byte targetIndex = 1;
        var board = new Figure[16].AsSpan();
        board[targetIndex] = originalFigure;

        // Act
        board.ChangeFigureType(sourceIndex, targetIndex, newFigure, IgnoreEvent);

        // Assert
        Assert.Equal(newFigure, board[targetIndex]);
    }

    [Theory]
    [InlineData(Figure.Knight)]
    [InlineData(Figure.Mage)]
    [InlineData(Figure.Wall)]
    [InlineData(Figure.Knight | Figure.IsWhite)]
    [InlineData(Figure.Mage | Figure.IsBlack)]
    public void MakeUnitKing_MakesFigureKing(Figure originalFigure)
    {
        // Arrange
        const byte sourceIndex = 0;
        const byte targetIndex = 1;
        var board = new Figure[16].AsSpan();
        board[targetIndex] = originalFigure;

        // Act
        board.MakeUnitKing(sourceIndex, targetIndex, IgnoreEvent);

        // Assert
        Assert.Equal(originalFigure | Figure.IsKing, board[targetIndex]);
    }

    private static void IgnoreEvent(BoardEvent arg1, Span<Figure> arg2)
    {
    }
}
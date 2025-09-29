// C:\Users\danys\Source\Repos\CrownsGuard\CrownsGuard.Engine.Test\Helpers\ChangeToQueenTest.cs

using CrownsGuard.Engine.Helpers;
using CrownsGuard.Core.Figures;
using CrownsGuard.Core.Board;

namespace CrownsGuard.Engine.Test.Helpers;

public class FigureActionExecutorTest
{
    [Fact]
    public void ExecuteFigureAction_ShouldMoveFigureWhenActionIsMove()
    {
        // Arrange
        var board = new Figure[64];
        board[2] = Figure.Knight;
        var action = new FigureAction(FigureActionType.Move, 2, 10, Figure.Knight);

        // Act
        FigureActionExecutor.ExecuteFigureAction(board, action, DoNothing);

        // Assert
        Assert.Equal(Figure.Empty, board[2]);
        Assert.Equal(Figure.Knight, board[10]);
    }

    [Fact]
    public void ExecuteFigureAction_ShouldPerformMeleeAttack()
    {
        // Arrange
        var board = new Figure[64];
        board[2] = Figure.Knight;
        board[10] = Figure.Pikeman;
        var action = new FigureAction(FigureActionType.MeleeAttack, 2, 10, Figure.Knight);

        // Act
        FigureActionExecutor.ExecuteFigureAction(board, action, DoNothing);

        // Assert
        Assert.Equal(Figure.Knight, board[10]);
        Assert.Equal(Figure.Empty, board[2]);
    }

    [Fact]
    public void ExecuteFigureAction_ShouldThrowInvalidOperationForNonExecutableAction()
    {
        // Arrange
        var board = new Figure[64];
        var action = new FigureAction(FigureActionType.IsExecutable, 2, 10, Figure.Knight);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() =>
            FigureActionExecutor.ExecuteFigureAction(board, action, DoNothing));
    }

    [Fact]
    public void ChangeToQueen_ShouldChangeFigureToQueen_WhenTargetTileIsWalkable()
    {
        // Arrange
        var board = new Figure[64];
        board[2] = Figure.LegionarySword;
        board[20] = Figure.Empty;
        var action = new FigureAction(FigureActionType.ChangeToQueen, 2, 20, Figure.LegionarySword);

        // Act
        FigureActionExecutor.ChangeToQueen(board, action, DoNothing);

        // Assert
        Assert.Equal(Figure.Queen, board[20]);
        Assert.Equal(Figure.Empty, board[2]);
    }

    [Fact]
    public void ChangeToQueen_ShouldChangeFigureToQueen_WhenTargetTileIsNotWalkable()
    {
        // Arrange
        var board = new Figure[64];
        board[2] = Figure.LegionarySword;
        board[20] = Figure.Knight;
        var action = new FigureAction(FigureActionType.ChangeToQueen, 2, 20, Figure.LegionarySword);

        // Act
        FigureActionExecutor.ChangeToQueen(board, action, DoNothing);

        // Assert
        Assert.Equal(Figure.Queen, board[20]);
        Assert.Equal(Figure.Empty, board[2]);
    }

    [Fact]
    public void MeleePierceAttack_WhenLength1_ShouldKill1FigureInPath()
    {
        // Arrange
        var board = new Figure[64];
        board[2] = Figure.Knight;
        board[10] = Figure.Pikeman;
        board[18] = Figure.Archer;
        var action = new FigureAction(FigureActionType.MeleePierceAttack, 2, 10, Figure.Knight);

        // Act
        FigureActionExecutor.MeleePierceAttack(board, action, DoNothing);

        // Assert
        Assert.Equal(Figure.Empty, board[2]);
        Assert.Equal(Figure.Knight, board[10]);
    }

    [Fact]
    public void MeleePierceAttack_WhenLength2_ShouldKill2FiguresInPath()
    {
        // Arrange
        var board = new Figure[64];
        board[2] = Figure.Knight;
        board[10] = Figure.Pikeman;
        board[18] = Figure.Archer;
        var action = new FigureAction(FigureActionType.MeleePierceAttack, 2, 18, Figure.Knight);

        // Act
        FigureActionExecutor.MeleePierceAttack(board, action, DoNothing);

        // Assert
        Assert.Equal(Figure.Empty, board[2]);
        Assert.Equal(Figure.Empty, board[10]);
        Assert.Equal(Figure.Knight, board[18]);
    }

    [Fact]
    public void MeleePierceAttack_WhenLength3_ShouldKill3FiguresInPath()
    {
        // Arrange
        var board = new Figure[64];
        board[2] = Figure.Knight;
        board[10] = Figure.Pikeman;
        board[18] = Figure.Archer;
        var action = new FigureAction(FigureActionType.MeleePierceAttack, 2, 26, Figure.Knight);

        // Act
        FigureActionExecutor.MeleePierceAttack(board, action, DoNothing);

        // Assert
        Assert.Equal(Figure.Empty, board[2]);
        Assert.Equal(Figure.Empty, board[10]);
        Assert.Equal(Figure.Empty, board[18]);
        Assert.Equal(Figure.Knight, board[26]);
    }

    [Fact]
    public void ExecuteFigureAction_ShouldThrowArgumentOutOfRangeForUnknownAction()
    {
        // Arrange
        var board = new Figure[64];
        var action = new FigureAction((FigureActionType)999, 2, 10, Figure.Knight);

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            FigureActionExecutor.ExecuteFigureAction(board, action, DoNothing));
    }

    private static void DoNothing(BoardEvent arg1, Span<Figure> arg2)
    {
    }
}
using BattleChess3.Game.Figures;
using BattleChess3.Game.GameBoard;
using BattleChess3.Game.Players;

namespace BattleChess3.CrossFireFigures.Utilities;

internal static class AdvancedFigureActions
{
    public static bool CanDestroy(this ITile unitTile, IBoard board, Position relativePosition)
    {
        var movePosition = unitTile.RelativePosition + relativePosition;
        return board.TryGetTile(movePosition, out var targetTile) &&
               !targetTile.IsEmpty();
    }
    
    public static void TryDestroyTile(this ITile unitTile, IBoard board, Position positionDiff)
    {
        if (!board.TryGetTile(unitTile.RelativePosition + positionDiff, out var targetTile))
            return;

        unitTile.KillWithoutMove(targetTile, board);
    }

    public static bool CanAttack(this ITile unitTile, IBoard board, Position relativePosition)
    {
        var movePosition = unitTile.RelativePosition + relativePosition;
        return board.TryGetTile(movePosition, out var targetTile) &&
               unitTile.CanAttack(targetTile);
    }
    
    public static bool TryCreateKillWithMove(this ITile unitTile, IBoard board, Position relativePosition, out FigureAction action)
    {
        var movePosition = unitTile.RelativePosition + relativePosition;
        if (!board.TryGetTile(movePosition, out var targetTile) ||
            !unitTile.CanAttack(targetTile))
        {
            action = FigureAction.None;
            return false;
        }

        action = unitTile.CreateKillWithMove(targetTile, board);
        return true;
    }
    
    public static bool TryCreateKillWithoutMove(this ITile unitTile, IBoard board, Position relativePosition, out FigureAction action)
    {
        var movePosition = unitTile.RelativePosition + relativePosition;
        if (!board.TryGetTile(movePosition, out var targetTile) ||
            !unitTile.CanAttack(targetTile))
        {
            action = FigureAction.None;
            return false;
        }

        action = unitTile.CreateKillWithoutMove(targetTile, board);
        return true;
    }

    public static bool CanMoveTo(this ITile unitTile, IBoard board, Position relativePosition)
    {
        var movePosition = unitTile.RelativePosition + relativePosition;
        return board.TryGetTile(movePosition, out var targetTile) &&
               unitTile.CanMoveTo(targetTile);
    }

    public static bool TryCreateMoveAction(this ITile unitTile, IBoard board, Position relativePosition, out FigureAction action)
    {
        var movePosition = unitTile.RelativePosition + relativePosition;
        if (!board.TryGetTile(movePosition, out var targetTile) ||
            !unitTile.CanMoveTo(targetTile))
        {
            action = FigureAction.None;
            return false;
        }

        action = unitTile.CreateMoveAction(targetTile, board);
        return true;
    }

    public static bool TryCreateNewFigureAction(this ITile unitTile, IBoard board, Position relativePosition, 
        LocalPlayerInfo player, IFigureType figureType,
        out FigureAction action)
    {
        var movePosition = unitTile.RelativePosition + relativePosition;
        if (!board.TryGetTile(movePosition, out var targetTile) ||
            !targetTile.IsEmpty())
        {
            action = FigureAction.None;
            return false;
        }

        action = unitTile.CreateNewFigureAction(targetTile, player, figureType, board);
        return true;
    }
    public static FigureAction CreateNewFigureAction(this ITile unitTile, ITile targetTile, LocalPlayerInfo player, IFigureType figureType, IBoard board)
    {
        return new FigureAction(
            FigureActionTypes.Special, 
            targetTile.AbsolutePosition,
            () => targetTile.CreateFigure(new Figure(player, figureType, false), board));
    }

    public static FigureAction CreateMoveAction(this ITile unitTile, ITile targetTile, IBoard board)
    {
        return new FigureAction(
            FigureActionTypes.Move, 
            targetTile.AbsolutePosition,
            () => unitTile.MoveToTile(targetTile, board));
    }

    public static FigureAction CreateKillWithoutMove(this ITile unitTile, ITile targetTile, IBoard board)
    {
        return new FigureAction(
            FigureActionTypes.Attack, 
            targetTile.AbsolutePosition,
            () => unitTile.KillWithoutMove(targetTile, board));
    }

    public static FigureAction CreateKillWithMove(this ITile unitTile, ITile targetTile, IBoard board)
    {
        return new FigureAction(
            FigureActionTypes.Attack, 
            targetTile.AbsolutePosition,
            () =>
            {
                unitTile.KillWithMove(targetTile, board);
            });
    }
}
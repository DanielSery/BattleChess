using BattleChess3.DefaultFigures.Utilities;
using BattleChess3.Game.Board;
using BattleChess3.Game.Figures;
using BattleChess3.Game.Players;

namespace BattleChess3.DefaultFigures;

public static class DefaultFigureActions
{
    public static FigureAction CreateNewFigureAction(this ITile unitTile, ITile targetTile, Player player, IFigureType figureType, IBoard board)
    {
        return new FigureAction(
            FigureActionTypes.Special, 
            targetTile.AbsolutePosition,
            targetTile.AbsolutePosition,
            () => targetTile.CreateFigure(new Figure(player, figureType), board));
    }

    public static FigureAction CreateMoveAction(this ITile unitTile, ITile targetTile, IBoard board)
    {
        return new FigureAction(
            FigureActionTypes.Move, 
            unitTile.AbsolutePosition,
            targetTile.AbsolutePosition,
            () => unitTile.MoveToTile(targetTile, board));
    }

    public static FigureAction CreateKillWithoutMove(this ITile unitTile, ITile targetTile, IBoard board)
    {
        return new FigureAction(
            FigureActionTypes.Attack, 
            unitTile.AbsolutePosition,
            targetTile.AbsolutePosition,
            () => unitTile.KillWithoutMove(targetTile, board));
    }

    public static FigureAction CreateKillWithMove(this ITile unitTile, ITile targetTile, IBoard board)
    {
        return new FigureAction(
            FigureActionTypes.Attack, 
            unitTile.AbsolutePosition,
            targetTile.AbsolutePosition,
            () =>
            {
                unitTile.KillWithMove(targetTile, board);
            });
    }

    public static FigureAction CreatePassTurn(this ITile tile)
    {
        return new FigureAction(
            FigureActionTypes.Special, 
            tile.AbsolutePosition,
            tile.AbsolutePosition,
            () => { });
    }
}
using BattleChess3.Core.Model;
using BattleChess3.Core.Model.Figures;
using BattleChess3.DefaultFigures.Utilities;

namespace BattleChess3.DefaultFigures;

public static class DefaultFigureActions
{
    public static FigureAction CreateAddFigureAction(this ITile targetTile, Figure createdFigure, ITile[] board)
    {
        return new FigureAction(FigureActionTypes.Special, () => targetTile.CreateFigure(createdFigure, board));
    }

    public static FigureAction CreateMoveAction(this ITile unitTile, ITile targetTile, ITile[] board)
    {
        return new FigureAction(FigureActionTypes.Move, () => unitTile.MoveToTile(targetTile, board));
    }

    public static FigureAction CreateKillWithoutMove(this ITile unitTile, ITile targetTile, ITile[] board)
    {
        return new FigureAction(FigureActionTypes.Attack, () => targetTile.Die(board));
    }

    public static FigureAction CreateKillWithMove(this ITile unitTile, ITile targetTile, ITile[] board)
    {
        return new FigureAction(FigureActionTypes.Attack, () =>
        {
            targetTile.Die(board);
            unitTile.MoveToTile(targetTile, board);
        });
    }

    public static FigureAction CreateSwapFigures(this ITile unitTile, ITile targetTile)
    {
        return new FigureAction(FigureActionTypes.Special, () => unitTile.SwapTiles(targetTile));
    }

    public static FigureAction CreatePassTurn(this ITile tile)
    {
        return new FigureAction(FigureActionTypes.Special, () => { });
    }
}
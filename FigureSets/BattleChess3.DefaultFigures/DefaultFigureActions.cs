﻿using BattleChess3.Core.Model;
using BattleChess3.Core.Model.Figures;
using BattleChess3.DefaultFigures.Utilities;

namespace BattleChess3.DefaultFigures;

public static class DefaultFigureActions
{
    public static FigureAction CreateAddFigureAction(this ITile targetTile, Figure createdFigure)
    {
        return new FigureAction(FigureActionTypes.Special, () => targetTile.CreateFigure(createdFigure));
    }

    public static FigureAction CreateMoveAction(this ITile unitTile, ITile targetTile)
    {
        return new FigureAction(FigureActionTypes.Move, () => unitTile.MoveToTile(targetTile));
    }

    public static FigureAction CreateKillWithoutMove(this ITile unitTile, ITile targetTile)
    {
        return new FigureAction(FigureActionTypes.Attack, targetTile.Die);
    }

    public static FigureAction CreateKillWithMove(this ITile unitTile, ITile targetTile)
    {
        return new FigureAction(FigureActionTypes.Attack, () =>
        {
            targetTile.Die();
            unitTile.MoveToTile(targetTile);
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
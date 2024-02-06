using BattleChess3.Core.Model;
using BattleChess3.Core.Model.Figures;

namespace BattleChess3.DefaultFigures.Utilities;

public static class FiguresHelper
{
    public static bool IsEmpty(this ITile figureType)
    {
        return figureType.Figure.FigureType.Equals(Empty.Instance);
    }

    public static bool IsWater(this ITile figureType)
    {
        return figureType.Figure.FigureType.Equals(Water.Instance);
    }

    public static bool IsOwnedByYou(this ITile checkedTile, ITile yoursTile)
    {
        return checkedTile.Figure.Owner.Equals(yoursTile.Figure.Owner);
    }

    public static bool IsOwnedByEnemy(this ITile checkedTile, ITile yoursTile)
    {
        return !checkedTile.Figure.Owner.Equals(Player.Neutral) &&
               !checkedTile.Figure.Owner.Equals(yoursTile.Figure.Owner);
    }

    public static void CreateFigure(this ITile tile, Figure createdFigure)
    {
        tile.Figure = createdFigure;
        tile.Figure.Owner.Figures.Add(tile.Figure);
    }

    public static void Die(this ITile tile)
    {
        tile.Figure.Owner.Figures.Remove(tile.Figure);
        tile.Figure = new Figure(Player.Neutral, Empty.Instance);
    }

    public static void MoveToTile(this ITile from, ITile to)
    {
        to.Figure = from.Figure;
        from.Figure = new Figure(Player.Neutral, Empty.Instance);
    }

    public static void SwapTiles(this ITile first, ITile second)
    {
        (second.Figure, first.Figure) = (first.Figure, second.Figure);
    }
}
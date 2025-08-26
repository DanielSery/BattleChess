using BattleChess3.Game.Figures;
using BattleChess3.Game.GameBoard;
using BattleChess3.Game.Players;

namespace BattleChess3.CrossFireFigures.Utilities;

internal static class FiguresHelper
{
    public static bool CanMoveTo(this ITile yourTile, ITile targetTile)
    {
        return targetTile.IsEmpty() || targetTile.IsFire();
    }

    public static bool CanAttack(this ITile yourTile, ITile targetTile)
    {
        if (targetTile.Figure.Owner.Equals(yourTile.Figure.Owner))
            return false;

        return !targetTile.IsEmpty() &&
               !targetTile.IsWall() &&
               !targetTile.IsFire();
    }

    public static bool IsWall(this ITile tile)
    {
        return tile.Figure.Type is Wall;
    }

    public static bool IsFire(this ITile tile)
    {
        return tile.Figure.Type is Fire;
    }
    
    public static bool IsEmpty(this ITile tile)
    {
        return tile.Figure.Type is Empty;
    }

    public static bool IsAllyTo(this ITile yoursTile, ITile checkedTile)
    {
        return checkedTile.Figure.Owner.Equals(yoursTile.Figure.Owner);
    }

    public static bool IsEnemyTo(this ITile yoursTile, ITile checkedTile)
    {
        return !checkedTile.Figure.Owner.Equals(yoursTile.Figure.Owner) &&
               !checkedTile.Figure.Owner.Equals(Player.Neutral);
    }

    public static void CreateFigure(this ITile tile, Figure createdFigure, IBoard board)
    {
        tile.Figure = createdFigure;
        tile.Figure.Owner.Figures.Add(tile.Figure);
        tile.Figure.Type.OnCreated(tile, board);
    }

    public static void Die(this ITile tile, IBoard board)
    {
        var figureType = tile.Figure.Type;
        figureType.OnDying(tile, board); 
        tile.Figure.Owner.Figures.Remove(tile.Figure);
        tile.Figure = new Figure(Player.Neutral, CrossFireFigureGroup.Empty, false);
        figureType.OnDied(tile, board);
    }

    public static void SwapWithTile(this ITile from, ITile to, IBoard board)
    {
        var movingFigure = from.Figure.Type;
        var targetFigure = to.Figure.Type;

        movingFigure.OnMoving(from, to, board);
        targetFigure.OnMoving(to, from, board);

        (to.Figure, from.Figure) = (from.Figure, to.Figure);

        movingFigure.OnMoved(from, to, board);
        targetFigure.OnMoved(to, from, board);
    }

    public static void MoveToTile(this ITile from, ITile to, IBoard board)
    {
        var movingFigure = from.Figure.Type; 
        movingFigure.OnMoving(from, to, board);
        
        to.Figure.Owner.Figures.Remove(to.Figure);
        to.Figure = from.Figure;
        from.Figure = new Figure(Player.Neutral, CrossFireFigureGroup.Empty, false);
        
        movingFigure.OnMoved(from, to, board);
    }

    public static void KillWithoutMove(this ITile from, ITile to, IBoard board)
    {
        var attackingFigure = from.Figure.Type; 
        var killedFigure = to.Figure.Type;
        
        attackingFigure.OnAttacking(from, to, board);
        killedFigure.OnDying(to, board);
        killedFigure.OnBeingAttacked(to, from, board);
        
        to.Figure.Owner.Figures.Remove(to.Figure);
        to.Figure = new Figure(Player.Neutral, CrossFireFigureGroup.Empty, false);
        
        killedFigure.OnDied(to, board);
        killedFigure.OnKilled(to, from, board);
        attackingFigure.OnAttacked(from, to, board);
    }

    public static void KillWithMove(this ITile from, ITile to, IBoard board)
    {
        var attackingFigure = from.Figure.Type; 
        var killedFigure = to.Figure.Type;
        
        attackingFigure.OnMoving(from, to, board);
        attackingFigure.OnAttacking(from, to, board);
        killedFigure.OnDying(to, board);
        killedFigure.OnBeingAttacked(to, from, board);
        
        to.Figure.Owner.Figures.Remove(to.Figure);
        to.Figure = from.Figure;
        from.Figure = new Figure(Player.Neutral, CrossFireFigureGroup.Empty, false);
        
        attackingFigure.OnMoved(from, to, board);
        killedFigure.OnDied(to, board);
        killedFigure.OnKilled(to, from, board);
        attackingFigure.OnAttacked(from, to, board);
    }
}
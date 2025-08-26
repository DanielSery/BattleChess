using BattleChess3.Game.GameBoard;

namespace BattleChess3.CrossFireFigures.Utilities;

internal static class BoardHelper
{
    public static IEnumerable<ITile> GetRelativeTiles(this IEnumerable<Position> positions, IBoard board, ITile tile)
    {
        foreach (var position in positions)
        {
            if (board.TryGetRelativeTile(tile, position, out var resultTile))
            {
                yield return resultTile;
            }
        }
    }
    
    public static IEnumerable<ITile> GetRelativeDirectionTiles(this Position direction, int fromInclusive, int toInclusive, IBoard board, ITile tile)
    {
        for (var i = fromInclusive; i <= toInclusive; i++)
        {
            if (!board.TryGetRelativeTile(tile, direction * i, out var targetTile))
                break;
            
            yield return targetTile;
        }
    }

    public static bool TryGetRelativeTile(this IBoard board, ITile fromTile, Position relativePosition, out ITile tile)
    {
        var targetPosition = fromTile.Position + relativePosition;
        return board.TryGetTile(targetPosition, out tile);
    }

    public static bool TryGetRelativeDirectionTile(this IBoard board, ITile fromTile, Position direction, int index,
        out ITile tile)
    {
        var targetPosition = fromTile.Position + direction * index;
        return board.TryGetTile(targetPosition, out tile);
    }

}
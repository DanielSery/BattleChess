using BattleChess3.Game.Board;

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
    
    public static bool TryGetRelativeTile(this IBoard board, ITile fromTile, Position relativePosition, out ITile tile)
    {
        var targetPosition = fromTile.Position + relativePosition;
        return board.TryGetTile(targetPosition, out tile);
    }
}
using CrownsGuard.Core;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.Core.SimulatedBoard;

namespace CrownsGuard.FigureDefinitions.Utilities;

internal static class BoardHelper
{
    public static bool IsWalkableTile(this Figure[] board, Position position)
    {
        var index = position.GetIndex();
        if (index < 0 || index >= board.Length || 
            position.X is < 0 or >= Constants.BoardLength)
        {
            return false;
        }

        return board[index].IsWalkable();
    }
    
    public static bool IsEmptyTile(this Figure[] board, Position position)
    {
        var index = position.GetIndex();
        if (index < 0 || index >= board.Length || 
            position.X is < 0 or >= Constants.BoardLength)
        {
            return false;
        }

        return board[index].IsEmpty();
    }

    public static bool TryGetFigure(this Figure[] board, Position position, out Figure tile)
    {
        var index = position.GetIndex();
        if (index < 0 || index >= board.Length || 
            position.X is < 0 or >= Constants.BoardLength)
        {
            tile = new Figure();
            return false;
        }

        tile = board[index];
        return true;
    }
}
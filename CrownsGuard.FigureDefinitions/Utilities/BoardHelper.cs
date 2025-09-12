using System.Diagnostics;
using CrownsGuard.Core;
using CrownsGuard.Core.Figures;
using CrownsGuard.Core.GameBoard;

namespace CrownsGuard.FigureDefinitions.Utilities;

internal static class BoardHelper
{
    public static bool IsInBoard(this Position position)
    {
        if (position.Y is < 0 or >= Constants.BoardLength ||
            position.X is < 0 or >= Constants.BoardLength)
        {
            return false;
        }

        return true;
    }

    public static bool TryGetFigure(this ReadOnlySpan<Figure> board, Position position, out Figure tile)
    {
        Debug.Assert(board.Length > 0);

        if (position.Y is < 0 or >= Constants.BoardLength ||
            position.X is < 0 or >= Constants.BoardLength)
        {
            tile = new Figure();
            return false;
        }

        tile = board[position.GetIndex()];
        return true;
    }

    public static bool TryGetFigure(this Span<Figure> board, Position position, out Figure tile)
    {
        Debug.Assert(board.Length > 0);
        
        if (position.Y is < 0 or >= Constants.BoardLength || 
            position.X is < 0 or >= Constants.BoardLength)
        {
            tile = new Figure();
            return false;
        }

        tile = board[position.GetIndex()];
        return true;
    }
}
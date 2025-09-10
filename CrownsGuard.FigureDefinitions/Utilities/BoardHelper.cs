using CrownsGuard.Core;
using CrownsGuard.Core.Figures;
using CrownsGuard.Core.GameBoard;

namespace CrownsGuard.FigureDefinitions.Utilities;

internal static class BoardHelper
{
    public static bool TryGetFigure(this Span<Figure> board, Position position, out Figure tile)
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
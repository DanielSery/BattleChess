using CrownsGuard.Core.Figures;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.Core.Helpers;
using CrownsGuard.Engine.Helpers;

namespace CrownsGuard.FigureDefinitions.Figures;

public static class Fire
{
    public static void OnDied(BoardEvent boardEvent, Span<Figure> board, Action<BoardEvent, Span<Figure>> onEvent)
    {
        var figure = board[boardEvent.SourceIndex];

        if (figure.GetFigureType() == Figure.Dragon)
        {
            return;
        }
        
        board.Die(boardEvent.SourceIndex, onEvent);
    }
} 

using CrownsGuard.Core.Board;
using CrownsGuard.Core.Figures;
using CrownsGuard.Core.Helpers;
using CrownsGuard.Engine.Helpers;

namespace CrownsGuard.Engine.Figures;

public static class Fire
{
    public static void OnMovedToFire(BoardEvent boardEvent, Span<Figure> board, Action<BoardEvent, Span<Figure>> onEvent)
    {
        var figure = board[boardEvent.TargetIndex];

        if (figure.GetFigureType() == Figure.Dragon)
        {
            return;
        }
        
        board.Die(boardEvent.TargetIndex, onEvent);
    }
} 

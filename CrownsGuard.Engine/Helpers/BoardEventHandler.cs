using CrownsGuard.Core.Board;
using CrownsGuard.Core.Figures;
using CrownsGuard.Engine.Figures;

namespace CrownsGuard.Engine.Helpers;

public static class BoardEventHandler
{
    public static void HandleFigureActionEvent(BoardEvent boardEvent, Span<Figure> board)
    {
        if (boardEvent is { EventType: BoardEventType.Moved, TargetedFigure: Figure.Fire })
        {
            Fire.OnMovedToFire(boardEvent, board, HandleFigureActionEvent);
        }
        else if (boardEvent is { EventType: BoardEventType.Attacked, TargetedFigure: Figure.Explosives })
        {
            Explosives.OnAttacked(boardEvent, board, HandleFigureActionEvent);
        }
    }
    
    public static void HandleFigureActionEvent(BoardEvent boardEvent, Span<Figure> board, Action<BoardEvent, Span<Figure>> onEvent)
    {
        if (boardEvent is { EventType: BoardEventType.Moved, TargetedFigure: Figure.Fire })
        {
            Fire.OnMovedToFire(boardEvent, board, onEvent);
        }
        else if (boardEvent is { EventType: BoardEventType.Attacked, TargetedFigure: Figure.Explosives })
        {
            Explosives.OnAttacked(boardEvent, board, onEvent);
        }
    } 
}
using CrownsGuard.Core.Figures;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.Core.Helpers;
using CrownsGuard.FigureDefinitions.Utilities;

namespace CrownsGuard.FigureDefinitions.Figures;

public class Explosives : ICrownsGuardFigureTypeInfo
{
    public FigureId FigureId => FigureId.Explosives;

    public IDictionary<int, Uri> ImageUris =>
        new Dictionary<int, Uri>
        {
            { 0, new Uri($"pack://application:,,,/CrownsGuard.FigureDefinitions;component/Images/{GetType().Name}.png", UriKind.Absolute) },
        };

    public static ArrayPoolMemory<FigureAction> GetPossibleActions(Position sourcePosition, Figure sourceFigure, ReadOnlySpan<Figure> board)
    {
        return ArrayPoolMemory<FigureAction>.Empty;
    }

    public static void OnAttacked(BoardEvent boardEvent, Figure[] board, Action<BoardEvent, Span<Figure>> onEvent)
    {
        TryDie(board, boardEvent.SourcePosition + new Position(-1, -1), onEvent);
        TryDie(board,boardEvent.SourcePosition + new Position(-1, 0), onEvent);
        TryDie(board,boardEvent.SourcePosition + new Position(-1, 1), onEvent);
        TryDie(board,boardEvent.SourcePosition + new Position(0, -1), onEvent);
        TryDie(board,boardEvent.SourcePosition + new Position(0, 1), onEvent);
        TryDie(board,boardEvent.SourcePosition + new Position(1, -1), onEvent);
        TryDie(board,boardEvent.SourcePosition + new Position(1, 0), onEvent);
        TryDie(board,boardEvent.SourcePosition + new Position(1, 1), onEvent);
    }

    private static void TryDie(Span<Figure> board, Position position, Action<BoardEvent, Span<Figure>> onEvent)
    {
        if (!board.TryGetFigure(position, out Figure _))
            return;
        
        board.Die(position, onEvent);
    }
}
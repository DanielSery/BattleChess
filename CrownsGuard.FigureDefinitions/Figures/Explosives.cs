using CrownsGuard.Core.Figures;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.FigureDefinitions.Utilities;

namespace CrownsGuard.FigureDefinitions.Figures;

public class Explosives : ICrownsGuardFigureTypeInfo
{
    public Figure Figure => Figure.Explosives;

    public IDictionary<int, Uri> ImageUris =>
        new Dictionary<int, Uri>
        {
            { 0, new Uri($"pack://application:,,,/CrownsGuard.FigureDefinitions;component/Images/{GetType().Name}.png", UriKind.Absolute) },
        };

    public static void OnAttacked(BoardEvent boardEvent, Figure[] board, Action<BoardEvent, Span<Figure>> onEvent)
    {
        TryDestroyTile(board, boardEvent.SourceIndex,  new Position(-1, -1), onEvent);
        TryDestroyTile(board, boardEvent.SourceIndex,  new Position(-1, 0), onEvent);
        TryDestroyTile(board, boardEvent.SourceIndex,  new Position(-1, 1), onEvent);
        TryDestroyTile(board, boardEvent.SourceIndex,  new Position(0, -1), onEvent);
        TryDestroyTile(board, boardEvent.SourceIndex,  new Position(0, 1), onEvent);
        TryDestroyTile(board, boardEvent.SourceIndex,  new Position(1, -1), onEvent);
        TryDestroyTile(board, boardEvent.SourceIndex,  new Position(1, 0), onEvent);
        TryDestroyTile(board, boardEvent.SourceIndex,  new Position(1, 1), onEvent);
    }
    
    private static void TryDestroyTile(Span<Figure> board, int sourceIndex, Position relative,
        Action<BoardEvent, Span<Figure>> onEvent)
    {
        var targetIndex = sourceIndex.GetWithOffset(relative);
        if (targetIndex == -1) return;

        board.KillWithoutMove((byte)sourceIndex, (byte)targetIndex, onEvent);
    }
}
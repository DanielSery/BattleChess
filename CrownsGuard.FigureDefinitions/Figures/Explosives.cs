using CrownsGuard.Core.Figures;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.Core.Players;
using CrownsGuard.Core.SimulatedBoard;
using CrownsGuard.FigureDefinitions.Utilities;

namespace CrownsGuard.FigureDefinitions.Figures;

public class Explosives : ICrownsGuardFigureType
{
    public int FigureValue => 1;
    public FigureId FigureId => FigureId.Explosives;

    public IDictionary<int, Uri> ImageUris =>
        new Dictionary<int, Uri>
        {
            { 0, new Uri($"pack://application:,,,/CrownsGuard.FigureDefinitions;component/Images/{GetType().Name}.png", UriKind.Absolute) },
        };

    public static FigureAction[] GetPossibleActions(Position sourcePosition, Figure sourceFigure, Figure[] board)
    {
        return [];
    }

    public static void OnAttacked(BoardEvent boardEvent, Figure[] board, Action<BoardEvent, Figure[]> onEvent)
    {
        TryDie(board, boardEvent.Position + new Position(-1, -1), onEvent);
        TryDie(board,boardEvent.Position + new Position(-1, 0), onEvent);
        TryDie(board,boardEvent.Position + new Position(-1, 1), onEvent);
        TryDie(board,boardEvent.Position + new Position(0, -1), onEvent);
        TryDie(board,boardEvent.Position + new Position(0, 1), onEvent);
        TryDie(board,boardEvent.Position + new Position(1, -1), onEvent);
        TryDie(board,boardEvent.Position + new Position(1, 0), onEvent);
        TryDie(board,boardEvent.Position + new Position(1, 1), onEvent);
    }

    private static void TryDie(Figure[] board, Position position, Action<BoardEvent, Figure[]> onEvent)
    {
        if (!board.TryGetFigure(position, out Figure _))
            return;
        
        board.Die(position, onEvent);
    }
}
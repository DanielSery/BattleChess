using CrownsGuard.Core.Figures;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.Core.SimulatedBoard;
using CrownsGuard.FigureDefinitions.Utilities;

namespace CrownsGuard.FigureDefinitions.Figures;

public class Fire : ICrownsGuardFigureType
{
    public int FigureValue => 0;
    public FigureId FigureId => FigureId.Fire;

    public IDictionary<int, Uri> ImageUris =>
        new Dictionary<int, Uri>
        {
            { 0, new Uri($"pack://application:,,,/CrownsGuard.FigureDefinitions;component/Images/{GetType().Name}.png", UriKind.Absolute) },
        };

    public static FigureAction[] GetPossibleActions(Position sourcePosition, Figure sourceFigure, Figure[] board)
    {
        return [];
    }

    public static void OnDied(BoardEvent boardEvent, Figure[] board, Action<BoardEvent, Figure[]> onEvent)
    {
        if (!board.TryGetFigure(boardEvent.Position, out Figure figure))
            return;

        if (figure.FigureType == FigureId.Dragon)
            return;
        
        board.Die(boardEvent.Position, onEvent);
    }
} 

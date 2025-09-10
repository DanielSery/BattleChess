using CrownsGuard.Core.Figures;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.Core.Helpers;
using CrownsGuard.FigureDefinitions.Utilities;

namespace CrownsGuard.FigureDefinitions.Figures;

public class Fire : ICrownsGuardFigureTypeInfo
{
    public int FigureValue => 0;
    public FigureId FigureId => FigureId.Fire;

    public IDictionary<int, Uri> ImageUris =>
        new Dictionary<int, Uri>
        {
            { 0, new Uri($"pack://application:,,,/CrownsGuard.FigureDefinitions;component/Images/{GetType().Name}.png", UriKind.Absolute) },
        };

    public static ArrayPoolMemory<FigureAction> GetPossibleActions(Position sourcePosition, Figure sourceFigure, Span<Figure> board)
    {
        return ArrayPoolMemory<FigureAction>.Empty;
    }

    public static void OnDied(BoardEvent boardEvent, Span<Figure> board, Action<BoardEvent, Span<Figure>> onEvent)
    {
        if (!board.TryGetFigure(boardEvent.SourcePosition, out Figure figure))
        {
            return;
        }

        if (figure.FigureType == FigureId.Dragon)
        {
            return;
        }
        
        board.Die(boardEvent.SourcePosition, onEvent);
    }
} 

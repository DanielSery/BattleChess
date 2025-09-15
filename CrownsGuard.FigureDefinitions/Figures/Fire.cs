using CrownsGuard.Core.Figures;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.Core.Helpers;
using CrownsGuard.FigureDefinitions.Utilities;

namespace CrownsGuard.FigureDefinitions.Figures;

public class Fire : ICrownsGuardFigureTypeInfo
{
    public Figure Figure => Figure.Fire;

    public IDictionary<int, Uri> ImageUris =>
        new Dictionary<int, Uri>
        {
            { 0, new Uri($"pack://application:,,,/CrownsGuard.FigureDefinitions;component/Images/{GetType().Name}.png", UriKind.Absolute) },
        };

    public static void OnDied(BoardEvent boardEvent, Span<Figure> board, Action<BoardEvent, Span<Figure>> onEvent)
    {
        if (!board.TryGetFigure(boardEvent.SourcePosition, out Figure figure))
        {
            return;
        }

        if (figure.GetFigureType() == Figure.Dragon)
        {
            return;
        }
        
        board.Die(boardEvent.SourcePosition, onEvent);
    }
} 

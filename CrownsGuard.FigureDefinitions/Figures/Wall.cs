using CrownsGuard.Core.Figures;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.Core.Helpers;
using CrownsGuard.FigureDefinitions.Utilities;

namespace CrownsGuard.FigureDefinitions.Figures;

public class Wall : ICrownsGuardFigureTypeInfo
{
    public int FigureValue => 1;
    public FigureId FigureId => FigureId.Wall;

    public static ArrayPoolMemory<FigureAction> GetPossibleActions(Position sourcePosition, Figure sourceFigure, Span<Figure> board)
    {
        return ArrayPoolMemory<FigureAction>.Empty;
    }

    public IDictionary<int, Uri> ImageUris =>
        new Dictionary<int, Uri>
        {
            { 0, new Uri($"pack://application:,,,/CrownsGuard.FigureDefinitions;component/Images/{GetType().Name}.png", UriKind.Absolute) },
        };
}
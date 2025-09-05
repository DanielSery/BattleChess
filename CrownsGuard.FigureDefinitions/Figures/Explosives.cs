using CrownsGuard.Core.Figures;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.Core.Players;
using CrownsGuard.FigureDefinitions.Utilities;

namespace CrownsGuard.FigureDefinitions.Figures;

public class Explosives : ICrossFireFigureType
{
    public int FigureValue => 1;

    public int FigureId => CrossFireFigureIds.ExplosivesId;
    
    public IDictionary<int, Uri> ImageUris =>
        new Dictionary<int, Uri>
        {
            { 0, new Uri($"pack://application:,,,/CrownsGuard.FigureDefinitions;component/Images/{GetType().Name}.png", UriKind.Absolute) },
        };
    
    public IEnumerable<FigureAction> GetPossibleActions(ITile unitTile, IBoard board)
    {
        return [];
    }

    public void OnBeingAttacked(ITile unitTile, ITile attackingTile, IBoard board)
    {
        SilentDie(board, unitTile.RelativePosition + new Position(-1, -1));
        SilentDie(board, unitTile.RelativePosition + new Position(-1, 0));
        SilentDie(board, unitTile.RelativePosition + new Position(-1, 1));
        SilentDie(board, unitTile.RelativePosition + new Position(0, -1));
        SilentDie(board, unitTile.RelativePosition + new Position(0, 1));
        SilentDie(board, unitTile.RelativePosition + new Position(1, -1));
        SilentDie(board, unitTile.RelativePosition + new Position(1, 0));
        SilentDie(board, unitTile.RelativePosition + new Position(1, 1));
    }

    public void OnKilled(ITile unitTile, ITile attackingTile, IBoard board)
    {
        unitTile.Die(board);
    }

    private static void SilentDie(IBoard board, Position position)
    {
        if (!board.TryGetTile(position, out var tile))
            return;

        tile.Figure.Owner.Figures.Remove(tile.Figure);
        tile.Figure = new Figure(NeutralFigureOwner.Instance, CrossFireFigureGroup.Empty, false);
    }
}
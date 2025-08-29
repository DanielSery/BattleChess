using BattleChess3.CrossFireFigures.Utilities;
using BattleChess3.Game.Figures;
using BattleChess3.Game.GameBoard;
using BattleChess3.Game.Players;

namespace BattleChess3.CrossFireFigures.Figures;

public class Explosives : ICrossFireFigureType
{
    /// <inheritdoc />
    public int FigureValue { get; } = 1;
    
    int IFigureType.FigureId => CrossFireFigureIds.ExplosivesId;
    
    IDictionary<int, Uri> IFigureType.ImageUris =>
        new Dictionary<int, Uri>
        {
            { 0, new Uri($"pack://application:,,,/BattleChess3.CrossFireFigures;component/Images/{GetType().Name}.png", UriKind.Absolute) },
        };
    
    public IEnumerable<FigureAction> GetPossibleActions(ITile unitTile, IBoard board)
    {
        return [];
    }

    void IFigureType.OnBeingAttacked(ITile unitTile, ITile attackingTile, IBoard board)
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

    void IFigureType.OnKilled(ITile unitTile, ITile attackingTile, IBoard board)
    {
        unitTile.Die(board);
    }

    private static void SilentDie(IBoard board, Position position)
    {
        if (!board.TryGetTile(position, out var tile))
            return;

        tile.Figure.Owner.Figures.Remove(tile.Figure);
        tile.Figure = new Figure(NeutralPlayerInfo.Instance, CrossFireFigureGroup.Empty, false);
    }
}
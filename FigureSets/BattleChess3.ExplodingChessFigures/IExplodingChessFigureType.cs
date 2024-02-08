using BattleChess3.Core.Model;
using BattleChess3.Core.Model.Figures;
using BattleChess3.DefaultFigures;
using BattleChess3.ExplodingChessFigures.Localization;

namespace BattleChess3.ExplodingChessFigures;

internal interface IExplodingChessFigureType : IFigureType
{
    string IFigureType.UnitName => $"{nameof(ExplodingChessFigureGroup)}.{GetType().Name}";
    string IFigureType.DisplayName => CurrentLocalization.Instance[$"{GetType().Name}_Name"];
    string IFigureType.Description => CurrentLocalization.Instance[$"{GetType().Name}_{nameof(Description)}"];

    IDictionary<int, Uri> IFigureType.ImageUris =>
        new Dictionary<int, Uri>
        {
            { 1, new Uri($"pack://application:,,,/BattleChess3.ExplodingChessFigures;component/Images/{GetType().Name}1.png", UriKind.Absolute) },
            { 2, new Uri($"pack://application:,,,/BattleChess3.ExplodingChessFigures;component/Images/{GetType().Name}2.png", UriKind.Absolute) }
        };

    void IFigureType.OnDied(ITile unitTile, ITile[] board)
    {
        SilentDie(board, unitTile.Position + new Position(-1, -1));
        SilentDie(board, unitTile.Position + new Position(-1, 0));
        SilentDie(board, unitTile.Position + new Position(-1, 1));
        SilentDie(board, unitTile.Position + new Position(0, -1));
        SilentDie(board, unitTile.Position + new Position(0, 1));
        SilentDie(board, unitTile.Position + new Position(1, -1));
        SilentDie(board, unitTile.Position + new Position(1, 0));
        SilentDie(board, unitTile.Position + new Position(1, 1));
    }

    private static void SilentDie(ITile[] board, Position position)
    {
        if (position.IsOutsideBoard())
            return;

        var tile = board[position];
        tile.Figure.Owner.Figures.Remove(tile.Figure);
        tile.Figure = new Figure(Player.Neutral, DefaultFigureGroup.Empty);
    }
}
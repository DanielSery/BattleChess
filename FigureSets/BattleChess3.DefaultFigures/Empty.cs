using BattleChess3.DefaultFigures.Localization;
using BattleChess3.Game.Board;
using BattleChess3.Game.Figures;

namespace BattleChess3.DefaultFigures;

public class Empty : IFigureType
{
    int IFigureType.FigureId => 0;
    int IFigureType.SetId => 0;
    
    string IFigureType.DisplayName => CurrentLocalization.Instance[$"{GetType().Name}_{nameof(IFigureType.DisplayName)}"];
    string IFigureType.BaseDescription => CurrentLocalization.Instance[$"{GetType().Name}_{nameof(IFigureType.BaseDescription)}"];
    string IFigureType.MovementDescription => CurrentLocalization.Instance[$"{GetType().Name}_{nameof(IFigureType.MovementDescription)}"];
    string IFigureType.AttackDescription => CurrentLocalization.Instance[$"{GetType().Name}_{nameof(IFigureType.AttackDescription)}"];
    string IFigureType.SpecialDescription => CurrentLocalization.Instance[$"{GetType().Name}_{nameof(IFigureType.SpecialDescription)}"];

    IDictionary<int, Uri> IFigureType.ImageUris =>
        new Dictionary<int, Uri>
        {
            { 0, new Uri($"pack://application:,,,/BattleChess3.DefaultFigures;component/Images/{GetType().Name}0.png", UriKind.Absolute) }
        };

    public IEnumerable<FigureAction> GetPossibleActions(ITile unitTile, IBoard board)
    {
        return [];
    }
}
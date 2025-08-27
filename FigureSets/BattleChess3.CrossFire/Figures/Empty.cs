using BattleChess3.CrossFireFigures.Localization;
using BattleChess3.Game.Figures;
using BattleChess3.Game.GameBoard;

namespace BattleChess3.CrossFireFigures.Figures;

public class Empty : IFigureType
{
    /// <inheritdoc />
    public int FigureValue { get; } = 0;
    
    int IFigureType.FigureId => CrossFireFigureIds.EmptyId;
    
    string IFigureType.DisplayName => CurrentLocalization.Instance[$"{GetType().Name}_{nameof(IFigureType.DisplayName)}"];
    string IFigureType.BaseDescription => CurrentLocalization.Instance[$"{GetType().Name}_{nameof(IFigureType.BaseDescription)}"];
    string IFigureType.MovementDescription => CurrentLocalization.Instance[$"{GetType().Name}_{nameof(IFigureType.MovementDescription)}"];
    string IFigureType.AttackDescription => CurrentLocalization.Instance[$"{GetType().Name}_{nameof(IFigureType.AttackDescription)}"];
    string IFigureType.SpecialDescription => CurrentLocalization.Instance[$"{GetType().Name}_{nameof(IFigureType.SpecialDescription)}"];

    IDictionary<int, Uri> IFigureType.ImageUris =>
        new Dictionary<int, Uri>
        {
            { 0, new Uri($"pack://application:,,,/BattleChess3.CrossFireFigures;component/Images/{GetType().Name}.png", UriKind.Absolute) },
        };

    public IEnumerable<FigureAction> GetPossibleActions(ITile unitTile, IBoard board)
    {
        return [];
    }
}
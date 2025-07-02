using BattleChess3.DefaultFigures.Localization;
using BattleChess3.Game.Figures;

namespace BattleChess3.DefaultFigures;

internal interface IDefaultFigureType : IFigureType
{
    int IFigureType.SetId => 0;
    string IFigureType.DisplayName => CurrentLocalization.Instance[$"{GetType().Name}_{nameof(DisplayName)}"];
    string IFigureType.BaseDescription => CurrentLocalization.Instance[$"{GetType().Name}_{nameof(BaseDescription)}"];
    string IFigureType.MovementDescription => CurrentLocalization.Instance[$"{GetType().Name}_{nameof(MovementDescription)}"];
    string IFigureType.AttackDescription => CurrentLocalization.Instance[$"{GetType().Name}_{nameof(AttackDescription)}"];
    string IFigureType.SpecialDescription => CurrentLocalization.Instance[$"{GetType().Name}_{nameof(SpecialDescription)}"];

    IDictionary<int, Uri> IFigureType.ImageUris =>
        new Dictionary<int, Uri>
        {
            { 0, new Uri($"pack://application:,,,/BattleChess3.DefaultFigures;component/Images/{GetType().Name}0.png", UriKind.Absolute) }
        };
}
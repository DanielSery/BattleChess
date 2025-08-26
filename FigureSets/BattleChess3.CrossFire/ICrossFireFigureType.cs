using BattleChess3.CrossFireFigures.Localization;
using BattleChess3.Game.Figures;
using BattleChess3.Game.GameBoard;

namespace BattleChess3.CrossFireFigures;

internal interface ICrossFireFigureType : IFigureType
{
    protected static readonly Position[] NeighbourPositions =
    [
        new(-1, -1), new(-1, 0), new(-1, 1),
        new(0, -1), new(0, 1),
        new(1, -1), new(1, 0), new(1, 1)
    ];
    
    string IFigureType.DisplayName => CurrentLocalization.Instance[$"{GetType().Name}_{nameof(DisplayName)}"];
    string IFigureType.BaseDescription => string.Format(CurrentLocalization.Instance[$"{GetType().Name}_{nameof(BaseDescription)}"], FigureValue);
    string IFigureType.MovementDescription => CurrentLocalization.Instance[$"{GetType().Name}_{nameof(MovementDescription)}"];
    string IFigureType.AttackDescription => CurrentLocalization.Instance[$"{GetType().Name}_{nameof(AttackDescription)}"];
    string IFigureType.SpecialDescription => CurrentLocalization.Instance[$"{GetType().Name}_{nameof(SpecialDescription)}"];

    IDictionary<int, Uri> IFigureType.ImageUris =>
        new Dictionary<int, Uri>
        {
            { 1, new Uri($"pack://application:,,,/BattleChess3.CrossFireFigures;component/Images/{GetType().Name}1.png", UriKind.Absolute) },
            { 2, new Uri($"pack://application:,,,/BattleChess3.CrossFireFigures;component/Images/{GetType().Name}2.png", UriKind.Absolute) }
        };
}
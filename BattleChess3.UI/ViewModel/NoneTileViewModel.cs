using BattleChess3.Core.Model;
using BattleChess3.Core.Model.Figures;

namespace BattleChess3.UI.ViewModel;

public class NoneTileViewModel : ITileViewModel
{
    public static readonly NoneTileViewModel Instance = new();
    
    public Position Position => Position.None;
    public Position AbsolutePosition => Position.None;
    public Figure Figure { get; set; } = Figure.None;
    public bool IsMouseOver { get; set; }
    public bool IsSelected { get; set; }
    public bool IsPossibleAttack { get; set; }
    public bool IsPossibleMove { get; set; }
    public bool IsPossibleSpecial { get; set; }
    public FigureAction PossibleAction { get; set; } = FigureAction.None;
    public ITile GetPovTile(Player player) => this;
}

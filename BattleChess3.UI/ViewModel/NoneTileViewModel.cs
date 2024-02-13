using BattleChess3.Game.Board;
using BattleChess3.Game.Figures;
using BattleChess3.Game.Players;

namespace BattleChess3.UI.ViewModel;

public class NoneTileViewModel : TileViewModel
{
    public static readonly NoneTileViewModel Instance = new();

    public NoneTileViewModel() : base(new Position(-1, -1))
    {
    }
}
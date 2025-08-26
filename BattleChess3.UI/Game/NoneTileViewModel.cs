using BattleChess3.Game.GameBoard;
using BattleChess3.UI.Shared;

namespace BattleChess3.UI.Game;

public sealed class NoneTileViewModel : TileViewModel
{
    public static readonly NoneTileViewModel Instance = new();

    private NoneTileViewModel() : base(new Position(-1, -1))
    {
    }
}
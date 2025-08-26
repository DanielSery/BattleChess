using BattleChess3.Game.Figures;
using BattleChess3.Game.Players;

namespace BattleChess3.Game.GameBoard;

public class NoneTile : ITile
{
    public static readonly ITile Instance = new NoneTile();
    
    public Position Position => Position.None;
    public Position AbsolutePosition => Position.None;

    public Figure Figure
    {
        get => Figure.None;
        set { }
    }

    public ITile GetRelativeTile(PlayerInfo player)
    {
        return Instance;
    }

    /// <inheritdoc />
    public void OnDied()
    {
    }

    /// <inheritdoc />
    public void OnMovedFrom()
    {
    }

    /// <inheritdoc />
    public void OnMovedTo()
    {
    }

    /// <inheritdoc />
    public void OnCreated()
    {
    }
}
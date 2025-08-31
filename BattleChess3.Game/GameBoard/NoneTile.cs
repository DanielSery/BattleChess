using BattleChess3.Core.Figures;
using BattleChess3.Core.GameBoard;
using BattleChess3.Core.Players;

namespace BattleChess3.Game.GameBoard;

public class NoneTile : ITile
{
    public static readonly ITile Instance = new NoneTile();

    private NoneTile() { }
    
    public Position RelativePosition => Position.None;
    public Position AbsolutePosition => Position.None;

    public IFigure Figure
    {
        get => BattleChess3.Core.Figures.Figure.None;
        set { }
    }

    public ITile GetRelativeTile(Player player)
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
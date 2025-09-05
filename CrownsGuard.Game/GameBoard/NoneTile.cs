using CrownsGuard.Core.Figures;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.Core.Players;

namespace CrownsGuard.Game.GameBoard;

public class NoneTile : ITile
{
    public static readonly ITile Instance = new NoneTile();

    private NoneTile() { }
    
    public Position RelativePosition => Position.None;
    public Position AbsolutePosition => Position.None;

    public IFigure Figure
    {
        get => CrownsGuard.Core.Figures.Figure.None;
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
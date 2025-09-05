using CrownsGuard.Core.Figures;

namespace CrownsGuard.Core.Players;

public class NeutralFigureOwner : IFigureOwner
{
    public static readonly NeutralFigureOwner Instance = new NeutralFigureOwner();

    private NeutralFigureOwner() { }

    /// <inheritdoc />
    public Player Player => Player.Neutral;

    /// <inheritdoc />
    public List<IFigure> Figures { get; } = [];
}
using BattleChess3.Core.Figures;

namespace BattleChess3.Core.Players;

public class NeutralFigureOwner : IFigureOwner
{
    public static readonly NeutralFigureOwner Instance = new NeutralFigureOwner();

    private NeutralFigureOwner() { }

    /// <inheritdoc />
    public Player Player => Player.Neutral;

    /// <inheritdoc />
    public List<IFigure> Figures { get; } = [];
}
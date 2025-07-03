using BattleChess3.Game.Players;

namespace BattleChess3.Game.Figures;

public interface IFigure
{
    public Guid Id { get; }
    public Player Owner { get; }
    public IFigureType Type { get; }
    public bool IsKing { get; }
}
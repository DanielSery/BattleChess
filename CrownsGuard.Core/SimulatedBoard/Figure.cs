using CrownsGuard.Core.Players;

namespace CrownsGuard.Core.SimulatedBoard;

public readonly struct Figure
{
    public readonly Player Player;
    public readonly bool IsKing;
    public readonly FigureType FigureType;

    public Figure(Player player, bool isKing, FigureType figureType)
    {
        Player = player;
        IsKing = isKing;
        FigureType = figureType;
    }
}
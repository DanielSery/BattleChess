using CrownsGuard.Core.Players;

namespace CrownsGuard.Core.SimulatedBoard;

public readonly struct Figure
{
    public readonly Player Player;
    public readonly bool IsKing;
    public readonly FigureId FigureType;

    public Figure(Player player, bool isKing, FigureId figureType)
    {
        Player = player;
        IsKing = isKing;
        FigureType = figureType;
    }
}
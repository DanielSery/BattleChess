using CrownsGuard.Core.Players;

namespace CrownsGuard.Core.Figures;

public readonly struct Figure
{
    public readonly PlayerColor PlayerColor;
    public readonly bool IsKing;
    public readonly FigureId FigureType;

    public Figure(PlayerColor playerColor, bool isKing, FigureId figureType)
    {
        PlayerColor = playerColor;
        IsKing = isKing;
        FigureType = figureType;
    }
}
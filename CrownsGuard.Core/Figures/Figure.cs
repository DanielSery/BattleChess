using CrownsGuard.Core.Players;

namespace CrownsGuard.Core.Figures;

public readonly struct Figure : IEquatable<Figure>
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

    /// <inheritdoc />
    public bool Equals(Figure other)
    {
        return PlayerColor == other.PlayerColor && IsKing == other.IsKing && FigureType == other.FigureType;
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return obj is Figure other && Equals(other);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return HashCode.Combine((int)PlayerColor, IsKing, (int)FigureType);
    }
}
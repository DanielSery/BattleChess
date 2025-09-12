using System.Diagnostics;
using System.Runtime.InteropServices;
using CrownsGuard.Core.Players;

namespace CrownsGuard.Core.Figures;

[DebuggerDisplay("{PlayerColor}-{FigureType}-{IsKing}")]
[StructLayout(LayoutKind.Explicit, Pack = 1)]
public readonly struct Figure
{
    [FieldOffset(0)] public readonly int IntValue;
    [FieldOffset(0)] public readonly PlayerColor PlayerColor;
    [FieldOffset(1)] public readonly bool IsKing;
    [FieldOffset(2)] public readonly FigureId FigureType;

    public Figure(PlayerColor playerColor, bool isKing, FigureId figureType)
    {
        PlayerColor = playerColor;
        IsKing = isKing;
        FigureType = figureType;
    }

    public Figure(int intValue)
    {
        IntValue = intValue;
    }
}
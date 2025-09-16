namespace CrownsGuard.FigureDefinitions.Utilities;

public static class PositionsGroups
{
    public const int YOffset = 256;
    
    public static readonly short[] RookDirections =
    [
        -1+0*YOffset, +1+0*YOffset,
        +0-1*YOffset, +0+1*YOffset
    ];
    
    public static readonly short[] BishopDirections =
    [
        -1-1*YOffset, +1+1*YOffset,
        +1-1*YOffset, -1+1*YOffset
    ];
    
    public static readonly short[] QueenDirections =
    [
        -1-1*YOffset, -1+0*YOffset, -1+1*YOffset,
        +0-1*YOffset, +0+1*YOffset,
        +1-1*YOffset, +1+0*YOffset, +1+1*YOffset
    ];
    
    public static readonly short[] KnightPositions =
    [
        -2-1*YOffset, -2+1*YOffset,
        -1-2*YOffset, -1+2*YOffset,
        +1-2*YOffset, +1+2*YOffset,
        +2-1*YOffset, +2+1*YOffset
    ];
}
namespace CrownsGuard.Engine.Helpers;

public static class PositionsGroups
{
    public const int YOffset = 256;
    
    public static readonly short[] RookDirections =
    [
        unchecked((byte)-1)+0*YOffset, unchecked((byte)+1)+0*YOffset,
        unchecked((byte)+0)-1*YOffset, unchecked((byte)+0)+1*YOffset
    ];
    
    public static readonly short[] BishopDirections =
    [
        unchecked((byte)-1)-1*YOffset, unchecked((byte)+1)+1*YOffset,
        unchecked((byte)+1)-1*YOffset, unchecked((byte)-1)+1*YOffset
    ];
    
    public static readonly short[] QueenDirections =
    [
        unchecked((byte)-1)-1*YOffset, unchecked((byte)-1)+0*YOffset, unchecked((byte)-1)+1*YOffset,
        unchecked((byte)+0)-1*YOffset, unchecked((byte)+0)+1*YOffset,
        unchecked((byte)+1)-1*YOffset, unchecked((byte)+1)+0*YOffset, unchecked((byte)+1)+1*YOffset
    ];
    
    public static readonly short[] KnightPositions =
    [
        unchecked((byte)-2)-1*YOffset, unchecked((byte)-2)+1*YOffset,
        unchecked((byte)-1)-2*YOffset, unchecked((byte)-1)+2*YOffset,
        unchecked((byte)+1)-2*YOffset, unchecked((byte)+1)+2*YOffset,
        unchecked((byte)+2)-1*YOffset, unchecked((byte)+2)+1*YOffset
    ];
}
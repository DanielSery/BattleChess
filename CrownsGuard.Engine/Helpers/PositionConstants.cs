namespace CrownsGuard.Engine.Helpers;

public static class PositionConstants
{
    public const int YOffset = 256;

    public const int R = unchecked((byte)+1) + 0 * YOffset;
    public const int L = unchecked((byte)-1) + 0 * YOffset;
    public const int U = unchecked((byte)+0) + 1 * YOffset;
    public const int D = unchecked((byte)+0) - 1 * YOffset;
    
    public const int UR = unchecked((byte)+1) + 1 * YOffset;
    public const int UL = unchecked((byte)-1) + 1 * YOffset;
    public const int DR = unchecked((byte)+1) - 1 * YOffset;
    public const int DL = unchecked((byte)-1) - 1 * YOffset;
    
    public const int UUR = unchecked((byte)+1) + 2 * YOffset;
    public const int URR = unchecked((byte)+2) + 1 * YOffset;
    public const int UUL = unchecked((byte)-1) + 2 * YOffset;
    public const int ULL = unchecked((byte)-2) + 1 * YOffset;
    public const int DDR = unchecked((byte)+1) - 2 * YOffset;
    public const int DRR = unchecked((byte)+2) - 1 * YOffset;
    public const int DDL = unchecked((byte)-1) - 2 * YOffset;
    public const int DLL = unchecked((byte)-2) - 1 * YOffset;
    
    
    public static readonly short[] RookDirections =
    [
        L, R, D, U
    ];
    
    public static readonly short[] BishopDirections =
    [
        DL, UR, DR, UL
    ];
    
    public static readonly short[] QueenDirections =
    [
        DL, L, UL,
        D, U,
        DR, R, UR
    ];
    
    public static readonly short[] KnightPositions =
    [
        DLL, ULL,
        DDL, UUL,
        DDR, UUR,
        DRR, URR
    ];
}
namespace CrownsGuard.Engine.Helpers;

public static class PositionConstants
{
    public const int YOffset = 256;

    public const int R1 = +1 + 0 * YOffset;
    public const int R2 = +2 + 0 * YOffset;      // 2 steps right
    public const int R3 = +3 + 0 * YOffset;      // 3 steps right
    public const int R4 = +4 + 0 * YOffset;      // 4 steps right
    public const int R5 = +5 + 0 * YOffset;      // 5 steps right
    public const int R6 = +6 + 0 * YOffset;      // 6 steps right
    public const int R7 = +7 + 0 * YOffset;      // 7 steps right

    public const int L1 = unchecked((byte)-1) + 0 * YOffset;
    public const int L2 = unchecked((byte)-2) + 0 * YOffset;      // 2 steps left
    public const int L3 = unchecked((byte)-3) + 0 * YOffset;      // 3 steps left
    public const int L4 = unchecked((byte)-4) + 0 * YOffset;      // 4 steps left
    public const int L5 = unchecked((byte)-5) + 0 * YOffset;      // 5 steps left
    public const int L6 = unchecked((byte)-6) + 0 * YOffset;      // 6 steps left
    public const int L7 = unchecked((byte)-7) + 0 * YOffset;      // 7 steps left

    public const int U1 = +0 + 1 * YOffset;
    public const int U2 = +0 + 2 * YOffset;
    public const int U3 = +0 + 3 * YOffset;
    public const int U4 = +0 + 4 * YOffset;
    public const int U5 = +0 + 5 * YOffset;   // 5 steps up
    public const int U6 = +0 + 6 * YOffset;   // 6 steps up
    public const int U7 = +0 + 7 * YOffset;   // 7 steps up
    
    public const int D1 = +0 - 1 * YOffset;
    public const int D2 = +0 - 2 * YOffset;
    public const int D3 = +0 - 3 * YOffset;
    public const int D4 = +0 - 4 * YOffset;
    public const int D5 = +0 - 5 * YOffset;
    public const int D6 = +0 - 6 * YOffset;
    public const int D7 = +0 - 7 * YOffset;
    
    public const int U1R1 = +1 + 1 * YOffset;
    public const int U2R2 = +2 + 2 * YOffset;   
    public const int U3R3 = +3 + 3 * YOffset;     // 3 steps up, 3 right
    public const int U4R4 = +4 + 4 * YOffset;     // 4 steps up, 4 right
    public const int U5R5 = +5 + 5 * YOffset;     // 5 steps up, 5 right
    public const int U6R6 = +6 + 6 * YOffset;     // 6 steps up, 6 right
    public const int U7R7 = +7 + 7 * YOffset;     // 7 steps up, 7 right
    
    public const int U1L1 = unchecked((byte)-1) + 1 * YOffset;
    public const int U2L2 = unchecked((byte)-2) + 2 * YOffset;  
    public const int U3L3 = unchecked((byte)-3) + 3 * YOffset;     // 3 steps up, 3 left
    public const int U4L4 = unchecked((byte)-4) + 4 * YOffset;     // 4 steps up, 4 left
    public const int U5L5 = unchecked((byte)-5) + 5 * YOffset;     // 5 steps up, 5 left
    public const int U6L6 = unchecked((byte)-6) + 6 * YOffset;     // 6 steps up, 6 left
    public const int U7L7 = unchecked((byte)-7) + 7 * YOffset;     // 7 steps up, 7 left
    
    public const int D1R1 = +1 - 1 * YOffset;
    public const int D2R2 = +2 - 2 * YOffset;     
    public const int D3R3 = +3 - 3 * YOffset;     // 3 steps down, 3 right
    public const int D4R4 = +4 - 4 * YOffset;     // 4 steps down, 4 right
    public const int D5R5 = +5 - 5 * YOffset;     // 5 steps down, 5 right
    public const int D6R6 = +6 - 6 * YOffset;     // 6 steps down, 6 right
    public const int D7R7 = +7 - 7 * YOffset;     // 7 steps down, 7 right
    
    public const int D1L1 = unchecked((byte)-1) - 1 * YOffset;
    public const int D2L2 = unchecked((byte)-2) - 2 * YOffset;
    public const int D3L3 = unchecked((byte)-3) - 3 * YOffset;     // 3 steps down, 3 left
    public const int D4L4 = unchecked((byte)-4) - 4 * YOffset;     // 4 steps down, 4 left
    public const int D5L5 = unchecked((byte)-5) - 5 * YOffset;     // 5 steps down, 5 left
    public const int D6L6 = unchecked((byte)-6) - 6 * YOffset;     // 6 steps down, 6 left
    public const int D7L7 = unchecked((byte)-7) - 7 * YOffset;     // 7 steps down, 7 left
    
    public const int U2R1 = +1 + 2 * YOffset;
    public const int U1R2 = +2 + 1 * YOffset;
    public const int U2L1 = unchecked((byte)-1) + 2 * YOffset;
    public const int U1L2 = unchecked((byte)-2) + 1 * YOffset;
    public const int D2R1 = +1 - 2 * YOffset;
    public const int D1R2 = +2 - 1 * YOffset;
    public const int D2L1 = unchecked((byte)-1) - 2 * YOffset;
    public const int D1L2 = unchecked((byte)-2) - 1 * YOffset;

    public static readonly short[] RookDirections =
    [
        L1, R1, D1, U1
    ];
    
    public static readonly short[] BishopDirections =
    [
        D1L1, U1R1, D1R1, U1L1
    ];
    
    public static readonly short[] QueenDirections =
    [
        D1L1, L1, U1L1,
        D1, U1,
        D1R1, R1, U1R1
    ];
    
    public static readonly short[] KnightPositions =
    [
        D1L2, U1L2,
        D2L1, U2L1,
        D2R1, U2R1,
        D1R2, U1R2
    ];
}
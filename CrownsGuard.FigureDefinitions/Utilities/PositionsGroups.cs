using CrownsGuard.Core.GameBoard;

namespace CrownsGuard.FigureDefinitions.Utilities;

public static class PositionsGroups
{
    public static readonly Position[] RookDirections =
    [
        new(-1, 0), new(1, 0),
        new(0, -1), new(0, 1)
    ];
    
    public static readonly Position[] BishopDirections =
    [
        new(-1, -1), new(1, 1),
        new(1, -1), new(-1, 1)
    ];
    
    public static readonly Position[] QueenDirections =
    [
        new(-1, -1), new(-1, 0), new(-1, 1),
        new(0, -1), new(0, 1),
        new(1, -1), new(1, 0), new(1, 1)
    ];
    
    public static readonly Position[] KnightPositions =
    [
        new(-2, -1), new(-2, 1),
        new(-1, -2), new(-1, 2),
        new(1, -2), new(1, 2),
        new(2, -1), new(2, 1)
    ];
}
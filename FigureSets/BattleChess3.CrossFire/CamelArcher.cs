using BattleChess3.Game.Board;
using BattleChess3.Game.Figures;

namespace BattleChess3.CrossFireFigures;

public class CamelArcher : IFigureTypeWithChainedAttacksAndMoves, ICrossFireFigureType
{
    int IFigureType.FigureId => 18;
    
    Position[] IFigureTypeWithChainedAttacksAndMoves.MoveDirections { get; } =
    [
        new(-1, -1), new(-1, 1),
        new(1, -1), new(1, 1)
    ];

    Position[] IFigureTypeWithChainedAttacksAndMoves.AttackDirections { get; } =
    [
        new(-1, 0),
        new(1, 0),
        new(0, -1),
        new(0, 1)
    ];
}
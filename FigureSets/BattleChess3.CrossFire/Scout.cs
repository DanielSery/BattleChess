using BattleChess3.Game.Board;
using BattleChess3.Game.Figures;

namespace BattleChess3.CrossFireFigures;

public class Scout : IFigureTypeWithChainedAttackMoves, ICrossFireFigureType
{
    int IFigureType.FigureId => 16;
    
    Position[] IFigureTypeWithChainedAttackMoves.AttackMoveDirections { get; } =
    [
        new(0, -1), new(0, 1),
        new(-1, 0), new(1, 0)
    ];
}
using BattleChess3.Game.Board;
using BattleChess3.Game.Figures;

namespace BattleChess3.ChessFigures;

public class Bishop : IChessFigureType, IFigureTypeWithChainedAttackMoves
{
    int IFigureType.FigureId => 3;
    
    Position[] IFigureTypeWithChainedAttackMoves.AttackMoveDirections { get; } =
    [
        new(-1, -1), new(-1, 1),
        new(1, -1), new(1, 1)
    ];
}
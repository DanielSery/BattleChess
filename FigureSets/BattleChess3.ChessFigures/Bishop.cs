using BattleChess3.Game.Board;
using BattleChess3.Game.Figures;

namespace BattleChess3.ChessFigures;

public class Bishop : IChessFigureType, IFigureTypeWithChainedAttackMoves
{
    int IFigureType.FigureId => 3;
    
    Position[] IFigureTypeWithChainedAttackMoves.AttackMoveDirections { get; } =
    [
        (-1, -1), (-1, 1),
        (1, -1), (1, 1)
    ];
}
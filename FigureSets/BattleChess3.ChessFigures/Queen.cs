using BattleChess3.Game.Board;
using BattleChess3.Game.Figures;

namespace BattleChess3.ChessFigures;

public class Queen : IChessFigureType, IFigureTypeWithChainedAttackMoves
{
    int IFigureType.FigureId => 5;
    
    Position[] IFigureTypeWithChainedAttackMoves.AttackMoveDirections { get; } =
    {
        (-1, -1), (-1, 0), (-1, 1),
        (0, -1), (0, 1),
        (1, -1), (1, 0), (1, 1)
    };
}
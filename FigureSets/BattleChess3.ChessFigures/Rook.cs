using BattleChess3.Game.Board;
using BattleChess3.Game.Figures;

namespace BattleChess3.ChessFigures;

public class Rook : IChessFigureType, IFigureTypeWithChainedAttackMoves
{
    int IFigureType.FigureId => 4;
    
    Position[] IFigureTypeWithChainedAttackMoves.AttackMoveDirections { get; } =
    [
        (-1, 0), (1, 0), (0, -1), (0, 1)
    ];
}
using BattleChess3.Game.Board;
using BattleChess3.Game.Figures;

namespace BattleChess3.ChessFigures;

public class Knight : IChessFigureType, IFigureTypeWithDifferentAttackMoves
{
    int IFigureType.FigureId => 2;
    
    Position[] IFigureTypeWithDifferentAttackMoves.AttackMovePositions { get; } =
    [
        (-2, -1), (-2, 1),
        (-1, -2), (-1, 2),
        (1, -2), (1, 2),
        (2, -1), (2, 1)
    ];
}
using BattleChess3.Game.Board;
using BattleChess3.Game.Figures;

namespace BattleChess3.SpyFigures;

public class SpyKnight : ISpyFigureType, ISpyFigureTypeWithDifferentAttackMoves
{
    int IFigureType.FigureId => 3;
    
    Position[] ISpyFigureTypeWithDifferentAttackMoves.AttackMovePositions { get; } =
    [
        new(-2, -1), new(-2, 1),
        new(-1, -2), new(-1, 2),
        new(1, -2), new(1, 2),
        new(2, -1), new(2, 1)
    ];
}
using BattleChess3.Game.Board;
using BattleChess3.Game.Figures;

namespace BattleChess3.SpyFigures;

public class SpyBishop : ISpyFigureType, ISpyFigureTypeWithChainedAttackMoves
{
    int IFigureType.FigureId => 1;
    
    Position[] ISpyFigureTypeWithChainedAttackMoves.AttackMoveDirections { get; } =
    [
        new(-1, -1), new(-1, 1),
        new(1, -1), new(1, 1)
    ];
}
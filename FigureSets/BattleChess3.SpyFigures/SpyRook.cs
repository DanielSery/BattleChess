using BattleChess3.Game.Board;
using BattleChess3.Game.Figures;

namespace BattleChess3.SpyFigures;

public class SpyRook : ISpyFigureType, ISpyFigureTypeWithChainedAttackMoves
{
    int IFigureType.FigureId => 6;
    
    Position[] ISpyFigureTypeWithChainedAttackMoves.AttackMoveDirections { get; } = 
    {
        (-1, 0), (1, 0), (0, -1), (0, 1),
    };
}
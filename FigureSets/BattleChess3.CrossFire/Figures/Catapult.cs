using BattleChess3.CrossFireFigures.Utilities;
using BattleChess3.Game.Figures;
using BattleChess3.Game.GameBoard;

namespace BattleChess3.CrossFireFigures.Figures;

public class Catapult : ICrossFireFigureType
{
    /// <inheritdoc />
    public int FigureValue { get; } = 12;
    
    int IFigureType.FigureId => CrossFireFigureIds.CatapultId;
    
    private static readonly Position[] AttackPositions =
    [
        new (-1, 2), new (1, 2),
        new (-2, 3), new (0, 3), new (2, 3),
    ];

    IEnumerable<FigureAction> IFigureType.GetPossibleActions(ITile unitTile, IBoard board)
    {
        foreach (var neighbourTile in ICrossFireFigureType.NeighbourPositions.GetRelativeTiles(board, unitTile))
        {
            if (unitTile.IsEnemyTo(neighbourTile))
                yield break;
        }
        
        foreach (var targetTile in AttackPositions.GetRelativeTiles(board, unitTile))
        {
            if (!targetTile.IsEmpty())
                yield return unitTile.CreateKillWithoutMove(targetTile, board);
        }
    }
}
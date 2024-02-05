using BattleChess3.Core.Model;
using BattleChess3.Core.Model.Figures;
using BattleChess3.DefaultFigures;
using BattleChess3.DefaultFigures.Utilities;

namespace BattleChess3.CrossFireFigures;

public class Bomber : ICrossFireFigureType
{
    public static readonly Bomber Instance = new();

    public FigureAction GetPossibleAction(ITile unitTile, ITile targetTile, ITile[] board)
    {
        var movement = targetTile.Position - unitTile.Position;
        var targetPosition = (7 - movement.X) + (7 - movement.Y) * 15;;

        if (targetTile.IsEmpty() && (Actions[targetPosition] & 1) == 1)
        {
            return new FigureAction(FigureActionTypes.Move, () =>
            {
                targetTile.Figure = unitTile.Figure;
                unitTile.Figure = new Figure(Player.Neutral, Empty.Instance);
                
                TryDestroyTile(board, targetTile.Position + (1, -1));
                TryDestroyTile(board, targetTile.Position + (1, 0));
                TryDestroyTile(board, targetTile.Position + (1, 1));
                TryDestroyTile(board, targetTile.Position + (0, -1));
                TryDestroyTile(board, targetTile.Position + (0, 1));
                TryDestroyTile(board, targetTile.Position + (-1, -1));
                TryDestroyTile(board, targetTile.Position + (-1, 0));
                TryDestroyTile(board, targetTile.Position + (-1, 1));
            });
        }

        if (targetTile.IsOwnedByEnemy(unitTile) && (Actions[targetPosition] & 2) == 2)
        {
            return new FigureAction(FigureActionTypes.Attack, () =>
            {
                TryDestroyTile(board, targetTile.Position + (1, -1));
                TryDestroyTile(board, targetTile.Position + (1, 0));
                TryDestroyTile(board, targetTile.Position + (1, 1));
                TryDestroyTile(board, targetTile.Position + (0, -1));
                TryDestroyTile(board, targetTile.Position + (0, 1));
                TryDestroyTile(board, targetTile.Position + (-1, -1));
                TryDestroyTile(board, targetTile.Position + (-1, 0));
                TryDestroyTile(board, targetTile.Position + (-1, 1));
            });
        }

        return FigureAction.None;
    }

    private static void TryDestroyTile(ITile[] board, Position targetPosition)
    {
        if (!targetPosition.IsInBoard())
            return;

        board[targetPosition].Die();
    }

    private int[] Actions { get; } = {
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 2, 0, 1, 0, 2, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 1, 0, 8, 0, 1, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 2, 0, 1, 0, 2, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
    };
}

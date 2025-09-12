using System.Runtime.CompilerServices;
using CrownsGuard.Core;
using CrownsGuard.Core.Figures;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.Core.Helpers;
using CrownsGuard.FigureDefinitions.Utilities;

namespace CrownsGuard.FigureDefinitions.Figures;

public class Warhammer : ICrownsGuardFigureTypeInfo
{
    public FigureId FigureId => FigureId.Warhammer;

    public static void GetPossibleActions(Position sourcePosition, Figure sourceFigure, ReadOnlySpan<Figure> board, Stack<FigureAction> actions)
    {
        foreach (var relative in PositionsGroups.BishopDirections)
        {
            var targetPosition = sourcePosition + relative;
            if (!board.TryGetFigure(targetPosition, out var targetFigure))
            {
                continue;
            }
            
            if (targetFigure.IsWalkable())
            {
                actions.Push(new FigureAction(FigureActionType.Move, sourcePosition, targetPosition));
            }
        }
        
        return;
    }

    public static int EvaluateAction(ReadOnlySpan<Figure> board, FigureAction action)
    {
        var movement = action.TargetPosition - action.SourcePosition;
        if (movement is { Y: 1, X: 1 })
        {
            return GetImpact(action.TargetPosition + new Position(1, -1)) +
                   GetImpact(action.TargetPosition + new Position(1, 0)) +
                   GetImpact(action.TargetPosition + new Position(1, 1)) +
                   Constants.MoveImpact;
        }
        else if (movement is { Y: -1, X: 1 })
        {
            return GetImpact(action.TargetPosition + new Position(-1, -1)) +
                   GetImpact(action.TargetPosition + new Position(-1, 0)) +
                   GetImpact(action.TargetPosition + new Position(-1, 1)) +
                   Constants.MoveImpact;
        }
        else if (movement is { Y: 1, X: -1 })
        {
            return GetImpact(action.TargetPosition + new Position(-1, 1)) +
                   GetImpact(action.TargetPosition + new Position(0, 1)) +
                   GetImpact(action.TargetPosition + new Position(1, 1)) +
                   Constants.MoveImpact;
        }
        else if (movement is { Y: -1, X: -1 })
        {
            return GetImpact(action.TargetPosition + new Position(-1, -1)) +
                   GetImpact(action.TargetPosition + new Position(0, -1)) +
                   GetImpact(action.TargetPosition + new Position(1, -1)) +
                   Constants.MoveImpact;
        }
        else
        {
            return 0;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int GetImpact(Position targetPosition)
    {
        return targetPosition.IsInBoard() ? Constants.PossibleHalfRangedAttackImpact : 0;
    }

    public static void ExecuteAction(Span<Figure> board, FigureAction action, Action<BoardEvent, Span<Figure>> onEvent)
    {
        board.MoveFigure(action.SourcePosition, action.TargetPosition, onEvent);
        var movement = action.TargetPosition - action.SourcePosition;
        if (movement is { Y: 0, X: 1 })
        {
            TryDestroyTile(board, action.TargetPosition, new Position(1, -1), onEvent);
            TryDestroyTile(board, action.TargetPosition, new Position(1, 0), onEvent);
            TryDestroyTile(board, action.TargetPosition, new Position(1, 1), onEvent);
        }
        else if (movement is { Y: 0, X: -1 })
        {
            TryDestroyTile(board, action.TargetPosition, new Position(-1, -1), onEvent);
            TryDestroyTile(board, action.TargetPosition, new Position(-1, 0), onEvent);
            TryDestroyTile(board, action.TargetPosition, new Position(-1, 1), onEvent);
        }
        else if (movement is { Y: 1, X: 0 })
        {
            TryDestroyTile(board, action.TargetPosition, new Position(-1, 1), onEvent);
            TryDestroyTile(board, action.TargetPosition, new Position(0, 1), onEvent);
            TryDestroyTile(board, action.TargetPosition, new Position(1, 1), onEvent);
        }
        else if (movement is { Y: -1, X: 0 })
        {
            TryDestroyTile(board, action.TargetPosition, new Position(-1, -1), onEvent);
            TryDestroyTile(board, action.TargetPosition, new Position(0, -1), onEvent);
            TryDestroyTile(board, action.TargetPosition, new Position(1, -1), onEvent);
        }
    }

    private static void TryDestroyTile(Span<Figure> board, Position sourcePosition, Position relative,
        Action<BoardEvent, Span<Figure>> onEvent)
    {
        if (!board.TryGetFigure(sourcePosition + relative, out _))
        {
            return;
        }

        board.KillWithoutMove(sourcePosition, sourcePosition + relative, onEvent);
    }
}
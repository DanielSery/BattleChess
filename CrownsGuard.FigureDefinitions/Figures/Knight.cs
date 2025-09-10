using CrownsGuard.Core.Figures;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.Core.Helpers;
using CrownsGuard.FigureDefinitions.Utilities;

namespace CrownsGuard.FigureDefinitions.Figures;

public class Knight : ICrownsGuardFigureTypeInfo
{
    public int FigureValue => 10;
    public FigureId FigureId => FigureId.Knight;

    public static ArrayPoolMemory<FigureAction> GetPossibleActions(Position sourcePosition, Figure sourceFigure, Span<Figure> board)
    {
        Span<FigureAction> actions = stackalloc FigureAction[36];
        int actionsCount = 0;

        foreach (var relative in PositionsGroups.RookDirections)
        {
            var targetPosition = sourcePosition + relative;
            if (!board.TryGetFigure(targetPosition, out var targetFigure)) continue;

            if (targetFigure.IsWalkable())
                actions[actionsCount++] = new FigureAction(FigureActionType.Move, sourcePosition, targetPosition);
        }

        foreach (var relative in PositionsGroups.RookDirections)
        {
            for (var i = 1; i <= 3; i++)
            {
                var targetPosition = sourcePosition + relative * i;
                if (!board.TryGetFigure(targetPosition, out var targetFigure)) break;

                if (sourceFigure.CanAttack(targetFigure))
                {
                    actions[actionsCount++] = new FigureAction(FigureActionType.Move, sourcePosition, targetPosition);
                }
                else if (targetFigure.IsEmpty())
                {
                    break;
                }
            }
        }

        return actions.ToArrayPoolMemory(actionsCount);
    }

    public static void ExecuteAction(Span<Figure> board, ref FigureAction action, Action<BoardEvent, Span<Figure>> onEvent)
    {
        var move = action.TargetPosition - action.SourcePosition;

        if (move.X is <= 1 and >= -1 &&
            move.Y is <= 1 and >= -1)
        {
            board.KillWithMove(action.SourcePosition, action.TargetPosition, onEvent);
        }
        else if (move.X is <= 2 and >= -2 &&
                 move.Y is <= 2 and >= -2)
        {
            var smallMove = new Position((short)Math.Sign(move.X), (short)Math.Sign(move.Y));
            var sourcePosition = action.SourcePosition;

            board.KillWithMove(sourcePosition, sourcePosition + smallMove, onEvent);
            var figure = board[(sourcePosition + smallMove).GetIndex()];
            if (figure.FigureType != FigureId.Blade)
                return;

            board.KillWithMove(sourcePosition + smallMove, action.TargetPosition, onEvent);
        }
    }
}
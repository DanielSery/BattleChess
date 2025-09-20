using System.Runtime.CompilerServices;
using CrownsGuard.Core.Figures;
using CrownsGuard.Core.Helpers;

namespace CrownsGuard.AI.Helpers;

public class ActionImpactEvaluator
{
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public static int EvaluateAction(ReadOnlySpan<Figure> board, FigureAction action, Figure currentFigureColor)
    {
        var actionType = action.FigureActionType & FigureActionType.EvaluationMask;
        switch (actionType)
        {
            case FigureActionType.IsTargeted | FigureActionType.IsMovingAttack:
            {
                var sourceFigureValue = action.SourceFigure.GetFigureValue();
                var targetFigure = board[action.TargetIndex];
                var targetFigureValue = targetFigure.GetFigureValue();

                if (action.SourceFigure.IsSameColor(targetFigure))
                {
                    return (int)(action.FigureActionType & FigureActionType.ActionValueMask) * sourceFigureValue * sourceFigureValue / ((targetFigureValue + sourceFigureValue) * 100);
                }

                var attackerAdvantage = currentFigureColor == action.SourceFigure.GetFigureColor() ? 100 : 0;
                return (int)(action.FigureActionType & FigureActionType.ActionValueMask) * 10 * targetFigureValue / (targetFigureValue + sourceFigureValue) + attackerAdvantage;
            }
            case FigureActionType.IsTargeted:
            {
                var attackerAdvantage = currentFigureColor == action.SourceFigure.GetFigureColor() ? 100 : 0;
                var targetFigure = board[action.TargetIndex];
                var targetFigureValue = targetFigure.GetFigureValue();
                return (int)(action.FigureActionType & FigureActionType.ActionValueMask) * targetFigureValue + attackerAdvantage;
            }
            case FigureActionType.IsMovingAttack:
            {
                var sourceFigureValue = action.SourceFigure.GetFigureValue();
                return (int)(action.FigureActionType & FigureActionType.ActionValueMask) * 25 / sourceFigureValue;
            }
            default:
            {
                return (int)(action.FigureActionType & FigureActionType.ActionValueMask);
            }
        }
    }
}
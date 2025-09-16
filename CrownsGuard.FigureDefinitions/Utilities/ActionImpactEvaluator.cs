// Copyright (c) Veeam Software Group GmbH

using System.Runtime.CompilerServices;
using CrownsGuard.Core;
using CrownsGuard.Core.Figures;
using CrownsGuard.Core.Helpers;

namespace CrownsGuard.FigureDefinitions.Utilities;

public class ActionImpactEvaluator
{
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public static int EvaluateAction(ReadOnlySpan<Figure> board, FigureAction action, Figure currentFigureColor)
    {
        if (action.FigureActionType == FigureActionType.Move)
        {
            return Constants.MoveValue;
        }
        
        var actionType = action.FigureActionType & FigureActionType.EvaluationMask;
        switch (actionType)
        {
            case FigureActionType.IsTargeted | FigureActionType.IsMovingAttack:
            {
                var sourceFigureValue = (int)action.SourceFigure;
                var targetFigure = board[action.TargetIndex];
                var targetFigureValue = (int)targetFigure;

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
                var targetFigureValue = (int)board[action.TargetIndex] / 100;
                return (int)(action.FigureActionType & FigureActionType.ActionValueMask) * targetFigureValue + attackerAdvantage;
            }
            default:
            {
                return (int)(action.FigureActionType & FigureActionType.ActionValueMask);
            }
        }
    }
}
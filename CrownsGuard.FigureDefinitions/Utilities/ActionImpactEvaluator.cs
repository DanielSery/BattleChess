// Copyright (c) Veeam Software Group GmbH

using System.Runtime.CompilerServices;
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
            return (int)(action.FigureActionType & FigureActionType.ActionValueMask);
        }
        
        var actionType = action.FigureActionType & FigureActionType.ActionTypeMask;
        switch (actionType)
        {
            case FigureActionType.IsTargetDependantMovingAttack:
            {
                var sourceFigureValue = (int)action.SourceFigure;
                var targetFigureValue = (int)action.TargetFigure;

                if (action.SourceFigure.IsSameColor(action.TargetFigure))
                {
                    return (int)(action.FigureActionType & FigureActionType.ActionValueMask) * sourceFigureValue * sourceFigureValue / ((targetFigureValue + sourceFigureValue) * 100);
                }

                var attackerAdvantage = currentFigureColor == action.SourceFigure.GetFigureColor() ? 100 : 0;
                return (int)(action.FigureActionType & FigureActionType.ActionValueMask) * 10 * targetFigureValue / (targetFigureValue + sourceFigureValue) + attackerAdvantage;
            }
            case FigureActionType.IsBlindMovingAttack:
            {
                var attackerAdvantage = currentFigureColor == action.SourceFigure.GetFigureColor() ? 100 : 0;
                return (int)(action.FigureActionType & FigureActionType.ActionValueMask) + attackerAdvantage;
            }
            case 
            default:
            {
                if (action.FigureActionType.HasFlag(FigureActionType.IsTargetDependant))
                {
                    var attackerAdvantage = currentFigureColor == action.SourceFigure.GetFigureColor() ? 100 : 0;
                    var targetFigureValue = (int)board[action.TargetPosition.GetIndex()] / 100;
                    return (int)(action.FigureActionType & FigureActionType.ActionValueMask) * targetFigureValue + attackerAdvantage;
                }
                else
                {
                    return (int)(action.FigureActionType & FigureActionType.ActionValueMask);
                }
            }
        }
    }
}
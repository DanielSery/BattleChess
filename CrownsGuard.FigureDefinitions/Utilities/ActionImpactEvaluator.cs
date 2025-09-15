// Copyright (c) Veeam Software Group GmbH

using System.Runtime.CompilerServices;
using CrownsGuard.Core.Figures;
using CrownsGuard.Core.Helpers;
using CrownsGuard.Core.Players;

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
        if (actionType == FigureActionType.IsTargetDependantMovingAttack)
        {
            var sourceFigure = board[action.SourcePosition.GetIndex()];
            var targetFigure = board[action.TargetPosition.GetIndex()];
            var sourceFigureValue = sourceFigure.GetFigureValue();
            var targetFigureValue = targetFigure.GetFigureValue();

            if (sourceFigure.GetFigureColor() == targetFigure.GetFigureColor())
            {
                return (int)(action.FigureActionType & FigureActionType.ActionValueMask) * sourceFigureValue * sourceFigureValue / (targetFigureValue + sourceFigureValue);
            }

            var attackerAdvantage = currentFigureColor == sourceFigure.GetFigureColor() ? 100 : 0;
            return (int)(action.FigureActionType & FigureActionType.ActionValueMask) * 10 * targetFigureValue / (targetFigureValue + sourceFigureValue) + attackerAdvantage;
        }
        else if (actionType == FigureActionType.IsBlindMovingAttack)
        {
            var sourceFigure = board[action.SourcePosition.GetIndex()];
            var attackerAdvantage = currentFigureColor == sourceFigure.GetFigureColor() ? 100 : 0;
            return (int)(action.FigureActionType & FigureActionType.ActionValueMask) + attackerAdvantage;
        }
        else if (action.FigureActionType.HasFlag(FigureActionType.IsTargetDependant))
        {
            var sourceFigure = board[action.SourcePosition.GetIndex()];
            var attackerAdvantage = currentFigureColor == sourceFigure.GetFigureColor() ? 100 : 0;
            var targetFigureValue = board[action.TargetPosition.GetIndex()].GetFigureValue();
            return (int)(action.FigureActionType & FigureActionType.ActionValueMask) * targetFigureValue + attackerAdvantage;
        }
        else
        {
            return (int)(action.FigureActionType & FigureActionType.ActionValueMask);
        }
    }
}
// Copyright (c) Veeam Software Group GmbH

using System.Runtime.CompilerServices;
using CrownsGuard.Core.Figures;

namespace CrownsGuard.FigureDefinitions.Utilities;

public class ActionImpactEvaluator
{
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public static int EvaluateAction(ReadOnlySpan<Figure> board, FigureAction action)
    {
        if (action.FigureActionType.HasFlag(FigureActionType.IsTargetDependantMovingAttack))
        {
            var sourceFigureValue = board[action.SourcePosition.GetIndex()].FigureType.GetFigureValue();
            var targetFigureValue = board[action.TargetPosition.GetIndex()].FigureType.GetFigureValue();
            return (int)(action.FigureActionType & FigureActionType.ActionValueMask) * targetFigureValue * targetFigureValue / (targetFigureValue + sourceFigureValue);
        }
        else if (action.FigureActionType.HasFlag(FigureActionType.IsMovingAttack))
        {
            var sourceFigureValue = board[action.SourcePosition.GetIndex()].FigureType.GetFigureValue();
            return (int)(action.FigureActionType & FigureActionType.ActionValueMask) * 5 / (5 + sourceFigureValue);
        }
        else if (action.FigureActionType.HasFlag(FigureActionType.IsTargetDependant))
        {
            var targetFigureValue = board[action.TargetPosition.GetIndex()].FigureType.GetFigureValue();
            return (int)(action.FigureActionType & FigureActionType.ActionValueMask) * targetFigureValue;
        }
        else
        {
            return (int)(action.FigureActionType & FigureActionType.ActionValueMask);
        }
    }
}
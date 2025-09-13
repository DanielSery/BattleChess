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
            var sourceFigure = board[action.SourcePosition.GetIndex()];
            var targetFigure = board[action.TargetPosition.GetIndex()];
            var sourceFigureValue = sourceFigure.FigureType.GetFigureValue();
            var targetFigureValue = targetFigure.FigureType.GetFigureValue();

            if (sourceFigure.PlayerColor == targetFigure.PlayerColor)
            {
                return (int)(action.FigureActionType & FigureActionType.ActionValueMask) * sourceFigureValue * sourceFigureValue / (targetFigureValue + sourceFigureValue);
            }

            return (int)(action.FigureActionType & FigureActionType.ActionValueMask) * targetFigureValue * targetFigureValue / (targetFigureValue + sourceFigureValue);
        }
        else if (action.FigureActionType.HasFlag(FigureActionType.IsMovingAttack))
        {
            return (int)(action.FigureActionType & FigureActionType.ActionValueMask);
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
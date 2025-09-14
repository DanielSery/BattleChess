// Copyright (c) Veeam Software Group GmbH

using System.Runtime.CompilerServices;
using CrownsGuard.Core.Figures;
using CrownsGuard.Core.Players;

namespace CrownsGuard.FigureDefinitions.Utilities;

public class ActionImpactEvaluator
{
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public static int EvaluateAction(ReadOnlySpan<Figure> board, FigureAction action, PlayerColor currentPlayerColor)
    {
        var sourceFigure = board[action.SourcePosition.GetIndex()];
        var attackerAdvantage = currentPlayerColor == sourceFigure.PlayerColor ? 100 : 0;

        if (action.FigureActionType.HasFlag(FigureActionType.IsTargetDependantMovingAttack))
        {
            var targetFigure = board[action.TargetPosition.GetIndex()];
            var sourceFigureValue = sourceFigure.FigureType.GetFigureValue();
            var targetFigureValue = targetFigure.FigureType.GetFigureValue();

            if (sourceFigure.PlayerColor == targetFigure.PlayerColor)
            {
                return (int)(action.FigureActionType & FigureActionType.ActionValueMask) * sourceFigureValue * sourceFigureValue / (targetFigureValue + sourceFigureValue);
            }

            return (int)(action.FigureActionType & FigureActionType.ActionValueMask) * 10 * targetFigureValue / (targetFigureValue + sourceFigureValue) + attackerAdvantage;
        }
        else if (action.FigureActionType.HasFlag(FigureActionType.IsMovingAttack))
        {
            return (int)(action.FigureActionType & FigureActionType.ActionValueMask) + attackerAdvantage;
        }
        else if (action.FigureActionType.HasFlag(FigureActionType.IsTargetDependant))
        {
            var targetFigureValue = board[action.TargetPosition.GetIndex()].FigureType.GetFigureValue();
            return (int)(action.FigureActionType & FigureActionType.ActionValueMask) * targetFigureValue + attackerAdvantage;
        }
        else
        {
            return (int)(action.FigureActionType & FigureActionType.ActionValueMask);
        }
    }
}
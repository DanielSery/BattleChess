using CrownsGuard.Core;
using CrownsGuard.Core.Figures;
using CrownsGuard.Core.Helpers;
using CrownsGuard.Engine.Helpers;

namespace CrownsGuard.AI.Helpers
{
    public static class BoardEvaluationHelper
    {
        public static int EvaluateBoard(ReadOnlySpan<Figure> board, Stack<FigureAction> actionsStack, Figure currentFigureColor)
        {
            var evaluation = 0;
            for (var i = 0; i < board.Length; i++)
            {
                var figure = board[i];
                if (figure.IsNeutralFigure())
                {
                    continue;
                }

                var oldCount = actionsStack.Count;
                FigureActionsResolver.GetPossibleActions(i, figure, board, actionsStack);
                var added = actionsStack.Count - oldCount;
                if (figure.IsWhite())
                {
                    for (var j = 0; j < added; j++)
                    {
                        var action = actionsStack.Pop();
                        if (action.FigureActionType == FigureActionType.Move)
                            evaluation -= Constants.MoveValue;
                        else evaluation -= ActionImpactEvaluator.EvaluateAction(board, action, currentFigureColor);
                    }
                }
                else
                {
                    for (var j = 0; j < added; j++)
                    {
                        var action = actionsStack.Pop();
                        if (action.FigureActionType == FigureActionType.Move)
                            evaluation += Constants.MoveValue;
                        else evaluation += ActionImpactEvaluator.EvaluateAction(board, action, currentFigureColor);
                    }
                }
            }
            return evaluation;
        }
    }
}

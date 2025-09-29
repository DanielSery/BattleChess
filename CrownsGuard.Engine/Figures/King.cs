using CrownsGuard.Core;
using CrownsGuard.Core.Board;
using CrownsGuard.Core.Figures;
using CrownsGuard.Core.Helpers;
using CrownsGuard.Engine.Helpers;

namespace CrownsGuard.Engine.Figures;

public static class King
{
    public static void GetPossibleActions(int sourceIndex, Figure sourceFigure, ReadOnlySpan<Figure> board, Stack<FigureAction> actions)
    {
        foreach (var relative in PositionConstants.QueenDirections)
        {
            var targetIndex = sourceIndex.GetWithOffset(relative);
            if (targetIndex == -1) continue;
            var targetFigure = board[targetIndex];

            if (targetFigure.IsWalkable())
            {
                actions.Push(new FigureAction(FigureActionType.Move, sourceIndex, targetIndex, sourceFigure));
            }

            if (sourceFigure.CanAttack(targetFigure))
            {
                actions.Push(new FigureAction(FigureActionType.MeleeAttack, sourceIndex, targetIndex, sourceFigure));
            }
            else
            {
                actions.Push(new FigureAction(FigureActionType.MeleeDefend, sourceIndex, targetIndex, sourceFigure));
            }
        }

        var absoluteX = PositionsHelper.GetAbsoluteX(sourceIndex);
        var absoluteY = PositionsHelper.GetAbsoluteY(sourceIndex);
        if (absoluteX != 4)
        {
            return;
        }

        if (sourceFigure.IsBlack())
        {
            if (absoluteY != 0)
            {
                return;
            }

            if (sourceFigure.IsAllyTo(board[0]))
            {
                if (board[1].IsEmpty() &&
                    board[2].IsEmpty() &&
                    board[3].IsEmpty())
                {
                    actions.Push(new FigureAction(FigureActionType.Castling, sourceIndex, +2+0*Constants.BoardLength, sourceFigure));
                }
                else
                {
                    actions.Push(new FigureAction(FigureActionType.PossibleCastling, sourceIndex, +2+0*Constants.BoardLength, sourceFigure));
                }
            }

            if (sourceFigure.IsAllyTo(board[7]))
            {
                if (board[5].IsEmpty() &&
                    board[6].IsEmpty())
                {
                    actions.Push(new FigureAction(FigureActionType.Castling, sourceIndex, +6+0*Constants.BoardLength, sourceFigure));
                }
                else
                {
                    actions.Push(new FigureAction(FigureActionType.PossibleCastling, sourceIndex, +6+0*Constants.BoardLength, sourceFigure));
                }
            }
        }
        else
        {
            if (absoluteY != 7)
            {
                return;
            }
            
            if (sourceFigure.IsAllyTo(board[+0+7*Constants.BoardLength]))
            {
                if (board[+1 + 7 * Constants.BoardLength].IsEmpty() &&
                    board[+2 + 7 * Constants.BoardLength].IsEmpty() &&
                    board[+3 + 7 * Constants.BoardLength].IsEmpty())
                {
                    actions.Push(new FigureAction(FigureActionType.Castling, sourceIndex, +2+7*Constants.BoardLength, sourceFigure));
                }
                else
                {
                    actions.Push(new FigureAction(FigureActionType.PossibleCastling, sourceIndex, +2+7*Constants.BoardLength, sourceFigure));
                }
            }

            if (sourceFigure.IsAllyTo(board[+7+7*Constants.BoardLength]))
            {
                if (board[+5+7*Constants.BoardLength].IsEmpty() &&
                    board[+6+7*Constants.BoardLength].IsEmpty())
                {
                    actions.Push(new FigureAction(FigureActionType.Castling, sourceIndex, +6+7*Constants.BoardLength, sourceFigure));
                }
                else
                {
                    actions.Push(new FigureAction(FigureActionType.PossibleCastling, sourceIndex, +6+7*Constants.BoardLength, sourceFigure));
                }
            }
        }
    }

    public static void ExecuteCastle(Span<Figure> board, FigureAction action, Action<BoardEvent, Span<Figure>> onEvent)
    {
        if (action.TargetIndex == +2+0*Constants.BoardLength)
        {
            board.MoveFigure(+4+0*Constants.BoardLength, +2+0*Constants.BoardLength, onEvent);
            board.MoveFigure(+0+0*Constants.BoardLength, +3+0*Constants.BoardLength, onEvent);
        }
        else if (action.TargetIndex == +2+7*Constants.BoardLength)
        {
            board.MoveFigure(+4+7*Constants.BoardLength, +2+7*Constants.BoardLength, onEvent);
            board.MoveFigure(+0+7*Constants.BoardLength, +3+7*Constants.BoardLength, onEvent);
        }
        else if (action.TargetIndex == +6+0*Constants.BoardLength)
        {
            board.MoveFigure(+4+0*Constants.BoardLength, +6+0*Constants.BoardLength, onEvent);
            board.MoveFigure(+7+0*Constants.BoardLength, +5+0*Constants.BoardLength, onEvent);
        }
        else if (action.TargetIndex == +6+7*Constants.BoardLength)
        {
            board.MoveFigure(+4+7*Constants.BoardLength, +6+7*Constants.BoardLength, onEvent);
            board.MoveFigure(+7+7*Constants.BoardLength, +5+7*Constants.BoardLength, onEvent);
        }
    }
}
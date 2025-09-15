using CrownsGuard.Core.Figures;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.Core.Helpers;
using CrownsGuard.FigureDefinitions.Utilities;

namespace CrownsGuard.FigureDefinitions.Figures;

public class King : ICrownsGuardFigureTypeInfo
{
    public Figure Figure => Figure.King;

    public static void GetPossibleActions(Position sourcePosition, Figure sourceFigure, ReadOnlySpan<Figure> board, Stack<FigureAction> actions)
    {
        foreach (var relative in PositionsGroups.QueenDirections)
        {
            var targetPosition = sourcePosition + relative;
            if (!board.TryGetFigure(targetPosition, out var targetFigure))
            {
                continue;
            }

            if (targetFigure.IsWalkable())
            {
                actions.Push(new FigureAction(FigureActionType.Move, sourcePosition, targetPosition, sourceFigure, targetFigure));
            }

            if (sourceFigure.CanAttack(targetFigure))
            {
                actions.Push(new FigureAction(FigureActionType.MeeleeAttack, sourcePosition, targetPosition, sourceFigure, targetFigure));
            }
            else
            {
                actions.Push(new FigureAction(FigureActionType.PossibleMeeleeAttack, sourcePosition, targetPosition, sourceFigure, targetFigure));
            }
        }

        if (sourcePosition.X != 4)
        {
            return;
        }

        if (sourceFigure.IsBlack())
        {
            if (sourcePosition.Y != 0)
            {
                return;
            }

            if (sourceFigure.IsAllyTo(board[0]))
            {
                if (board[1].IsEmpty() &&
                    board[2].IsEmpty() &&
                    board[3].IsEmpty())
                {
                    actions.Push(new FigureAction(FigureActionType.Castling, sourcePosition, new Position(2, 0), sourceFigure, Figure.Empty));
                }
                else
                {
                    actions.Push(new FigureAction(FigureActionType.PossibleCastling, sourcePosition, new Position(2, 0), sourceFigure, Figure.Empty));
                }
            }

            if (sourceFigure.IsAllyTo(board[7]))
            {
                if (board[5].IsEmpty() &&
                    board[6].IsEmpty())
                {
                    actions.Push(new FigureAction(FigureActionType.Castling, sourcePosition, new Position(6, 0), sourceFigure, Figure.Empty));
                }
                else
                {
                    actions.Push(new FigureAction(FigureActionType.PossibleCastling, sourcePosition, new Position(6, 0), sourceFigure, Figure.Empty));
                }
            }
        }

        if (sourceFigure.IsWhite())
        {
            if (sourcePosition.Y != 7)
            {
                return;
            }

            if (sourceFigure.IsAllyTo(board[new Position(0, 7).GetIndex()]) &&
                board[new Position(1, 7).GetIndex()].IsEmpty() &&
                board[new Position(2, 7).GetIndex()].IsEmpty() &&
                board[new Position(3, 7).GetIndex()].IsEmpty())
            {
                actions.Push(new FigureAction(FigureActionType.Castling, sourcePosition, new Position(2, 7), sourceFigure, Figure.Empty));
            }

            if (sourceFigure.IsAllyTo(board[new Position(7, 7).GetIndex()]) &&
                board[new Position(5, 7).GetIndex()].IsEmpty() &&
                board[new Position(6, 7).GetIndex()].IsEmpty())
            {
                actions.Push(new FigureAction(FigureActionType.Castling, sourcePosition, new Position(6, 7), sourceFigure, Figure.Empty));
            }
        }
    }

    public static void ExecuteCastle(Span<Figure> board, FigureAction action, Action<BoardEvent, Span<Figure>> onEvent)
    {
        if (action is { TargetPosition.X: 2 })
        {
            board.MoveFigure(new Position(4, action.TargetPosition.Y), new Position(2, action.TargetPosition.Y), onEvent);
            board.MoveFigure(new Position(0, action.TargetPosition.Y), new Position(3, action.TargetPosition.Y), onEvent);
        }
        else if (action is { TargetPosition.X: 6 })
        {
            board.MoveFigure(new Position(4, action.TargetPosition.Y), new Position(6, action.TargetPosition.Y), onEvent);
            board.MoveFigure(new Position(7, action.TargetPosition.Y), new Position(5, action.TargetPosition.Y), onEvent);
        }
    }
}
using CrownsGuard.Core.Figures;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.FigureDefinitions.Utilities;

namespace CrownsGuard.FigureDefinitions.Figures;

public class Barbarian : ICrownsGuardFigureTypeInfo
{
    public Figure Figure => Figure.Barbarian;

    public static void GetPossibleActions(int sourceIndex, Figure sourceFigure, ReadOnlySpan<Figure> board, Stack<FigureAction> actions)
    {
        foreach (var relative in PositionsGroups.KnightPositions)
        {
            var targetIndex = sourceIndex.GetWithOffset(relative);
            if (targetIndex == -1) continue;
            var targetFigure = board[targetIndex];

            if (targetFigure.IsWalkable())
            {
                actions.Push(new FigureAction(FigureActionType.Move, sourceIndex, targetIndex, sourceFigure));
            }
        }
        
        foreach (var relative in PositionsGroups.QueenDirections)
        {
            var movedIndex = sourceIndex.GetWithOffset(relative);
            if (movedIndex == -1) continue;
            var movedFigure = board[movedIndex];

            if (movedFigure.IsEmpty())
            {
                continue;
            }

            for (var targetIndex = movedIndex.GetWithOffset(relative); targetIndex != -1; targetIndex = targetIndex.GetWithOffset(relative))
            {
                var targetFigure = board[targetIndex];
                if (targetFigure.IsEmpty())
                {
                    actions.Push(new FigureAction(FigureActionType.PushFigure, movedIndex, targetIndex, sourceFigure));
                }
                else
                {
                    actions.Push(new FigureAction(FigureActionType.PossiblePushFigure, movedIndex, targetIndex, sourceFigure));
                }
            }
        }
    }
}
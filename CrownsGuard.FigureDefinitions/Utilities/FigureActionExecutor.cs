using System.Runtime.CompilerServices;
using CrownsGuard.Core.Figures;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.Core.Helpers;
using CrownsGuard.FigureDefinitions.Figures;

namespace CrownsGuard.FigureDefinitions.Utilities;

public static class FigureActionExecutor
{
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public static void ExecuteFigureAction(Span<Figure> board, FigureAction action, Action<BoardEvent, Span<Figure>> onEvent)
    {
        switch (action.FigureActionType)
        {
            case FigureActionType.PushFigure:
            case FigureActionType.Move:
                board.MoveFigure(action.SourceIndex, action.TargetIndex, onEvent);
                break;
            case FigureActionType.MeeleeAttack:
                board.KillWithMove(action.SourceIndex, action.TargetIndex, onEvent);
                break;
            case FigureActionType.RangedAttack:
                board.KillWithoutMove(action.SourceIndex, action.TargetIndex, onEvent);
                break;
            case FigureActionType.AlchemistMove:
                Alchemist.ExecuteMove(board, action, onEvent);
                break;
            case FigureActionType.BuildWall:
                board.CreateFigure(action.TargetIndex, Figure.Wall, onEvent);
                break;
            case FigureActionType.BattleAxeMove:
                BattleAxe.ExecuteMove(board, action, onEvent);
                break;
            case FigureActionType.BreatheFire:
                Dragon.ExecuteBreatheFire(board, action, onEvent);
                break;
            case FigureActionType.CannonAttack:
                Cannon.ExecuteAttack(board, action, onEvent);
                break;
            case FigureActionType.Castling:
                King.ExecuteCastle(board, action, onEvent);
                break;
            case FigureActionType.ChangeToQueen:
                ChangeToQueen(board, action, onEvent);
                break;
            case FigureActionType.ConvertUnit:
                board.ConvertUnit(action.SourceIndex, action.TargetIndex, onEvent);
                break;
            case FigureActionType.MageMove:
                Mage.ExecuteMove(board, action, onEvent);
                break;
            case FigureActionType.MinerMove:
                Miner.ExecuteMove(board, action, onEvent);
                break;
            case FigureActionType.MakeUnitKing:
                Priest.ExecuteMakeKing(board, action, onEvent);
                break;
            case FigureActionType.MeeleePierceAttack:
                MeeleePierceAttack(board, action, onEvent);
                break;
            case FigureActionType.SpartanMove:
                Spartan.ExecuteMove(board, action, onEvent);
                break;
            case FigureActionType.SwapWithFigure:
                board.SwapTiles(action.SourceIndex, action.TargetIndex, onEvent);
                break;
            case FigureActionType.WarhammerMove:
                Warhammer.ExecuteMove(board, action, onEvent);
                break;
            case FigureActionType.WizzardMove:
                Wizzard.ExecuteMove(board, action, onEvent);
                break;
            case FigureActionType.PossibleCannonAttack:
            case FigureActionType.PossibleConvertUnit:
            case FigureActionType.PossibleMeeleeAttack:
            case FigureActionType.PossibleMeeleePierceAttack:
            case FigureActionType.PossiblePushFigure:
            case FigureActionType.PossibleRangedAttack:
            case FigureActionType.IsExecutable:
            case FigureActionType.IsTargeted:
                throw new InvalidOperationException();
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private static void ChangeToQueen(Span<Figure> board, FigureAction action, Action<BoardEvent, Span<Figure>> onEvent)
    {
        var sourceFigureType = action.SourceFigure.GetFigureType();
        var targetIndex = action.TargetIndex;
        var targetFigure = board[targetIndex];

        if (targetFigure.IsWalkable())
        {
            board.MoveFigure(action.SourceIndex, action.TargetIndex, onEvent);
            if (board[targetIndex].GetFigureType() == sourceFigureType)
                board.ChangeFigureType(action.TargetIndex, action.TargetIndex, Figure.Queen, onEvent);
        }
        else
        {
            board.KillWithMove(action.SourceIndex, action.TargetIndex, onEvent);
            if (board[targetIndex].GetFigureType() == sourceFigureType)
                board.ChangeFigureType(action.TargetIndex, action.TargetIndex, Figure.Queen, onEvent);
        }
    }

    private static void MeeleePierceAttack(Span<Figure> board, FigureAction action, Action<BoardEvent, Span<Figure>> onEvent)
    {
        var sourceFigureType = action.SourceFigure.GetFigureType();
        
        var move = PositionsHelper.GetRelative(action.SourceIndex, action.TargetIndex);
        var moveX = PositionsHelper.GetRelativeX(move);
        var moveY = PositionsHelper.GetRelativeY(move);
        if (moveX is <= 1 and >= -1 &&
            moveY is <= 1 and >= -1)
        {
            board.KillWithMove(action.SourceIndex, action.TargetIndex, onEvent);
        }
        else if (moveX is <= 2 and >= -2 &&
                 moveY is <= 2 and >= -2)
        {
            var smallMove = PositionsHelper.GetRelativePosition(Math.Sign(moveX), Math.Sign(moveY));
            var midIndex = (byte)action.SourceIndex.GetWithOffset(smallMove);

            board.KillWithMove(action.SourceIndex, midIndex, onEvent);
            var figure = board[midIndex];
            if (figure.GetFigureType() != sourceFigureType)
                return;

            board.KillWithMove(midIndex, action.TargetIndex, onEvent);
        }
        else
        {
            var smallMove = PositionsHelper.GetRelativePosition(Math.Sign(moveX), Math.Sign(moveY));
            var step1Index = (byte)action.SourceIndex.GetWithOffset(smallMove);
            
            if (board[step1Index].IsWalkable())
                board.MoveFigure(action.SourceIndex, step1Index, onEvent);
            else board.KillWithMove(action.SourceIndex, step1Index, onEvent);

            if (board[step1Index].GetFigureType() != sourceFigureType)
                return;

            var step2Index = (byte)step1Index.GetWithOffset(smallMove);
            if (board[step2Index].IsWalkable())
                board.MoveFigure(step1Index, step2Index, onEvent);
            else board.KillWithMove(step1Index, step2Index, onEvent);

            if (board[step2Index].GetFigureType() != sourceFigureType)
                return;

            if (board[action.TargetIndex].IsWalkable())
                board.MoveFigure(step2Index, action.TargetIndex, onEvent);
            else board.KillWithMove(step2Index, action.TargetIndex, onEvent);
        }
    }
}
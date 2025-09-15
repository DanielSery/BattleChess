using System.Runtime.CompilerServices;
using CrownsGuard.Core.Figures;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.Core.Helpers;
using CrownsGuard.Core.Players;
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
                board.MoveFigure(action.SourcePosition, action.TargetPosition, onEvent);
                break;
            case FigureActionType.MeeleeAttack:
                board.KillWithMove(action.SourcePosition, action.TargetPosition, onEvent);
                break;
            case FigureActionType.RangedAttack:
                board.KillWithoutMove(action.SourcePosition, action.TargetPosition, onEvent);
                break;
            case FigureActionType.AlchemistMove:
                Alchemist.ExecuteMove(board, action, onEvent);
                break;
            case FigureActionType.BuildWall:
                board.CreateFigure(action.TargetPosition, Figure.Wall, onEvent);
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
                board.ConvertUnit(action.SourcePosition, action.TargetPosition, onEvent);
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
                board.SwapTiles(action.SourcePosition, action.TargetPosition, onEvent);
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
            case FigureActionType.IsTargetDependant:
                throw new InvalidOperationException();
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private static void ChangeToQueen(Span<Figure> board, FigureAction action, Action<BoardEvent, Span<Figure>> onEvent)
    {
        var sourceFigureType = board[action.SourcePosition.GetIndex()].GetFigureType();
        var targetIndex = action.TargetPosition.GetIndex();
        var targetFigure = board[targetIndex];

        if (targetFigure.IsWalkable())
        {
            board.MoveFigure(action.SourcePosition, action.TargetPosition, onEvent);
            if (board[targetIndex].GetFigureType() == sourceFigureType)
                board.ChangeFigureType(action.TargetPosition, action.TargetPosition, Figure.Queen, onEvent);
        }
        else
        {
            board.KillWithMove(action.SourcePosition, action.TargetPosition, onEvent);
            if (board[targetIndex]. GetFigureType() == sourceFigureType)
                board.ChangeFigureType(action.TargetPosition, action.TargetPosition, Figure.Queen, onEvent);
        }
    }

    private static void MeeleePierceAttack(Span<Figure> board, FigureAction action, Action<BoardEvent, Span<Figure>> onEvent)
    {
        var move = action.TargetPosition - action.SourcePosition;
        if (move.X is <= 1 and >= -1 &&
            move.Y is <= 1 and >= -1)
        {
            board.KillWithMove(action.SourcePosition, action.TargetPosition, onEvent);
        }
        else if (move.X is <= 2 and >= -2 &&
                 move.Y is <= 2 and >= -2)
        {
            var smallMove = new Position((sbyte)Math.Sign(move.X), (sbyte)Math.Sign(move.Y));
            var sourcePosition = action.SourcePosition;

            board.KillWithMove(sourcePosition, sourcePosition + smallMove, onEvent);
            var figure = board[(sourcePosition + smallMove).GetIndex()];
            if (figure.GetFigureType() != Figure.Blade)
                return;

            board.KillWithMove(sourcePosition + smallMove, action.TargetPosition, onEvent);
        }
        else
        {
            var smallMove = new Position((sbyte)Math.Sign(move.X), (sbyte)Math.Sign(move.Y));

            var step1Position = action.SourcePosition + smallMove;
            if (board[step1Position.GetIndex()].IsWalkable())
                board.MoveFigure(action.SourcePosition, step1Position, onEvent);
            else board.KillWithMove(action.SourcePosition, step1Position, onEvent);

            if (board[step1Position.GetIndex()].GetFigureType() != Figure.Elephant)
                return;

            var step2Position = action.SourcePosition + smallMove + smallMove;
            if (board[step2Position.GetIndex()].IsWalkable())
                board.MoveFigure(step1Position, step2Position, onEvent);
            else board.KillWithMove(step1Position, step2Position, onEvent);

            if (board[step2Position.GetIndex()].GetFigureType() != Figure.Elephant)
                return;

            if (board[action.TargetPosition.GetIndex()].IsWalkable())
                board.MoveFigure(step2Position, action.TargetPosition, onEvent);
            else board.KillWithMove(step2Position, action.TargetPosition, onEvent);
        }
    }
}
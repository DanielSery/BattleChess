using System.Runtime.CompilerServices;
using CrownsGuard.Core.Figures;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.FigureDefinitions.Figures;

namespace CrownsGuard.FigureDefinitions.Utilities;

public static class FigureActionExecutor
{
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public static void ExecuteFigureAction(Span<Figure> board, FigureAction action, Action<BoardEvent, Span<Figure>> onEvent)
    {
        var sourceFigure = board[action.SourcePosition.GetIndex()];
        switch (sourceFigure.FigureType)
        {
            case FigureId.Alchemist: Alchemist.ExecuteAction(board, action, onEvent); break;
            case FigureId.Archer: Archer.ExecuteAction(board, action, onEvent); break;
            case FigureId.Barbarian: Barbarian.ExecuteAction(board, action, onEvent); break;
            case FigureId.Bard: Bard.ExecuteAction(board, action, onEvent); break;
            case FigureId.BattleAxe: BattleAxe.ExecuteAction(board, action, onEvent); break;
            case FigureId.Blade: Blade.ExecuteAction(board, action, onEvent); break;
            case FigureId.Builder: Builder.ExecuteAction(board, action, onEvent); break;
            case FigureId.CamelArcher: CamelArcher.ExecuteAction(board, action, onEvent); break;
            case FigureId.CamelRider: CamelRider.ExecuteAction(board, action, onEvent); break;
            case FigureId.Cannon: Cannon.ExecuteAction(board, action, onEvent); break;
            case FigureId.Catapult: Catapult.ExecuteAction(board, action, onEvent); break;
            case FigureId.Chinese: Chinese.ExecuteAction(board, action, onEvent); break;
            case FigureId.Crossbow: Crossbow.ExecuteAction(board, action, onEvent); break;
            case FigureId.Dogs: Dogs.ExecuteAction(board, action, onEvent); break;
            case FigureId.Dragon: Dragon.ExecuteAction(board, action, onEvent); break;
            case FigureId.Elephant: Elephant.ExecuteAction(board, action, onEvent); break;
            case FigureId.JapanArcher: JapanArcher.ExecuteAction(board, action, onEvent); break;
            case FigureId.King: King.ExecuteAction(board, action, onEvent); break;
            case FigureId.Knight: Knight.ExecuteAction(board, action, onEvent); break;
            case FigureId.LegionaryPike: LegionaryPike.ExecuteAction(board, action, onEvent); break;
            case FigureId.LegionarySword: LegionarySword.ExecuteAction(board, action, onEvent); break;
            case FigureId.Mage: Mage.ExecuteAction(board, action, onEvent); break;
            case FigureId.Miner: Miner.ExecuteAction(board, action, onEvent); break;
            case FigureId.MountedArcher: MountedArcher.ExecuteAction(board, action, onEvent); break;
            case FigureId.MountedKnight: MountedKnight.ExecuteAction(board, action, onEvent); break;
            case FigureId.Musketeer: Musketeer.ExecuteAction(board, action, onEvent); break;
            case FigureId.Ninja: Ninja.ExecuteAction(board, action, onEvent); break;
            case FigureId.Nordguard: Nordguard.ExecuteAction(board, action, onEvent); break;
            case FigureId.Peasant: Peasant.ExecuteAction(board, action, onEvent); break;
            case FigureId.Pikeman: Pikeman.ExecuteAction(board, action, onEvent); break;
            case FigureId.Priest: Priest.ExecuteAction(board, action, onEvent); break;
            case FigureId.Queen: Queen.ExecuteAction(board, action, onEvent); break;
            case FigureId.Ranger: Ranger.ExecuteAction(board, action, onEvent); break;
            case FigureId.Samurai: Samurai.ExecuteAction(board, action, onEvent); break;
            case FigureId.Scout: Scout.ExecuteAction(board, action, onEvent); break;
            case FigureId.Spartan: Spartan.ExecuteAction(board, action, onEvent); break;
            case FigureId.Spearman: Spearman.ExecuteAction(board, action, onEvent); break;
            case FigureId.Trader: Trader.ExecuteAction(board, action, onEvent); break;
            case FigureId.Warhammer: Warhammer.ExecuteAction(board, action, onEvent); break;
            case FigureId.Whiplash: Whiplash.ExecuteAction(board, action, onEvent); break;
            case FigureId.Wizzard: Wizzard.ExecuteAction(board, action, onEvent); break;
            default: throw new ArgumentOutOfRangeException();
        };
    }
}
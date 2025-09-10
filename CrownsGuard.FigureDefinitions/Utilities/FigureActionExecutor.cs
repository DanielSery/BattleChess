using CrownsGuard.Core.Figures;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.Core.Helpers;
using CrownsGuard.FigureDefinitions.Figures;

namespace CrownsGuard.FigureDefinitions.Utilities;

public static class FigureActionExecutor
{
    public static void ExecuteFigureAction(Span<Figure> board, ref FigureAction action, Action<BoardEvent, Span<Figure>> onEvent)
    {
        var sourceFigure = board[action.SourcePosition.GetIndex()];
        switch (sourceFigure.FigureType)
        {
            case FigureId.Alchemist: Alchemist.ExecuteAction(board, ref action, onEvent); break;
            case FigureId.Archer: Archer.ExecuteAction(board, ref action, onEvent); break;
            case FigureId.Barbarian: Barbarian.ExecuteAction(board, ref action, onEvent); break;
            case FigureId.Bard: Bard.ExecuteAction(board, ref action, onEvent); break;
            case FigureId.BattleAxe: BattleAxe.ExecuteAction(board, ref action, onEvent); break;
            case FigureId.Blade: Blade.ExecuteAction(board, ref action, onEvent); break;
            case FigureId.Builder: Builder.ExecuteAction(board, ref action, onEvent); break;
            case FigureId.CamelArcher: CamelArcher.ExecuteAction(board, ref action, onEvent); break;
            case FigureId.CamelRider: CamelRider.ExecuteAction(board, ref action, onEvent); break;
            case FigureId.Cannon: Cannon.ExecuteAction(board, ref action, onEvent); break;
            case FigureId.Catapult: Catapult.ExecuteAction(board, ref action, onEvent); break;
            case FigureId.Chinese: Chinese.ExecuteAction(board, ref action, onEvent); break;
            case FigureId.Crossbow: Crossbow.ExecuteAction(board, ref action, onEvent); break;
            case FigureId.Dogs: Dogs.ExecuteAction(board, ref action, onEvent); break;
            case FigureId.Dragon: Dragon.ExecuteAction(board, ref action, onEvent); break;
            case FigureId.Elephant: Elephant.ExecuteAction(board, ref action, onEvent); break;
            case FigureId.JapanArcher: JapanArcher.ExecuteAction(board, ref action, onEvent); break;
            case FigureId.King: King.ExecuteAction(board, ref action, onEvent); break;
            case FigureId.Knight: Knight.ExecuteAction(board, ref action, onEvent); break;
            case FigureId.LegionaryPike: LegionaryPike.ExecuteAction(board, ref action, onEvent); break;
            case FigureId.LegionarySword: LegionarySword.ExecuteAction(board, ref action, onEvent); break;
            case FigureId.Mage: Mage.ExecuteAction(board, ref action, onEvent); break;
            case FigureId.Miner: Miner.ExecuteAction(board, ref action, onEvent); break;
            case FigureId.MountedArcher: MountedArcher.ExecuteAction(board, ref action, onEvent); break;
            case FigureId.MountedKnight: MountedKnight.ExecuteAction(board, ref action, onEvent); break;
            case FigureId.Musketeer: Musketeer.ExecuteAction(board, ref action, onEvent); break;
            case FigureId.Ninja: Ninja.ExecuteAction(board, ref action, onEvent); break;
            case FigureId.Nordguard: Nordguard.ExecuteAction(board, ref action, onEvent); break;
            case FigureId.Peasant: Peasant.ExecuteAction(board, ref action, onEvent); break;
            case FigureId.Pikeman: Pikeman.ExecuteAction(board, ref action, onEvent); break;
            case FigureId.Priest: Priest.ExecuteAction(board, ref action, onEvent); break;
            case FigureId.Queen: Queen.ExecuteAction(board, ref action, onEvent); break;
            case FigureId.Ranger: Ranger.ExecuteAction(board, ref action, onEvent); break;
            case FigureId.Samurai: Samurai.ExecuteAction(board, ref action, onEvent); break;
            case FigureId.Scout: Scout.ExecuteAction(board, ref action, onEvent); break;
            case FigureId.Spartan: Spartan.ExecuteAction(board, ref action, onEvent); break;
            case FigureId.Spearman: Spearman.ExecuteAction(board, ref action, onEvent); break;
            case FigureId.Trader: Trader.ExecuteAction(board, ref action, onEvent); break;
            case FigureId.Warhammer: Warhammer.ExecuteAction(board, ref action, onEvent); break;
            case FigureId.Whiplash: Whiplash.ExecuteAction(board, ref action, onEvent); break;
            case FigureId.Wizzard: Wizzard.ExecuteAction(board, ref action, onEvent); break;
            default: throw new ArgumentOutOfRangeException();
        };
    }
}
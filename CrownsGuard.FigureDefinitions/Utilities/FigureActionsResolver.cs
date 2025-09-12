using System.Runtime.CompilerServices;
using CrownsGuard.Core.Figures;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.Core.Helpers;
using CrownsGuard.FigureDefinitions.Figures;

namespace CrownsGuard.FigureDefinitions.Utilities;

public static class FigureActionsResolver
{
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public static void GetPossibleActions(Position sourcePosition, ReadOnlySpan<Figure> board, Stack<FigureAction> actions)
    {
        var sourceFigure = board[sourcePosition.GetIndex()];
        switch (sourceFigure.FigureType)
        {
            case FigureId.Empty: Empty.GetPossibleActions(sourcePosition, sourceFigure, board, actions); break;
            case FigureId.Alchemist: Alchemist.GetPossibleActions(sourcePosition, sourceFigure, board, actions); break;
            case FigureId.Archer: Archer.GetPossibleActions(sourcePosition, sourceFigure, board, actions); break;
            case FigureId.Barbarian: Barbarian.GetPossibleActions(sourcePosition, sourceFigure, board, actions); break;
            case FigureId.Bard: Bard.GetPossibleActions(sourcePosition, sourceFigure, board, actions); break;
            case FigureId.BattleAxe: BattleAxe.GetPossibleActions(sourcePosition, sourceFigure, board, actions); break;
            case FigureId.Blade: Blade.GetPossibleActions(sourcePosition, sourceFigure, board, actions); break;
            case FigureId.Builder: Builder.GetPossibleActions(sourcePosition, sourceFigure, board, actions); break;
            case FigureId.CamelArcher: CamelArcher.GetPossibleActions(sourcePosition, sourceFigure, board, actions); break;
            case FigureId.CamelRider: CamelRider.GetPossibleActions(sourcePosition, sourceFigure, board, actions); break;
            case FigureId.Cannon: Cannon.GetPossibleActions(sourcePosition, sourceFigure, board, actions); break;
            case FigureId.Catapult: Catapult.GetPossibleActions(sourcePosition, sourceFigure, board, actions); break;
            case FigureId.Chinese: Chinese.GetPossibleActions(sourcePosition, sourceFigure, board, actions); break;
            case FigureId.Crossbow: Crossbow.GetPossibleActions(sourcePosition, sourceFigure, board, actions); break;
            case FigureId.Dogs: Dogs.GetPossibleActions(sourcePosition, sourceFigure, board, actions); break;
            case FigureId.Dragon: Dragon.GetPossibleActions(sourcePosition, sourceFigure, board, actions); break;
            case FigureId.Elephant: Elephant.GetPossibleActions(sourcePosition, sourceFigure, board, actions); break;
            case FigureId.Explosives: Explosives.GetPossibleActions(sourcePosition, sourceFigure, board, actions); break;
            case FigureId.Fire: Fire.GetPossibleActions(sourcePosition, sourceFigure, board, actions); break;
            case FigureId.JapanArcher: JapanArcher.GetPossibleActions(sourcePosition, sourceFigure, board, actions); break;
            case FigureId.King: King.GetPossibleActions(sourcePosition, sourceFigure, board, actions); break;
            case FigureId.Knight: Knight.GetPossibleActions(sourcePosition, sourceFigure, board, actions); break;
            case FigureId.LegionaryPike: LegionaryPike.GetPossibleActions(sourcePosition, sourceFigure, board, actions); break;
            case FigureId.LegionarySword: LegionarySword.GetPossibleActions(sourcePosition, sourceFigure, board, actions); break;
            case FigureId.Mage: Mage.GetPossibleActions(sourcePosition, sourceFigure, board, actions); break;
            case FigureId.Miner: Miner.GetPossibleActions(sourcePosition, sourceFigure, board, actions); break;
            case FigureId.MountedArcher: MountedArcher.GetPossibleActions(sourcePosition, sourceFigure, board, actions); break;
            case FigureId.MountedKnight: MountedKnight.GetPossibleActions(sourcePosition, sourceFigure, board, actions); break;
            case FigureId.Musketeer: Musketeer.GetPossibleActions(sourcePosition, sourceFigure, board, actions); break;
            case FigureId.Ninja: Ninja.GetPossibleActions(sourcePosition, sourceFigure, board, actions); break;
            case FigureId.Nordguard: Nordguard.GetPossibleActions(sourcePosition, sourceFigure, board, actions); break;
            case FigureId.Peasant: Peasant.GetPossibleActions(sourcePosition, sourceFigure, board, actions); break;
            case FigureId.Pikeman: Pikeman.GetPossibleActions(sourcePosition, sourceFigure, board, actions); break;
            case FigureId.Priest: Priest.GetPossibleActions(sourcePosition, sourceFigure, board, actions); break;
            case FigureId.Queen: Queen.GetPossibleActions(sourcePosition, sourceFigure, board, actions); break;
            case FigureId.Ranger: Ranger.GetPossibleActions(sourcePosition, sourceFigure, board, actions); break;
            case FigureId.Samurai: Samurai.GetPossibleActions(sourcePosition, sourceFigure, board, actions); break;
            case FigureId.Scout: Scout.GetPossibleActions(sourcePosition, sourceFigure, board, actions); break;
            case FigureId.Spartan: Spartan.GetPossibleActions(sourcePosition, sourceFigure, board, actions); break;
            case FigureId.Spearman: Spearman.GetPossibleActions(sourcePosition, sourceFigure, board, actions); break;
            case FigureId.Trader: Trader.GetPossibleActions(sourcePosition, sourceFigure, board, actions); break;
            case FigureId.Trench: Trench.GetPossibleActions(sourcePosition, sourceFigure, board, actions); break;
            case FigureId.Wall: Wall.GetPossibleActions(sourcePosition, sourceFigure, board, actions); break;
            case FigureId.Warhammer: Warhammer.GetPossibleActions(sourcePosition, sourceFigure, board, actions); break;
            case FigureId.Whiplash: Whiplash.GetPossibleActions(sourcePosition, sourceFigure, board, actions); break;
            case FigureId.Wizzard: Wizzard.GetPossibleActions(sourcePosition, sourceFigure, board, actions); break;
            default: throw new ArgumentOutOfRangeException();
        }
    }
}
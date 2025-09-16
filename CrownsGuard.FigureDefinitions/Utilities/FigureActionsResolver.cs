using System.Runtime.CompilerServices;
using CrownsGuard.Core.Figures;
using CrownsGuard.Core.Helpers;
using CrownsGuard.FigureDefinitions.Figures;

namespace CrownsGuard.FigureDefinitions.Utilities;

public static class FigureActionsResolver
{
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public static void GetPossibleActions(int sourceIndex, Figure sourceFigure, ReadOnlySpan<Figure> board, Stack<FigureAction> actions)
    {
        if (sourceFigure <= Figure.LastNeutralFigure)
            return;
        
        switch (sourceFigure.GetFigureType())
        {
            case Figure.Empty:
            case Figure.Explosives:
            case Figure.Fire:
            case Figure.Trench:
            case Figure.Wall: break;
            case Figure.Alchemist: Alchemist.GetPossibleActions(sourceIndex, sourceFigure, board, actions); break;
            case Figure.Archer: Archer.GetPossibleActions(sourceIndex, sourceFigure, board, actions); break;
            case Figure.Barbarian: Barbarian.GetPossibleActions(sourceIndex, sourceFigure, board, actions); break;
            case Figure.Bard: Bard.GetPossibleActions(sourceIndex, sourceFigure, board, actions); break;
            case Figure.BattleAxe: BattleAxe.GetPossibleActions(sourceIndex, sourceFigure, board, actions); break;
            case Figure.Blade: Blade.GetPossibleActions(sourceIndex, sourceFigure, board, actions); break;
            case Figure.Builder: Builder.GetPossibleActions(sourceIndex, sourceFigure, board, actions); break;
            case Figure.CamelArcher: CamelArcher.GetPossibleActions(sourceIndex, sourceFigure, board, actions); break;
            case Figure.CamelRider: CamelRider.GetPossibleActions(sourceIndex, sourceFigure, board, actions); break;
            case Figure.Cannon: Cannon.GetPossibleActions(sourceIndex, sourceFigure, board, actions); break;
            case Figure.Catapult: Catapult.GetPossibleActions(sourceIndex, sourceFigure, board, actions); break;
            case Figure.Chinese: Chinese.GetPossibleActions(sourceIndex, sourceFigure, board, actions); break;
            case Figure.Crossbow: Crossbow.GetPossibleActions(sourceIndex, sourceFigure, board, actions); break;
            case Figure.Dogs: Dogs.GetPossibleActions(sourceIndex, sourceFigure, board, actions); break;
            case Figure.Dragon: Dragon.GetPossibleActions(sourceIndex, sourceFigure, board, actions); break;
            case Figure.Elephant: Elephant.GetPossibleActions(sourceIndex, sourceFigure, board, actions); break;
            case Figure.JapanArcher: JapanArcher.GetPossibleActions(sourceIndex, sourceFigure, board, actions); break;
            case Figure.King: King.GetPossibleActions(sourceIndex, sourceFigure, board, actions); break;
            case Figure.Knight: Knight.GetPossibleActions(sourceIndex, sourceFigure, board, actions); break;
            case Figure.LegionaryPike: LegionaryPike.GetPossibleActions(sourceIndex, sourceFigure, board, actions); break;
            case Figure.LegionarySword: LegionarySword.GetPossibleActions(sourceIndex, sourceFigure, board, actions); break;
            case Figure.Mage: Mage.GetPossibleActions(sourceIndex, sourceFigure, board, actions); break;
            case Figure.Miner: Miner.GetPossibleActions(sourceIndex, sourceFigure, board, actions); break;
            case Figure.MountedArcher: MountedArcher.GetPossibleActions(sourceIndex, sourceFigure, board, actions); break;
            case Figure.MountedKnight: MountedKnight.GetPossibleActions(sourceIndex, sourceFigure, board, actions); break;
            case Figure.Musketeer: Musketeer.GetPossibleActions(sourceIndex, sourceFigure, board, actions); break;
            case Figure.Ninja: Ninja.GetPossibleActions(sourceIndex, sourceFigure, board, actions); break;
            case Figure.Nordguard: Nordguard.GetPossibleActions(sourceIndex, sourceFigure, board, actions); break;
            case Figure.Peasant: Peasant.GetPossibleActions(sourceIndex, sourceFigure, board, actions); break;
            case Figure.Pikeman: Pikeman.GetPossibleActions(sourceIndex, sourceFigure, board, actions); break;
            case Figure.Priest: Priest.GetPossibleActions(sourceIndex, sourceFigure, board, actions); break;
            case Figure.Queen: Queen.GetPossibleActions(sourceIndex, sourceFigure, board, actions); break;
            case Figure.Ranger: Ranger.GetPossibleActions(sourceIndex, sourceFigure, board, actions); break;
            case Figure.Samurai: Samurai.GetPossibleActions(sourceIndex, sourceFigure, board, actions); break;
            case Figure.Scout: Scout.GetPossibleActions(sourceIndex, sourceFigure, board, actions); break;
            case Figure.Spartan: Spartan.GetPossibleActions(sourceIndex, sourceFigure, board, actions); break;
            case Figure.Spearman: Spearman.GetPossibleActions(sourceIndex, sourceFigure, board, actions); break;
            case Figure.Trader: Trader.GetPossibleActions(sourceIndex, sourceFigure, board, actions); break;
            case Figure.Warhammer: Warhammer.GetPossibleActions(sourceIndex, sourceFigure, board, actions); break;
            case Figure.Whiplash: Whiplash.GetPossibleActions(sourceIndex, sourceFigure, board, actions); break;
            case Figure.Wizzard: Wizzard.GetPossibleActions(sourceIndex, sourceFigure, board, actions); break;
            default: throw new ArgumentOutOfRangeException();
        }
    }
}
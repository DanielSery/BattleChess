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
        switch (sourceFigure.GetFigureType())
        {
            case Figure.Empty: Empty.GetPossibleActions(sourcePosition, sourceFigure, board, actions); break;
            case Figure.Alchemist: Alchemist.GetPossibleActions(sourcePosition, sourceFigure, board, actions); break;
            case Figure.Archer: Archer.GetPossibleActions(sourcePosition, sourceFigure, board, actions); break;
            case Figure.Barbarian: Barbarian.GetPossibleActions(sourcePosition, sourceFigure, board, actions); break;
            case Figure.Bard: Bard.GetPossibleActions(sourcePosition, sourceFigure, board, actions); break;
            case Figure.BattleAxe: BattleAxe.GetPossibleActions(sourcePosition, sourceFigure, board, actions); break;
            case Figure.Blade: Blade.GetPossibleActions(sourcePosition, sourceFigure, board, actions); break;
            case Figure.Builder: Builder.GetPossibleActions(sourcePosition, sourceFigure, board, actions); break;
            case Figure.CamelArcher: CamelArcher.GetPossibleActions(sourcePosition, sourceFigure, board, actions); break;
            case Figure.CamelRider: CamelRider.GetPossibleActions(sourcePosition, sourceFigure, board, actions); break;
            case Figure.Cannon: Cannon.GetPossibleActions(sourcePosition, sourceFigure, board, actions); break;
            case Figure.Catapult: Catapult.GetPossibleActions(sourcePosition, sourceFigure, board, actions); break;
            case Figure.Chinese: Chinese.GetPossibleActions(sourcePosition, sourceFigure, board, actions); break;
            case Figure.Crossbow: Crossbow.GetPossibleActions(sourcePosition, sourceFigure, board, actions); break;
            case Figure.Dogs: Dogs.GetPossibleActions(sourcePosition, sourceFigure, board, actions); break;
            case Figure.Dragon: Dragon.GetPossibleActions(sourcePosition, sourceFigure, board, actions); break;
            case Figure.Elephant: Elephant.GetPossibleActions(sourcePosition, sourceFigure, board, actions); break;
            case Figure.Explosives: Explosives.GetPossibleActions(sourcePosition, sourceFigure, board, actions); break;
            case Figure.Fire: Fire.GetPossibleActions(sourcePosition, sourceFigure, board, actions); break;
            case Figure.JapanArcher: JapanArcher.GetPossibleActions(sourcePosition, sourceFigure, board, actions); break;
            case Figure.King: King.GetPossibleActions(sourcePosition, sourceFigure, board, actions); break;
            case Figure.Knight: Knight.GetPossibleActions(sourcePosition, sourceFigure, board, actions); break;
            case Figure.LegionaryPike: LegionaryPike.GetPossibleActions(sourcePosition, sourceFigure, board, actions); break;
            case Figure.LegionarySword: LegionarySword.GetPossibleActions(sourcePosition, sourceFigure, board, actions); break;
            case Figure.Mage: Mage.GetPossibleActions(sourcePosition, sourceFigure, board, actions); break;
            case Figure.Miner: Miner.GetPossibleActions(sourcePosition, sourceFigure, board, actions); break;
            case Figure.MountedArcher: MountedArcher.GetPossibleActions(sourcePosition, sourceFigure, board, actions); break;
            case Figure.MountedKnight: MountedKnight.GetPossibleActions(sourcePosition, sourceFigure, board, actions); break;
            case Figure.Musketeer: Musketeer.GetPossibleActions(sourcePosition, sourceFigure, board, actions); break;
            case Figure.Ninja: Ninja.GetPossibleActions(sourcePosition, sourceFigure, board, actions); break;
            case Figure.Nordguard: Nordguard.GetPossibleActions(sourcePosition, sourceFigure, board, actions); break;
            case Figure.Peasant: Peasant.GetPossibleActions(sourcePosition, sourceFigure, board, actions); break;
            case Figure.Pikeman: Pikeman.GetPossibleActions(sourcePosition, sourceFigure, board, actions); break;
            case Figure.Priest: Priest.GetPossibleActions(sourcePosition, sourceFigure, board, actions); break;
            case Figure.Queen: Queen.GetPossibleActions(sourcePosition, sourceFigure, board, actions); break;
            case Figure.Ranger: Ranger.GetPossibleActions(sourcePosition, sourceFigure, board, actions); break;
            case Figure.Samurai: Samurai.GetPossibleActions(sourcePosition, sourceFigure, board, actions); break;
            case Figure.Scout: Scout.GetPossibleActions(sourcePosition, sourceFigure, board, actions); break;
            case Figure.Spartan: Spartan.GetPossibleActions(sourcePosition, sourceFigure, board, actions); break;
            case Figure.Spearman: Spearman.GetPossibleActions(sourcePosition, sourceFigure, board, actions); break;
            case Figure.Trader: Trader.GetPossibleActions(sourcePosition, sourceFigure, board, actions); break;
            case Figure.Trench: Trench.GetPossibleActions(sourcePosition, sourceFigure, board, actions); break;
            case Figure.Wall: Wall.GetPossibleActions(sourcePosition, sourceFigure, board, actions); break;
            case Figure.Warhammer: Warhammer.GetPossibleActions(sourcePosition, sourceFigure, board, actions); break;
            case Figure.Whiplash: Whiplash.GetPossibleActions(sourcePosition, sourceFigure, board, actions); break;
            case Figure.Wizzard: Wizzard.GetPossibleActions(sourcePosition, sourceFigure, board, actions); break;
            default: throw new ArgumentOutOfRangeException();
        }
    }
}
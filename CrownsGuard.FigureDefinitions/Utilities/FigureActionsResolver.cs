using CrownsGuard.Core.Figures;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.Core.Helpers;
using CrownsGuard.FigureDefinitions.Figures;

namespace CrownsGuard.FigureDefinitions.Utilities;

public static class FigureActionsResolver
{
    public static ArrayPoolMemory<FigureAction> GetPossibleActions(Position sourcePosition, Span<Figure> board)
    {
        var sourceFigure = board[sourcePosition.GetIndex()];
        return sourceFigure.FigureType switch
        {
            FigureId.Empty => Empty.GetPossibleActions(sourcePosition, sourceFigure, board),
            FigureId.Alchemist => Alchemist.GetPossibleActions(sourcePosition, sourceFigure, board),
            FigureId.Archer => Archer.GetPossibleActions(sourcePosition, sourceFigure, board),
            FigureId.Barbarian => Barbarian.GetPossibleActions(sourcePosition, sourceFigure, board),
            FigureId.Bard => Bard.GetPossibleActions(sourcePosition, sourceFigure, board),
            FigureId.BattleAxe => BattleAxe.GetPossibleActions(sourcePosition, sourceFigure, board),
            FigureId.Blade => Blade.GetPossibleActions(sourcePosition, sourceFigure, board),
            FigureId.Builder => Builder.GetPossibleActions(sourcePosition, sourceFigure, board),
            FigureId.CamelArcher => CamelArcher.GetPossibleActions(sourcePosition, sourceFigure, board),
            FigureId.CamelRider => CamelRider.GetPossibleActions(sourcePosition, sourceFigure, board),
            FigureId.Cannon => Cannon.GetPossibleActions(sourcePosition, sourceFigure, board),
            FigureId.Catapult => Catapult.GetPossibleActions(sourcePosition, sourceFigure, board),
            FigureId.Chinese => Chinese.GetPossibleActions(sourcePosition, sourceFigure, board),
            FigureId.Crossbow => Crossbow.GetPossibleActions(sourcePosition, sourceFigure, board),
            FigureId.Dogs => Dogs.GetPossibleActions(sourcePosition, sourceFigure, board),
            FigureId.Dragon => Dragon.GetPossibleActions(sourcePosition, sourceFigure, board),
            FigureId.Elephant => Elephant.GetPossibleActions(sourcePosition, sourceFigure, board),
            FigureId.Explosives => Explosives.GetPossibleActions(sourcePosition, sourceFigure, board),
            FigureId.Fire => Fire.GetPossibleActions(sourcePosition, sourceFigure, board),
            FigureId.JapanArcher => JapanArcher.GetPossibleActions(sourcePosition, sourceFigure, board),
            FigureId.King => King.GetPossibleActions(sourcePosition, sourceFigure, board),
            FigureId.Knight => Knight.GetPossibleActions(sourcePosition, sourceFigure, board),
            FigureId.LegionaryPike => LegionaryPike.GetPossibleActions(sourcePosition, sourceFigure, board),
            FigureId.LegionarySword => LegionarySword.GetPossibleActions(sourcePosition, sourceFigure, board),
            FigureId.Mage => Mage.GetPossibleActions(sourcePosition, sourceFigure, board),
            FigureId.Miner => Miner.GetPossibleActions(sourcePosition, sourceFigure, board),
            FigureId.MountedArcher => MountedArcher.GetPossibleActions(sourcePosition, sourceFigure, board),
            FigureId.MountedKnight => MountedKnight.GetPossibleActions(sourcePosition, sourceFigure, board),
            FigureId.Musketeer => Musketeer.GetPossibleActions(sourcePosition, sourceFigure, board),
            FigureId.Ninja => Ninja.GetPossibleActions(sourcePosition, sourceFigure, board),
            FigureId.Nordguard => Nordguard.GetPossibleActions(sourcePosition, sourceFigure, board),
            FigureId.Peasant => Peasant.GetPossibleActions(sourcePosition, sourceFigure, board),
            FigureId.Pikeman => Pikeman.GetPossibleActions(sourcePosition, sourceFigure, board),
            FigureId.Priest => Priest.GetPossibleActions(sourcePosition, sourceFigure, board),
            FigureId.Queen => Queen.GetPossibleActions(sourcePosition, sourceFigure, board),
            FigureId.Ranger => Ranger.GetPossibleActions(sourcePosition, sourceFigure, board),
            FigureId.Samurai => Samurai.GetPossibleActions(sourcePosition, sourceFigure, board),
            FigureId.Scout => Scout.GetPossibleActions(sourcePosition, sourceFigure, board),
            FigureId.Spartan => Spartan.GetPossibleActions(sourcePosition, sourceFigure, board),
            FigureId.Spearman => Spearman.GetPossibleActions(sourcePosition, sourceFigure, board),
            FigureId.Trader => Trader.GetPossibleActions(sourcePosition, sourceFigure, board),
            FigureId.Trench => Trench.GetPossibleActions(sourcePosition, sourceFigure, board),
            FigureId.Wall => Wall.GetPossibleActions(sourcePosition, sourceFigure, board),
            FigureId.Warhammer => Warhammer.GetPossibleActions(sourcePosition, sourceFigure, board),
            FigureId.Whiplash => Whiplash.GetPossibleActions(sourcePosition, sourceFigure, board),
            FigureId.Wizzard => Wizzard.GetPossibleActions(sourcePosition, sourceFigure, board),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}
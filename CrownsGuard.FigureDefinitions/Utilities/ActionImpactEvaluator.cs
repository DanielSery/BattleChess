// Copyright (c) Veeam Software Group GmbH

using System.Runtime.CompilerServices;
using CrownsGuard.Core.Figures;
using CrownsGuard.FigureDefinitions.Figures;

namespace CrownsGuard.FigureDefinitions.Utilities;

public class ActionImpactEvaluator
{
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public static int EvaluateAction(ReadOnlySpan<Figure> board, FigureAction action, Figure sourceFigure)
    {
        return sourceFigure.FigureType switch
        {
            FigureId.Alchemist => Alchemist.EvaluateAction(action),
            FigureId.Archer => Archer.EvaluateAction(board, action),
            FigureId.Barbarian => Barbarian.EvaluateAction(),
            FigureId.Bard => Bard.EvaluateAction(board, action),
            FigureId.BattleAxe => BattleAxe.EvaluateAction(board, action),
            FigureId.Blade => Blade.EvaluateAction(board, action),
            FigureId.Builder => Builder.EvaluateAction(action),
            FigureId.CamelArcher => CamelArcher.EvaluateAction(board, action),
            FigureId.CamelRider => CamelRider.EvaluateAction(board, action),
            FigureId.Cannon => Cannon.EvaluateAction(board, action),
            FigureId.Catapult => Catapult.EvaluateAction(board, action),
            FigureId.Chinese => Chinese.EvaluateAction(board, action),
            FigureId.Crossbow => Crossbow.EvaluateAction(board, action),
            FigureId.Dogs => Dogs.EvaluateAction(board, action),
            FigureId.Dragon => Dragon.EvaluateAction(action),
            FigureId.Elephant => Elephant.EvaluateAction(board, action),
            FigureId.JapanArcher => JapanArcher.EvaluateAction(board, action),
            FigureId.King => King.EvaluateAction(board, action),
            FigureId.Knight => Knight.EvaluateAction(board, action),
            FigureId.LegionaryPike => LegionaryPike.EvaluateAction(board, action),
            FigureId.LegionarySword => LegionarySword.EvaluateAction(board, action),
            FigureId.Mage => Mage.EvaluateAction(action),
            FigureId.Miner => Miner.EvaluateAction(action),
            FigureId.MountedArcher => MountedArcher.EvaluateAction(board, action),
            FigureId.MountedKnight => MountedKnight.EvaluateAction(board, action),
            FigureId.Musketeer => Musketeer.EvaluateAction(board, action),
            FigureId.Ninja => Ninja.EvaluateAction(board, action),
            FigureId.Nordguard => Nordguard.EvaluateAction(board, action),
            FigureId.Peasant => Peasant.EvaluateAction(board, action),
            FigureId.Pikeman => Pikeman.EvaluateAction(board, action),
            FigureId.Priest => Priest.EvaluateAction(board, action),
            FigureId.Queen => Queen.EvaluateAction(board, action),
            FigureId.Ranger => Ranger.EvaluateAction(board, action),
            FigureId.Samurai => Samurai.EvaluateAction(board, action),
            FigureId.Scout => Scout.EvaluateAction(board, action),
            FigureId.Spartan => Spartan.EvaluateAction(action),
            FigureId.Spearman => Spearman.EvaluateAction(board, action),
            FigureId.Trader => Trader.EvaluateAction(board, action),
            FigureId.Warhammer => Warhammer.EvaluateAction(board, action),
            FigureId.Whiplash => Whiplash.EvaluateAction(board, action),
            FigureId.Wizzard => Wizzard.EvaluateAction(board, action),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}
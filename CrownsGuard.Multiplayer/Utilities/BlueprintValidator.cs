using CrownsGuard.Core.Figures;
using CrownsGuard.Core.Helpers;
using System.Collections;
using FluentResults;

namespace CrownsGuard.Multiplayer.Utilities;

public static class BlueprintValidator
{
    public static Result ValidateResult(this Figure[] boardBlueprint, IReadOnlyList<byte>? unlockedFigures)
    {
        unlockedFigures ??= UnlockedFigures.DefaultUnlockedFigures;
        var bitArray = new BitArray(unlockedFigures.ToArray());
        var kingCount = 0;
        var totalValue = 0;
        
        foreach (var x in boardBlueprint)
        {
            var isNeutralFigure = x.IsNeutralFigure();
            var figureColor = x.GetFigureColor();
            
            if (isNeutralFigure && figureColor != Figure.Empty) return Result.Fail("Setup contains neutral figure assigned to player");
            if (!isNeutralFigure && figureColor == Figure.IsBlack) return Result.Fail("Setup contains black figure");
            if (!isNeutralFigure && figureColor == Figure.Empty) return Result.Fail("Setup contains player figure without player assigned");
            
            if (x.IsKing()) kingCount++;
            if (!bitArray[(int)x.GetFigureType()]) return Result.Fail("Setup contains not unlocked figure");
            totalValue += x.GetFigureValue();
        }

        if (boardBlueprint.Length != 16) return Result.Fail("Invalid number of figures in setup");
        if (kingCount == 0) return Result.Fail("Setup does not contain king");
        if (kingCount > 1) return Result.Fail("Setup contains more than one king");
        if (totalValue > Constants.MaxMapsPoints) return Result.Fail("Setup total figures value is too high");
        return Result.Ok();
    }
}
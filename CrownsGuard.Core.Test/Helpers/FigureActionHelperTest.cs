using CrownsGuard.Core.Figures;
using CrownsGuard.Core.Helpers;
using JetBrains.Annotations;

namespace CrownsGuard.Core.Test.Helpers;

[TestSubject(typeof(FigureActionHelper))]
public class FigureActionHelperTest
{
    [Theory]
    [InlineData(FigureActionType.Move)]
    [InlineData(FigureActionType.PushFigure)]
    [InlineData(FigureActionType.ChangeToQueen)]
    [InlineData(FigureActionType.MakeUnitKing)]
    [InlineData(FigureActionType.BuildWall)]
    [InlineData(FigureActionType.MinerMove)]
    [InlineData(FigureActionType.BreatheFire)]
    [InlineData(FigureActionType.AlchemistMove)]
    [InlineData(FigureActionType.SwapWithFigure)]
    [InlineData(FigureActionType.Castling)]
    [InlineData(FigureActionType.MeleeAttack)]
    [InlineData(FigureActionType.SpartanMove)]
    [InlineData(FigureActionType.MeleePierceAttack)]
    [InlineData(FigureActionType.BattleAxeMove)]
    [InlineData(FigureActionType.WarhammerMove)]
    [InlineData(FigureActionType.RangedAttack)]
    [InlineData(FigureActionType.MageMove)]
    [InlineData(FigureActionType.WizzardMove)]
    [InlineData(FigureActionType.ConvertUnit)]
    [InlineData(FigureActionType.CannonAttack)]
    public void IsExecutable_ReturnsTrue(FigureActionType actionType)
    {
        // Act
        var result = actionType.IsExecutable();

        // Assert
        Assert.True(result);
    }

    [Theory]
    [InlineData(FigureActionType.None)]
    [InlineData(FigureActionType.PossiblePushFigure)]
    [InlineData(FigureActionType.PossibleCastling)]
    [InlineData(FigureActionType.PossibleMeleeAttack)]
    [InlineData(FigureActionType.MeleeDefend)]
    [InlineData(FigureActionType.PossibleMeleePierceAttack)]
    [InlineData(FigureActionType.PossibleRangedAttack)]
    [InlineData(FigureActionType.PossibleConvertUnit)]
    [InlineData(FigureActionType.PossibleCannonAttack)]
    public void IsExecutable_ReturnsFalse(FigureActionType actionType)
    {
        // Act
        var result = actionType.IsExecutable();

        // Assert
        Assert.False(result);
    }
}
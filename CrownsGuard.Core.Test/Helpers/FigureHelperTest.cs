using AwesomeAssertions;
using CrownsGuard.Core.Figures;
using CrownsGuard.Core.Helpers;

namespace CrownsGuard.Core.Test.Helpers;

public class FigureHelperTest
{
    [Theory]
    [InlineData(Figure.Knight, Figure.Knight)]
    [InlineData(Figure.Knight | Figure.IsWhite, Figure.Knight)]
    [InlineData(Figure.Knight | Figure.IsWhite | Figure.IsKing, Figure.Knight)]
    [InlineData(Figure.Empty, Figure.Empty)]
    [InlineData(Figure.MountedArcher | Figure.IsBlack, Figure.MountedArcher)]
    public void GetFigureType_ReturnsBaseFigure(Figure input, Figure expected)
    {
        // Act
        var result = input.GetFigureType();

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(Figure.Knight, Figure.Empty)]
    [InlineData(Figure.Knight | Figure.IsKing, Figure.IsKing)]
    [InlineData(Figure.Knight | Figure.IsWhite | Figure.IsKing, Figure.IsKing)]
    [InlineData(Figure.Knight | Figure.IsWhite, Figure.Empty)]
    [InlineData(Figure.Empty, Figure.Empty)]
    [InlineData(Figure.IsKing, Figure.IsKing)]
    [InlineData(Figure.MountedArcher | Figure.IsBlack | Figure.IsKing, Figure.IsKing)]
    [InlineData(Figure.MountedArcher | Figure.IsBlack, Figure.Empty)]
    public void GetIsKing_ReturnsIsKingFlagIfPresent(Figure input, Figure expected)
    {
        // Act
        var result = input.GetIsKing();

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    // Same color, same figure
    [InlineData(Figure.Knight | Figure.IsWhite, Figure.Queen | Figure.IsWhite, true)]
    // Same color, different figures
    [InlineData(Figure.MountedArcher | Figure.IsBlack, Figure.CamelArcher | Figure.IsBlack, true)]
    // Different color, same base figure
    [InlineData(Figure.Knight | Figure.IsWhite, Figure.Knight | Figure.IsBlack, false)]
    // Different color, different figures
    [InlineData(Figure.Knight | Figure.IsWhite, Figure.CamelArcher | Figure.IsBlack, false)]
    // Both neutral
    [InlineData(Figure.Wall, Figure.Trench, true)]
    // Neutral vs colored
    [InlineData(Figure.Wall, Figure.Knight | Figure.IsWhite, false)]
    [InlineData(Figure.Knight | Figure.IsBlack, Figure.Trench, false)]
    // Both empty
    [InlineData(Figure.Empty, Figure.Empty, true)]
    // Empty vs colored
    [InlineData(Figure.Empty, Figure.Knight | Figure.IsWhite, false)]
    [InlineData(Figure.Knight | Figure.IsBlack, Figure.Empty, false)]
    public void IsSameColor_ReturnsExpectedResult(Figure a, Figure b, bool expected)
    {
        // Act
        var result = a.IsSameColor(b);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    // Neutral figure (no color)
    [InlineData(Figure.Wall, Figure.Empty)]
    // White figure
    [InlineData(Figure.Knight | Figure.IsWhite, Figure.IsWhite)]
    // Black figure
    [InlineData(Figure.Knight | Figure.IsBlack, Figure.IsBlack)]
    // White king
    [InlineData(Figure.Knight | Figure.IsWhite | Figure.IsKing, Figure.IsWhite)]
    // Black king
    [InlineData(Figure.Knight | Figure.IsBlack | Figure.IsKing, Figure.IsBlack)]
    // Neutral king (should still be neutral)
    [InlineData(Figure.Wall | Figure.IsKing, Figure.Empty)]
    // Empty
    [InlineData(Figure.Empty, Figure.Empty)]
    // Only color flag (no base figure)
    [InlineData(Figure.IsWhite, Figure.IsWhite)]
    [InlineData(Figure.IsBlack, Figure.IsBlack)]
    public void GetFigureColor_ReturnsExpectedColor(Figure input, Figure expected)
    {
        // Act
        var result = input.GetFigureColor();

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    // Neutral figures (should be true)
    [InlineData(Figure.Empty, true)]
    [InlineData(Figure.Fire, true)]
    [InlineData(Figure.Wall, true)]
    [InlineData(Figure.Trench, true)]
    [InlineData(Figure.Explosives, true)]
    // Neutral figures with flags (should be false)
    [InlineData(Figure.Wall | Figure.IsKing, false)]
    [InlineData(Figure.Trench | Figure.IsWhite, false)]
    [InlineData(Figure.Explosives | Figure.IsBlack, false)]
    // Non-neutral figures (should be false)
    [InlineData(Figure.MountedKnight, false)]
    [InlineData(Figure.Knight, false)]
    [InlineData(Figure.Queen, false)]
    // Non-neutral figures with flags (should be false)
    [InlineData(Figure.Knight | Figure.IsWhite, false)]
    [InlineData(Figure.MountedKnight | Figure.IsBlack, false)]
    [InlineData(Figure.Knight | Figure.IsWhite | Figure.IsKing, false)]
    public void IsNeutralFigure_ReturnsExpectedResult(Figure input, bool expected)
    {
        // Act
        var result = input.IsNeutralFigure();

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    // Neutral figures (no color)
    [InlineData(Figure.Empty, true)]
    [InlineData(Figure.Wall, true)]
    [InlineData(Figure.Trench, true)]
    [InlineData(Figure.Explosives, true)]
    // Neutral figures with color flags (should be false)
    [InlineData(Figure.Wall | Figure.IsWhite, false)]
    [InlineData(Figure.Trench | Figure.IsBlack, false)]
    [InlineData(Figure.Explosives | Figure.IsWhite, false)]
    // Colored figures (should be false)
    [InlineData(Figure.Knight | Figure.IsWhite, false)]
    [InlineData(Figure.Knight | Figure.IsBlack, false)]
    [InlineData(Figure.MountedKnight | Figure.IsWhite, false)]
    // Only color flags (should be false)
    [InlineData(Figure.IsWhite, false)]
    [InlineData(Figure.IsBlack, false)]
    // Non-neutral, non-coloredlu figures (should be true)
    [InlineData(Figure.Knight, true)]
    [InlineData(Figure.MountedKnight, true)]
    public void IsNeutral_ReturnsExpectedResult(Figure input, bool expected)
    {
        // Act
        var result = input.IsNeutral();

        // Assert
        result.Should().Be(expected);
    }
    
    [Theory]
    [InlineData(Figure.Empty, true)]
    [InlineData(Figure.Fire, true)]
    [InlineData(Figure.Wall, false)]
    [InlineData(Figure.Trench, false)]
    [InlineData(Figure.Explosives, false)]
    [InlineData(Figure.MountedKnight, false)]
    [InlineData(Figure.King, false)]
    [InlineData(Figure.Bard, false)]
    public void IsWalkable_BaseFigures(Figure figure, bool expected)
    {
        var actual = figure.IsWalkable();
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(Figure.Empty | Figure.IsWhite, true)]
    [InlineData(Figure.Fire | Figure.IsWhite, true)]
    [InlineData(Figure.Empty | Figure.IsBlack, true)]
    [InlineData(Figure.Fire | Figure.IsBlack, true)]
    [InlineData(Figure.Wall | Figure.IsWhite, false)]
    [InlineData(Figure.Wall | Figure.IsBlack, false)]
    [InlineData(Figure.Trench | Figure.IsWhite, false)]
    [InlineData(Figure.Trench | Figure.IsBlack, false)]
    public void IsWalkable_WithColorFlags(Figure figure, bool expected)
    {
        var actual = figure.IsWalkable();
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(Figure.Empty | Figure.IsKing, true)]
    [InlineData(Figure.Fire | Figure.IsKing, true)]
    [InlineData(Figure.Wall | Figure.IsKing, false)]
    [InlineData(Figure.MountedKnight | Figure.IsKing, false)]
    public void IsWalkable_WithKingFlag(Figure figure, bool expected)
    {
        var actual = figure.IsWalkable();
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(Figure.Empty | Figure.PlayerMask, true)]
    [InlineData(Figure.Fire | Figure.PlayerMask, true)]
    [InlineData(Figure.Wall | Figure.PlayerMask, false)]
    public void IsWalkable_WithPlayerMask(Figure figure, bool expected)
    {
        var actual = figure.IsWalkable();
        Assert.Equal(expected, actual);
    }
    
    [Theory]
        [InlineData(Figure.Knight | Figure.IsWhite, Figure.Knight | Figure.IsBlack, true)]
        [InlineData(Figure.Knight | Figure.IsBlack, Figure.Knight | Figure.IsWhite, true)]
        [InlineData(Figure.Knight | Figure.IsWhite, Figure.Knight | Figure.IsWhite, false)]
        [InlineData(Figure.Knight | Figure.IsBlack, Figure.Knight | Figure.IsBlack, false)]
        [InlineData(Figure.Knight | Figure.IsWhite, Figure.Empty, false)]
        [InlineData(Figure.Knight | Figure.IsBlack, Figure.Empty, false)]
        [InlineData(Figure.Knight | Figure.IsWhite, Figure.Wall, false)]
        [InlineData(Figure.Knight | Figure.IsBlack, Figure.Wall, false)]
        [InlineData(Figure.Knight | Figure.IsWhite, Figure.Fire, false)]
        [InlineData(Figure.Knight | Figure.IsBlack, Figure.Fire, false)]
        [InlineData(Figure.Knight | Figure.IsWhite, Figure.Trench, true)]
        [InlineData(Figure.Knight | Figure.IsBlack, Figure.Trench, true)]
        [InlineData(Figure.Knight | Figure.IsWhite, Figure.Explosives, true)]
        [InlineData(Figure.Knight | Figure.IsBlack, Figure.Explosives, true)]
        public void CanAttack_ReturnsExpected_ForVariousTargets(Figure attacker, Figure target, bool expected)
        {
            Assert.Equal(expected, attacker.CanAttack(target));
        }

        [Theory]
        [InlineData(Figure.Ninja | Figure.IsWhite, Figure.Priest | Figure.IsBlack, true)]
        [InlineData(Figure.Ninja | Figure.IsBlack, Figure.Priest | Figure.IsWhite, true)]
        [InlineData(Figure.Ninja | Figure.IsWhite, Figure.Priest | Figure.IsWhite, false)]
        [InlineData(Figure.Ninja | Figure.IsBlack, Figure.Priest | Figure.IsBlack, false)]
        public void CanAttack_OpponentPieces_AreAttackable(Figure attacker, Figure target, bool expected)
        {
            Assert.Equal(expected, attacker.CanAttack(target));
        }

        [Theory]
        [InlineData(Figure.Queen | Figure.IsWhite, Figure.LastNonAttackableFigure, false)]
        [InlineData(Figure.Queen | Figure.IsBlack, Figure.LastNonAttackableFigure, false)]
        public void CanAttack_LastNonAttackableFigure_IsNotAttackable(Figure attacker, Figure target, bool expected)
        {
            Assert.Equal(expected, attacker.CanAttack(target));
        }

        [Theory]
        [InlineData(Figure.Mage | Figure.IsWhite, (Figure)0, false)]
        [InlineData(Figure.Mage | Figure.IsBlack, (Figure)0, false)]
        public void CanAttack_Empty_IsNotAttackable(Figure attacker, Figure target, bool expected)
        {
            Assert.Equal(expected, attacker.CanAttack(target));
        }
        
        [Theory]
        [InlineData(Figure.Knight | Figure.IsBlack, Figure.Queen | Figure.IsBlack, true)]
        [InlineData(Figure.Knight | Figure.IsWhite, Figure.Queen | Figure.IsWhite, true)]
        [InlineData(Figure.Knight | Figure.IsBlack, Figure.Queen | Figure.IsWhite, false)]
        [InlineData(Figure.Knight | Figure.IsWhite, Figure.Queen | Figure.IsBlack, false)]
        [InlineData(Figure.Knight | Figure.IsBlack, Figure.Empty, false)]
        [InlineData(Figure.Empty, Figure.Queen | Figure.IsWhite, false)]
        [InlineData(Figure.Empty, Figure.Empty, true)]
        public void IsAllyTo_ReturnsExpected(Figure a, Figure b, bool expected)
        {
            Assert.Equal(expected, a.IsAllyTo(b));
        }

        [Theory]
        [InlineData(Figure.Trader | Figure.IsBlack, Figure.Miner | Figure.IsBlack, true)]
        [InlineData(Figure.Trader | Figure.IsWhite, Figure.Miner | Figure.IsWhite, true)]
        [InlineData(Figure.Trader | Figure.IsBlack, Figure.Miner | Figure.IsWhite, false)]
        public void IsAllyTo_WithDifferentFiguresSamePlayers_ReturnsTrue_And_CrossPlayers_ReturnsFalse(Figure a, Figure b, bool expected)
        {
            Assert.Equal(expected, a.IsAllyTo(b));
        }
        
        // White piece vs Black piece => enemy
    [Theory]
    [InlineData(Figure.Knight | Figure.IsWhite, Figure.Archer | Figure.IsBlack)]
    [InlineData(Figure.Bard | Figure.IsWhite, Figure.Ranger | Figure.IsBlack)]
    [InlineData(Figure.Musketeer | Figure.IsWhite, Figure.Crossbow | Figure.IsBlack)]
    [InlineData(Figure.King | Figure.IsWhite | Figure.IsKing, Figure.Queen | Figure.IsBlack)]
    public void ReturnsTrue_ForWhiteVsBlack(Figure a, Figure b)
    {
        Assert.True(a.IsEnemyTo(b));
    }

    // Black piece vs White piece => enemy
    [Theory]
    [InlineData(Figure.Ninja | Figure.IsBlack, Figure.Priest | Figure.IsWhite)]
    [InlineData(Figure.Samurai | Figure.IsBlack, Figure.Dragon | Figure.IsWhite)]
    [InlineData(Figure.Elephant | Figure.IsBlack, Figure.BattleAxe | Figure.IsWhite)]
    [InlineData(Figure.Mage | Figure.IsBlack, Figure.Blade | Figure.IsWhite)]
    public void ReturnsTrue_ForBlackVsWhite(Figure a, Figure b)
    {
        Assert.True(a.IsEnemyTo(b));
    }

    // Same color (white) => not enemy
    [Theory]
    [InlineData(Figure.Scout | Figure.IsWhite, Figure.Trader | Figure.IsWhite)]
    [InlineData(Figure.Spearman | Figure.IsWhite, Figure.Pikeman | Figure.IsWhite)]
    [InlineData(Figure.Archer | Figure.IsWhite, Figure.Crossbow | Figure.IsWhite)]
    [InlineData(Figure.Queen | Figure.IsWhite, Figure.King | Figure.IsWhite | Figure.IsKing)]
    public void ReturnsFalse_ForSameColorWhite(Figure a, Figure b)
    {
        Assert.False(a.IsEnemyTo(b));
    }

    // Same color (black) => not enemy
    [Theory]
    [InlineData(Figure.Knight | Figure.IsBlack, Figure.CamelArcher | Figure.IsBlack)]
    [InlineData(Figure.LegionarySword | Figure.IsBlack, Figure.LegionaryPike | Figure.IsBlack)]
    [InlineData(Figure.MountedArcher | Figure.IsBlack, Figure.MountedKnight | Figure.IsBlack)]
    [InlineData(Figure.Cannon | Figure.IsBlack, Figure.Catapult | Figure.IsBlack)]
    public void ReturnsFalse_ForSameColorBlack(Figure a, Figure b)
    {
        Assert.False(a.IsEnemyTo(b));
    }

    // Neutral/empty vs colored => not enemy (no opposing players)
    [Theory]
    [InlineData(Figure.Empty, Figure.Knight | Figure.IsWhite)]
    [InlineData(Figure.Fire, Figure.Archer | Figure.IsBlack)]
    [InlineData(Figure.Wall, Figure.Samurai | Figure.IsWhite)]
    [InlineData(Figure.Trench, Figure.Ninja | Figure.IsBlack)]
    [InlineData(Figure.Explosives, Figure.Miner | Figure.IsWhite)]
    public void ReturnsFalse_ForNeutralOrEmptyAgainstColored(Figure a, Figure b)
    {
        Assert.False(a.IsEnemyTo(b));
    }

    // Colored vs neutral/empty => not enemy
    [Theory]
    [InlineData(Figure.Knight | Figure.IsWhite, Figure.Empty)]
    [InlineData(Figure.Archer | Figure.IsBlack, Figure.Fire)]
    [InlineData(Figure.Samurai | Figure.IsWhite, Figure.Wall)]
    [InlineData(Figure.Ninja | Figure.IsBlack, Figure.Trench)]
    [InlineData(Figure.Miner | Figure.IsWhite, Figure.Explosives)]
    public void ReturnsFalse_ForColoredAgainstNeutralOrEmpty(Figure a, Figure b)
    {
        Assert.False(a.IsEnemyTo(b));
    }

    // Neutral/empty vs neutral/empty => not enemy
    [Theory]
    [InlineData(Figure.Empty, Figure.Empty)]
    [InlineData(Figure.Fire, Figure.Wall)]
    [InlineData(Figure.Trench, Figure.Explosives)]
    public void ReturnsFalse_ForNeutralOrEmptyPairs(Figure a, Figure b)
    {
        Assert.False(a.IsEnemyTo(b));
    }
}

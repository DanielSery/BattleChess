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
    [InlineData(Figure.Knight | Figure.IsWhite, Figure.Queen | Figure.IsWhite)]
    [InlineData(Figure.MountedArcher | Figure.IsBlack, Figure.CamelArcher | Figure.IsBlack)] 
    [InlineData(Figure.Wall, Figure.Trench)]
    [InlineData(Figure.Empty, Figure.Empty)]
    public void IsSameColor_ReturnsTrue(Figure a, Figure b)
    {
        // Act
        var result = a.IsSameColor(b);
    
        // Assert
        result.Should().BeTrue();
    }
    
    [Theory]
    [InlineData(Figure.Knight | Figure.IsWhite, Figure.Knight | Figure.IsBlack)]
    [InlineData(Figure.Knight | Figure.IsWhite, Figure.CamelArcher | Figure.IsBlack)]
    [InlineData(Figure.Wall, Figure.Knight | Figure.IsWhite)]
    [InlineData(Figure.Knight | Figure.IsBlack, Figure.Trench)]
    [InlineData(Figure.Empty, Figure.Knight | Figure.IsWhite)]
    [InlineData(Figure.Knight | Figure.IsBlack, Figure.Empty)]
    public void IsSameColor_ReturnsFalse(Figure a, Figure b) 
    {
        // Act
        var result = a.IsSameColor(b);
    
        // Assert
        result.Should().BeFalse();
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
    [InlineData(Figure.Empty)]
    [InlineData(Figure.Fire)]
    [InlineData(Figure.Wall)]
    [InlineData(Figure.Trench)]
    [InlineData(Figure.Explosives)]
    public void IsNeutralFigure_ReturnsTrue(Figure input)
    {
        // Act
        var result = input.IsNeutralFigure();

        // Assert
        result.Should().BeTrue();
    }

    [Theory]
    [InlineData(Figure.Wall | Figure.IsKing)]
    [InlineData(Figure.Trench | Figure.IsWhite)]
    [InlineData(Figure.Explosives | Figure.IsBlack)]
    [InlineData(Figure.MountedKnight)]
    [InlineData(Figure.Knight)]
    [InlineData(Figure.Queen)] 
    [InlineData(Figure.Knight | Figure.IsWhite)]
    [InlineData(Figure.MountedKnight | Figure.IsBlack)]
    [InlineData(Figure.Knight | Figure.IsWhite | Figure.IsKing)]
    public void IsNeutralFigure_ReturnsFalse(Figure input)
    {
        // Act
        var result = input.IsNeutralFigure();

        // Assert 
        result.Should().BeFalse();
    }

    [Theory]
    // Neutral figures (no color)
    [InlineData(Figure.Empty)]
    [InlineData(Figure.Wall)]
    [InlineData(Figure.Trench)]
    [InlineData(Figure.Explosives)]
    // Non-neutral, non-colored figures
    [InlineData(Figure.Knight)]
    [InlineData(Figure.MountedKnight)]
    public void IsNeutral_ReturnsTrue(Figure input)
    {
        // Act
        var result = input.IsNeutral();

        // Assert
        result.Should().BeTrue();
    }

    [Theory]
    // Neutral figures with color flags
    [InlineData(Figure.Wall | Figure.IsWhite)]
    [InlineData(Figure.Trench | Figure.IsBlack)]
    [InlineData(Figure.Explosives | Figure.IsWhite)]
    // Colored figures
    [InlineData(Figure.Knight | Figure.IsWhite)]
    [InlineData(Figure.Knight | Figure.IsBlack)]
    [InlineData(Figure.MountedKnight | Figure.IsWhite)]
    // Only color flags
    [InlineData(Figure.IsWhite)]
    [InlineData(Figure.IsBlack)]
    public void IsNeutral_ReturnsFalse(Figure input)
    {
        // Act
        var result = input.IsNeutral();

        // Assert
        result.Should().BeFalse();
    }
    
    [Theory]
    [InlineData(Figure.Empty)]
    [InlineData(Figure.Fire)]
    [InlineData(Figure.Empty | Figure.IsWhite)]
    [InlineData(Figure.Fire | Figure.IsWhite)]
    [InlineData(Figure.Empty | Figure.IsBlack)]
    [InlineData(Figure.Fire | Figure.IsBlack)]
    [InlineData(Figure.Empty | Figure.IsKing)]
    [InlineData(Figure.Fire | Figure.IsKing)]
    [InlineData(Figure.Empty | Figure.PlayerMask)]
    [InlineData(Figure.Fire | Figure.PlayerMask)]
    public void IsWalkable_ReturnsTrue(Figure figure)
    {
        var actual = figure.IsWalkable();
        Assert.True(actual);
    }
    
    [Theory]
    [InlineData(Figure.Wall)]
    [InlineData(Figure.Trench)]
    [InlineData(Figure.Explosives)]
    [InlineData(Figure.MountedKnight)]
    [InlineData(Figure.King)]
    [InlineData(Figure.Bard)]
    [InlineData(Figure.Wall | Figure.IsWhite)]
    [InlineData(Figure.Wall | Figure.IsBlack)]
    [InlineData(Figure.Trench | Figure.IsWhite)]
    [InlineData(Figure.Trench | Figure.IsBlack)]
    [InlineData(Figure.Wall | Figure.IsKing)]
    [InlineData(Figure.MountedKnight | Figure.IsKing)]
    [InlineData(Figure.Wall | Figure.PlayerMask)]
    public void IsWalkable_ReturnsFalse(Figure figure)
    {
        var actual = figure.IsWalkable();
        Assert.False(actual);
    }
    
    [Theory]
    [InlineData(Figure.Knight | Figure.IsWhite, Figure.Knight | Figure.IsBlack)]
    [InlineData(Figure.Knight | Figure.IsBlack, Figure.Knight | Figure.IsWhite)]
    [InlineData(Figure.Knight | Figure.IsWhite, Figure.Trench)]
    [InlineData(Figure.Knight | Figure.IsBlack, Figure.Trench)]
    [InlineData(Figure.Knight | Figure.IsWhite, Figure.Explosives)]
    [InlineData(Figure.Knight | Figure.IsBlack, Figure.Explosives)]
    [InlineData(Figure.Ninja | Figure.IsWhite, Figure.Priest | Figure.IsBlack)]
    [InlineData(Figure.Ninja | Figure.IsBlack, Figure.Priest | Figure.IsWhite)]
    public void CanAttack_ReturnsTrue(Figure attacker, Figure target)
    {
        attacker.CanAttack(target).Should().BeTrue();
    }

    [Theory]
    [InlineData(Figure.Knight | Figure.IsWhite, Figure.Knight | Figure.IsWhite)]
    [InlineData(Figure.Knight | Figure.IsBlack, Figure.Knight | Figure.IsBlack)]
    [InlineData(Figure.Knight | Figure.IsWhite, Figure.Empty)]
    [InlineData(Figure.Knight | Figure.IsBlack, Figure.Empty)]
    [InlineData(Figure.Knight | Figure.IsWhite, Figure.Wall)]
    [InlineData(Figure.Knight | Figure.IsBlack, Figure.Wall)]
    [InlineData(Figure.Knight | Figure.IsWhite, Figure.Fire)]
    [InlineData(Figure.Knight | Figure.IsBlack, Figure.Fire)]
    [InlineData(Figure.Ninja | Figure.IsWhite, Figure.Priest | Figure.IsWhite)]
    [InlineData(Figure.Ninja | Figure.IsBlack, Figure.Priest | Figure.IsBlack)]
    [InlineData(Figure.Queen | Figure.IsWhite, Figure.LastNonAttackableFigure)]
    [InlineData(Figure.Queen | Figure.IsBlack, Figure.LastNonAttackableFigure)]
    [InlineData(Figure.Mage | Figure.IsWhite, (Figure)0)]
    [InlineData(Figure.Mage | Figure.IsBlack, (Figure)0)]
    [InlineData(Figure.Knight | Figure.IsBlack, Figure.Queen | Figure.IsBlack)]
    [InlineData(Figure.Knight | Figure.IsWhite, Figure.Queen | Figure.IsWhite)]
    public void CanAttack_ReturnsFalse(Figure attacker, Figure target)
    {
        attacker.CanAttack(target).Should().BeFalse();
    }
    [Theory]
    [InlineData(Figure.Trader | Figure.IsBlack, Figure.Miner | Figure.IsBlack)]
    [InlineData(Figure.Trader | Figure.IsWhite, Figure.Miner | Figure.IsWhite)]
    [InlineData(Figure.Empty, Figure.Empty)]
    public void IsAllyTo_ReturnsTrue(Figure a, Figure b)
    {
        a.IsAllyTo(b).Should().BeTrue();
    }
        
    [Theory]
    [InlineData(Figure.Knight | Figure.IsBlack, Figure.Queen | Figure.IsWhite)]
    [InlineData(Figure.Knight | Figure.IsWhite, Figure.Queen | Figure.IsBlack)]
    [InlineData(Figure.Knight | Figure.IsBlack, Figure.Empty)]
    [InlineData(Figure.Empty, Figure.Queen | Figure.IsWhite)]
    [InlineData(Figure.Trader | Figure.IsBlack, Figure.Miner | Figure.IsWhite)]
    public void IsAllyTo_ReturnsFalse(Figure a, Figure b)
    {
        a.IsAllyTo(b).Should().BeFalse();
    }
        
    // White piece vs Black piece => enemy
    [Theory]
    [InlineData(Figure.Knight | Figure.IsWhite, Figure.Archer | Figure.IsBlack)]
    [InlineData(Figure.Bard | Figure.IsWhite, Figure.Ranger | Figure.IsBlack)]
    [InlineData(Figure.Musketeer | Figure.IsWhite, Figure.Crossbow | Figure.IsBlack)]
    [InlineData(Figure.King | Figure.IsWhite | Figure.IsKing, Figure.Queen | Figure.IsBlack)]
    [InlineData(Figure.Ninja | Figure.IsBlack, Figure.Priest | Figure.IsWhite)]
    [InlineData(Figure.Samurai | Figure.IsBlack, Figure.Dragon | Figure.IsWhite)]
    [InlineData(Figure.Elephant | Figure.IsBlack, Figure.BattleAxe | Figure.IsWhite)]
    [InlineData(Figure.Mage | Figure.IsBlack, Figure.Blade | Figure.IsWhite)]
    public void IsEnemyTo_ReturnsTrue(Figure a, Figure b)
    {
        Assert.True(a.IsEnemyTo(b));
    }

    [Theory]
    [InlineData(Figure.Scout | Figure.IsWhite, Figure.Trader | Figure.IsWhite)]
    [InlineData(Figure.Spearman | Figure.IsWhite, Figure.Pikeman | Figure.IsWhite)]
    [InlineData(Figure.Archer | Figure.IsWhite, Figure.Crossbow | Figure.IsWhite)]
    [InlineData(Figure.Queen | Figure.IsWhite, Figure.King | Figure.IsWhite | Figure.IsKing)]
    [InlineData(Figure.Knight | Figure.IsBlack, Figure.CamelArcher | Figure.IsBlack)]
    [InlineData(Figure.LegionarySword | Figure.IsBlack, Figure.LegionaryPike | Figure.IsBlack)]
    [InlineData(Figure.MountedArcher | Figure.IsBlack, Figure.MountedKnight | Figure.IsBlack)]
    [InlineData(Figure.Cannon | Figure.IsBlack, Figure.Catapult | Figure.IsBlack)]
    [InlineData(Figure.Empty, Figure.Knight | Figure.IsWhite)]
    [InlineData(Figure.Fire, Figure.Archer | Figure.IsBlack)]
    [InlineData(Figure.Wall, Figure.Samurai | Figure.IsWhite)]
    [InlineData(Figure.Trench, Figure.Ninja | Figure.IsBlack)]
    [InlineData(Figure.Explosives, Figure.Miner | Figure.IsWhite)]
    [InlineData(Figure.Knight | Figure.IsWhite, Figure.Empty)]
    [InlineData(Figure.Archer | Figure.IsBlack, Figure.Fire)]
    [InlineData(Figure.Samurai | Figure.IsWhite, Figure.Wall)]
    [InlineData(Figure.Ninja | Figure.IsBlack, Figure.Trench)]
    [InlineData(Figure.Miner | Figure.IsWhite, Figure.Explosives)]
    [InlineData(Figure.Empty, Figure.Empty)]
    [InlineData(Figure.Fire, Figure.Wall)]
    [InlineData(Figure.Trench, Figure.Explosives)]
    public void IsEnemyTo_ReturnsFalse(Figure a, Figure b)
    {
        Assert.False(a.IsEnemyTo(b));
    }
}
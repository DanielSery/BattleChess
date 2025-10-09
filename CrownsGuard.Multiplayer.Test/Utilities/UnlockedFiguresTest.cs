using CrownsGuard.Multiplayer.Utilities;
using JetBrains.Annotations;

namespace CrownsGuard.Multiplayer.Test.Utilities;

[TestSubject(typeof(UnlockedFigures))]
public class UnlockedFiguresTest
{
    [Fact]
    public Task DefaultUnlockedFigures_Verify()
    {
        return Verify(UnlockedFigures.DefaultUnlockedFigures);
    }
}
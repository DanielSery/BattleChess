using CrownsGuard.Core.Board;

namespace CrownsGuard.Core.Test.Board;

public class SampleSetupTest
{
    [Fact]
    public Task VerifyChessSetup()
    {
        return Verify(SampleSetup.ChessSetup);
    }
}
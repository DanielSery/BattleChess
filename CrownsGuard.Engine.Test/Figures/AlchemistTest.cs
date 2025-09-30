using CrownsGuard.Core.Figures;
using CrownsGuard.Engine.Figures;
using CrownsGuard.Engine.Helpers;
using CrownsGuard.Engine.Test.Helpers;

namespace CrownsGuard.Engine.Test.Figures;

public class AlchemistTest
{
    [Fact]
    public Task GetPossibleActions_Verify()
    {
        const int src = 27; // somewhere in the middle (d4)
        const Figure alchemist = Figure.Alchemist | Figure.IsWhite;

        return Verify(new List<object>
        {
            TestUtils.RunGetActionsScenario(
                "All empty",
                _ => { /* default empty neighbors */ }, src, alchemist, Alchemist.GetPossibleActions),
            
            TestUtils.RunGetActionsScenario(
                "Walls everywhere", b =>
                {
                    foreach (var rel in PositionConstants.QueenDirections)
                    {
                        var idx = src.GetWithOffset(rel);
                        if (idx != -1) b[idx] = Figure.Wall;
                    }
                }, src, alchemist, Alchemist.GetPossibleActions),
            
            TestUtils.RunGetActionsScenario(
                "Mixed", b =>
                {
                    var left = src.GetWithOffset(PositionConstants.L1);
                    var up = src.GetWithOffset(PositionConstants.U1);
                    var right = src.GetWithOffset(PositionConstants.R1);
                    var downLeft = src.GetWithOffset(PositionConstants.D1L1);
                    var down = src.GetWithOffset(PositionConstants.D1);

                    if (left != -1) b[left] = Figure.Empty;        // walkable
                    if (up != -1) b[up] = Figure.Fire;             // walkable
                    if (right != -1) b[right] = Figure.Explosives; // allowed
                    if (downLeft != -1) b[downLeft] = Figure.Wall;     // not allowed
                    if (down != -1) b[down] = Figure.Trench;       // not allowed
                }, src, alchemist, Alchemist.GetPossibleActions)
        });
    }

    [Fact]
    public Task ExecuteMove_Verify()
    {
        const int src = 27; // d4
        const Figure alchemist = Figure.Alchemist | Figure.IsWhite;

        return Verify(new List<object>
        {
            TestUtils.RunExecuteActionScenario(
                "To empty", _ => { }, 
                src, alchemist,
                PositionConstants.L1, FigureActionType.AlchemistMove),
            
            TestUtils.RunExecuteActionScenario(
                "To fire", b =>
            {
                var up = src.GetWithOffset(PositionConstants.U1);
                if (up != -1) b[up] = Figure.Fire;
            }, src, alchemist, PositionConstants.U1, FigureActionType.AlchemistMove),
            
            TestUtils.RunExecuteActionScenario(
                "To explosives", b =>
                {
                    var right = src.GetWithOffset(PositionConstants.R1);
                    if (right != -1) b[right] = Figure.Explosives;
                }, src, alchemist, PositionConstants.R1, FigureActionType.AlchemistMove)
        });
    }
}
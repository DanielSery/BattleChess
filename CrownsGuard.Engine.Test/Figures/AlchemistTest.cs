using System;
using System.Collections.Generic;
using System.Linq;
using CrownsGuard.Core.Board;
using CrownsGuard.Core.Figures;
using CrownsGuard.Core.Helpers;
using CrownsGuard.Engine.Figures;
using CrownsGuard.Engine.Helpers;
using CrownsGuard.Engine.Test.Helpers;

namespace CrownsGuard.Engine.Test.Figures;

public class AlchemistTest
{
    [Fact]
    public Task GetPossibleActions_VariousBoards_Verify()
    {
        const int src = 27; // somewhere in the middle (d4)
        const Figure alchemist = Figure.Alchemist | Figure.IsWhite;

        return Verify(new List<object>
        {
            TestUtils.RunGetActionsScenario("AllEmpty", _ => { /* default empty neighbors */ }, src, alchemist, Alchemist.GetPossibleActions),
            TestUtils.RunGetActionsScenario("WallsEverywhere", b =>
            {
                foreach (var rel in PositionConstants.QueenDirections)
                {
                    var idx = src.GetWithOffset(rel);
                    if (idx != -1) b[idx] = Figure.Wall;
                }
            }, src, alchemist, Alchemist.GetPossibleActions),
            TestUtils.RunGetActionsScenario("Mixed", b =>
            {
                var left = src.GetWithOffset(PositionConstants.L);
                var up = src.GetWithOffset(PositionConstants.U);
                var right = src.GetWithOffset(PositionConstants.R);
                var downLeft = src.GetWithOffset(PositionConstants.DL);
                var down = src.GetWithOffset(PositionConstants.D);

                if (left != -1) b[left] = Figure.Empty;        // walkable
                if (up != -1) b[up] = Figure.Fire;             // walkable
                if (right != -1) b[right] = Figure.Explosives; // allowed
                if (downLeft != -1) b[downLeft] = Figure.Wall;     // not allowed
                if (down != -1) b[down] = Figure.Trench;       // not allowed
            }, src, alchemist, Alchemist.GetPossibleActions)
        });
    }

    [Fact]
    public Task ExecuteMove_VariousBoards_Verify()
    {
        const int src = 27; // d4
        const Figure alchemist = Figure.Alchemist | Figure.IsWhite;

        return Verify(new List<object>
        {
            TestUtils.RunExecuteActionScenario("ToEmpty_Left", _ => { }, src, alchemist, 
                PositionConstants.QueenDirections[1], FigureActionType.AlchemistMove),
            TestUtils.RunExecuteActionScenario("ToFire_Up", b =>
            {
                var up = src.GetWithOffset(PositionConstants.U);
                if (up != -1) b[up] = Figure.Fire;
            }, src, alchemist, PositionConstants.U, FigureActionType.AlchemistMove),
            TestUtils.RunExecuteActionScenario("ToExplosives_Right", b =>
            {
                var right = src.GetWithOffset(PositionConstants.R);
                if (right != -1) b[right] = Figure.Explosives;
            }, src, alchemist, PositionConstants.R, FigureActionType.AlchemistMove)
        });
    }
}
// Copyright (c) Veeam Software Group GmbH

using System.Collections;
using CrownsGuard.Core.Figures;

namespace CrownsGuard.Multiplayer.Utilities;

public static class UnlockedFigures
{
    public static byte[] DefaultUnlockedFigures { get; }

    static UnlockedFigures()
    {
        DefaultUnlockedFigures = new byte[32];
        var bitArray = new BitArray(DefaultUnlockedFigures)
        {
            [(int)FigureId.Empty] = true,
            [(int)FigureId.Fire] = true,
            [(int)FigureId.Trench] = true,
            [(int)FigureId.Wall] = true,
            [(int)FigureId.Explosives] = true,
            [(int)FigureId.LegionarySword] = true,
            [(int)FigureId.CamelRider] = true,
            [(int)FigureId.Whiplash] = true,
            [(int)FigureId.MountedKnight] = true,
            [(int)FigureId.Queen] = true,
            [(int)FigureId.King] = true,
        };
        bitArray.CopyTo(DefaultUnlockedFigures, 0);
    }
}
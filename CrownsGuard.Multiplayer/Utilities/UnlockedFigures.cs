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
            [(int)Figure.Empty] = true,
            [(int)Figure.Fire] = true,
            [(int)Figure.Trench] = true,
            [(int)Figure.Wall] = true,
            [(int)Figure.Explosives] = true,
            [(int)Figure.LegionarySword] = true,
            [(int)Figure.CamelRider] = true,
            [(int)Figure.Whiplash] = true,
            [(int)Figure.MountedKnight] = true,
            [(int)Figure.Queen] = true,
            [(int)Figure.King] = true,
        };
        bitArray.CopyTo(DefaultUnlockedFigures, 0);
    }
}
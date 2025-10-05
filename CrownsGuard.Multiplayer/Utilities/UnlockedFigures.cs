using System.Collections;
using CrownsGuard.Core.Figures;

namespace CrownsGuard.Multiplayer.Utilities;

public static class UnlockedFigures
{
    public static IReadOnlyList<byte> DefaultUnlockedFigures { get; }

    static UnlockedFigures()
    {
        var result = new byte[32];
        var bitArray = new BitArray(result)
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
        bitArray.CopyTo(result, 0);
        DefaultUnlockedFigures = result;
    }
}
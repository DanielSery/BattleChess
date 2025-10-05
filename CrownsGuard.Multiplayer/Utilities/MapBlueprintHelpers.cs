using CrownsGuard.Core.Figures;

namespace CrownsGuard.Multiplayer.Utilities;

public static class MapBlueprintHelpers
{
    public static int[] GetIntData(this Figure[] map)
    {
        return map
            .Select(x => (int)x)
            .ToArray();
    }
}
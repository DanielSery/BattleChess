using System.Reflection;

namespace BattleChess3.Multiplayer.Utilities;

public static class GameVersion
{
    static GameVersion()
    {
        var version = Assembly.GetExecutingAssembly().GetName().Version;
        VersionId = version is not null
            ? ((byte)version.Major) << 16 | (byte)version.Minor << 8 | (byte)version.Revision
            : -1;
    }

    public static int VersionId { get; }
}
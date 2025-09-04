namespace BattleChess3.Maps.IO;

public class DirectoryHandler : IDirectoryHandler
{
    /// <inheritdoc />
    public bool Exists(string? path)
    {
        return Directory.Exists(path);
    }

    /// <inheritdoc />
    public DirectoryInfo CreateDirectory(string path)
    {
        return new DirectoryInfo(path);
    }

    /// <inheritdoc />
    public string[] GetFiles(string path, string searchPattern)
    {
        return Directory.GetFiles(path, searchPattern);
    }
}
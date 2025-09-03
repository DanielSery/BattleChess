namespace BattleChess3.Maps;

public class FileHandler : IFileHandler
{
    /// <inheritdoc />
    public string ReadAllText(string path)
    {
        return File.ReadAllText(path);
    }

    /// <inheritdoc />
    public void WriteAllText(string path, string? contents)
    {
        File.WriteAllText(path, contents);
    }

    /// <inheritdoc />
    public bool Exists(string? path)
    {
        return File.Exists(path);
    }
}
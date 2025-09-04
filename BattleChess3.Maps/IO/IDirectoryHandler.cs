namespace BattleChess3.Maps.IO;

public interface IDirectoryHandler
{
    bool Exists(string? path);

    DirectoryInfo CreateDirectory(string path);

    string[] GetFiles(string path, string searchPattern);
}
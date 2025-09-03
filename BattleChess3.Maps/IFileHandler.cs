namespace BattleChess3.Maps;

public interface IFileHandler
{
    string ReadAllText(string path);
    void WriteAllText(string path, string? contents);
    bool Exists(string? path);
}
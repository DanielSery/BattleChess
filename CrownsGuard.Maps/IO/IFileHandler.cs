namespace CrownsGuard.Maps.IO;

public interface IFileHandler
{
    string ReadAllText(string path);
    void WriteAllText(string path, string? contents);
    bool Exists(string? path);
    void Delete(string path);
}
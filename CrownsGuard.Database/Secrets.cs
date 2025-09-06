namespace CrownsGuard.Database;

/// <summary>
/// For working secrets create second partial Secrets class with setting of properties in ctor.
/// </summary>
internal static partial class Secrets
{
    public static string ConnectionString { get; }
}
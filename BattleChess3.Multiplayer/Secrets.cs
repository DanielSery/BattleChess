namespace BattleChess3.Multiplayer;

/// <summary>
/// For working secrets create second partial Secrets class with setting of properties in ctor.
/// </summary>
public static partial class Secrets
{
    public static string ConnectionString { get; }
    public static string EmailSalt { get; }
    public static string GmailString { get; }
}
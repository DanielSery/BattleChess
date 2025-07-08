namespace BattleChess3.Multiplayer;

[Flags]
public enum MultiplayerGameType
{
    None = 0,
    Host = 1,
    Ranked = 2,
    Lobby = 4,
}
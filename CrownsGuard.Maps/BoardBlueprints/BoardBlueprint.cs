using CrownsGuard.Core.Figures;
using CrownsGuard.Game.Players;

namespace CrownsGuard.Maps.BoardBlueprints;

// JSON serializable
public class BoardBlueprint
{
    public PlayerColor StartingPlayerColor { get; init; } = PlayerColor.White;
    public Figure[] Figures { get; init; } = [];
}
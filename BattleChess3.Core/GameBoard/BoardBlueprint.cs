using BattleChess3.Core.Figures;
using BattleChess3.Core.Players;

namespace BattleChess3.Core.GameBoard;

// JSON serializable
public class BoardBlueprint
{
    public static readonly BoardBlueprint ChessTeam = new()
    {
        Figures = [
            new(Player.White, 22, false),
            new(Player.White, 22, false),
            new(Player.White, 22, false),
            new(Player.White, 22, false),
            new(Player.White, 22, false),
            new(Player.White, 22, false),
            new(Player.White, 22, false),
            new(Player.White, 22, false),
            
            new(Player.White, 16, false),
            new(Player.White, 25, false),
            new(Player.White, 15, false),
            new(Player.White, 23, true),
            new(Player.White, 24, false),
            new(Player.White, 15, false),
            new(Player.White, 25, false),
            new(Player.White, 16, false)
        ],
        StartingPlayer = Player.White
    };
    
    public Player StartingPlayer { get; init; } = Player.White;
    public FigureBlueprint[] Figures { get; init; } = [];
}
using BattleChess3.Game.Board;
using BattleChess3.Game.Players;
using FluentResults;

namespace BattleChess3.Multiplayer;

public interface IMultiplayerGameService
{
    public event EventHandler<(Position, Position)>? RequestPlayMove;

    public void StartGame(MultiplayerGameType gameType, string? rankedGameId);
    public Task<Result> HandleHisTurnAsync();
    public Task<Result<string?>> HandleWinAsync(Player won, Player lost);
    public Task<Result> PlayedMoveAsync(Position from, Position to, TimeSpan timeSpent);
    public Task<Result> DeleteGameAsync(string gameId);
}
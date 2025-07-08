using BattleChess3.Game.Board;
using FluentResults;

namespace BattleChess3.Multiplayer;

public interface IMultiplayerGameService
{
    public event EventHandler<(Position, Position)>? RequestPlayMove;

    public void StartGame(MultiplayerGameType gameType, string? rankedGameId);
    public Task<Result> HandleHisTurnAsync();
    public Task<Result> HandleWinAsync();
    public Task<Result> PlayedMoveAsync(Position from, Position to);
    public Task<Result> DeleteGameAsync(string gameId);
}
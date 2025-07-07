using BattleChess3.Game.Board;
using FluentResults;

namespace BattleChess3.Multiplayer;

public interface IMultiplayerGameService
{
    public event EventHandler<(Position, Position)>? RequestPlayMove;

    public string? CurrentGameId { get; }
    public void StartGame(string? gameId);
    public Task HandleHisTurn(string? turnId = null);
    public Task<Result> PlayedMove(Position from, Position to);
    public Task<Result> DeleteGame(string gameId);
}
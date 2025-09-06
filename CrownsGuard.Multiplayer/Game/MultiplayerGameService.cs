using CrownsGuard.Core.GameBoard;
using CrownsGuard.Core.Players;
using CrownsGuard.Database.Database;
using CrownsGuard.Database.Game;
using CrownsGuard.Database.Players;
using CrownsGuard.Game;
using CrownsGuard.Game.Helpers;
using CrownsGuard.Multiplayer.Players;
using CrownsGuard.Multiplayer.Scheduling;
using CrownsGuard.Multiplayer.Utilities;
using FluentResults;

namespace CrownsGuard.Multiplayer.Game;

internal sealed class MultiplayerGameService : IMultiplayerGameService
{
    private readonly IMultiplayerScheduler _scheduler;

    private readonly IMultiplayerPlayerService _playerService;
    private readonly IPlayersCollectionHandler _playersCollectionHandler;
    private readonly IGameTurnsCollectionHandler _gameTurnsCollectionHandler;
    private readonly IDatabaseTimeProvider _databaseTimeProvider;

    public MultiplayerGameService(
        IMultiplayerScheduler scheduler,
        IMultiplayerPlayerService playerService,
        IPlayersCollectionHandler playersCollectionHandler,
        IGameTurnsCollectionHandler gameTurnsCollectionHandler,
        IDatabaseTimeProvider databaseTimeProvider)
    {
        _scheduler = scheduler;
        _playerService = playerService;
        _playersCollectionHandler = playersCollectionHandler;
        _gameTurnsCollectionHandler = gameTurnsCollectionHandler;
        _databaseTimeProvider = databaseTimeProvider;
    }

    public event EventHandler<(Position, Position, TimeSpan)>? RequestPlayMove;

    private MultiplayerGameType GameType { get; set; }
    private string? GameId { get; set; }
    private string? TurnId { get; set; }

    public void StartGame(MultiplayerGameType gameType, string? rankedGameId)
    {
        GameType = gameType;
        GameId = rankedGameId;
        TurnId = null;
    }

    public Task<Result<string?>> HandleWinAsync(
        bool notifyOther, WinType winType, IOnlinePlayerInfo won, IOnlinePlayerInfo lost, CancellationToken cancellationToken)
    {
        lock (_scheduler.SyncLock)
        {
            if (GameId is null)
                return Task.FromResult(Result.Ok<string?>(null));

            return _scheduler.QueueTask(async () =>
            {
                try
                {
                    if (GameId is null)
                    {
                        return Result.Ok<string?>(null);
                    }

                    await DeleteGameTurnsAsync(cancellationToken);
                    if (notifyOther)
                    {
                        await SendGameResultToOpponent(winType, cancellationToken);
                    }

                    if (!GameType.HasFlag(MultiplayerGameType.Ranked))
                    {
                        return Result.Ok<string?>(null);
                    }

                    if (notifyOther)
                    {
                        return await UpdatePlayersElo(won, lost, cancellationToken);
                    }
                    else
                    {
                        var updated = _playerService.LoggedInPlayer!.Id == won.PlayerId ? won : lost;
                        return await GetUpdatedElo(updated, cancellationToken);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex}");
                    return Result.Fail(ex.Message);
                }
            });
        }
    }

    private async Task SendGameResultToOpponent(WinType winType, CancellationToken cancellationToken)
    {
        if (GameId == null)
            return;

        if (winType == WinType.CapturedKing)
            return;

        Console.WriteLine("Sending game result to the other player");
        var messageIndex = winType switch
        {
            WinType.Surrender => IMultiplayerGameService.SurrenderMessage,
            WinType.NotResponding => IMultiplayerGameService.NotRespondingLostMessage,
            WinType.OutOfTime => IMultiplayerGameService.OutOfTimeMessage,
            _ => throw new ArgumentOutOfRangeException(nameof(winType), winType, null)
        };

        try
        {
            Console.WriteLine("Creating game result");
            var gameTurn = new GameTurn
            {
                GameId = GameId,
                FromIndex = (byte)messageIndex,
                ToIndex = 0,
                CreatedAt = DateTime.UtcNow,
                TimeSpentInSeconds = 0
            };
            await _gameTurnsCollectionHandler.InsertAsync(gameTurn, cancellationToken);
            Console.WriteLine("Created game result");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    private async Task<Result<string?>> GetUpdatedElo(IOnlinePlayerInfo lost, CancellationToken cancellationToken)
    {
        if (lost.PlayerId is null) throw new ArgumentNullException(nameof(lost));

        var updatedPlayerResult = await _playersCollectionHandler.FindByIdAsync(lost.PlayerId, cancellationToken);
        if (!updatedPlayerResult.TryGetValue(out var updatedPlayer)) return Result.Fail("Could not find losing player");
        if (updatedPlayer.Elo != lost.Elo)
        {
            _playerService.LoggedInPlayer!.Elo = updatedPlayer.Elo;
            return Result.Ok<string?>($"Elo {updatedPlayer.Elo - lost.Elo} → {updatedPlayer.Elo}");
        }

        updatedPlayerResult = await _playersCollectionHandler.WaitForEloUpdateAsync(lost.PlayerId, cancellationToken);
        if (!updatedPlayerResult.TryGetValue(out updatedPlayer)) return Result.Fail("Could not find losing player");

        _playerService.LoggedInPlayer!.Elo = updatedPlayer.Elo;
        return Result.Ok<string?>($"Elo {updatedPlayer.Elo - lost.Elo} → {updatedPlayer.Elo}");
    }

    private async Task<Result<string?>> UpdatePlayersElo(IOnlinePlayerInfo won, IOnlinePlayerInfo lost, CancellationToken cancellationToken)
    {
        if (won.PlayerId is null) throw new ArgumentNullException(nameof(won));
        if (lost.PlayerId is null) throw new ArgumentNullException(nameof(lost));

        var winningPlayerResult = await _playersCollectionHandler.FindByIdAsync(won.PlayerId, cancellationToken);
        if (!winningPlayerResult.TryGetValue(out var winningPlayer)) return Result.Fail("Could not find winning player");

        var losingPlayerResult = await _playersCollectionHandler.WaitForEloUpdateAsync(lost.PlayerId, cancellationToken);
        if (!losingPlayerResult.TryGetValue(out var losingPlayer)) return Result.Fail("Could not find losing player");

        winningPlayer.Elo = (short)won.Elo!;
        losingPlayer.Elo = (short)lost.Elo!;
        UpdateElo(winningPlayer, losingPlayer, 1d);

        var updateWinningPlayerResult = await _playersCollectionHandler.UpdateEloAsync(winningPlayer.Id, winningPlayer.Elo, cancellationToken);
        var updateLosingPlayerResult = await _playersCollectionHandler.UpdateEloAsync(losingPlayer.Id, losingPlayer.Elo, cancellationToken);

        if (!updateLosingPlayerResult.IsFailed) return updateLosingPlayerResult;
        if (!updateWinningPlayerResult.IsFailed) return updateWinningPlayerResult;

        var currentPlayer = _playerService.LoggedInPlayer!.Id == won.PlayerId ? won : lost;
        var updatedPlayer = _playerService.LoggedInPlayer!.Id == winningPlayer.Id ? winningPlayer : losingPlayer;
        _playerService.LoggedInPlayer!.Elo = updatedPlayer.Elo;
        return Result.Ok<string?>($"Elo {updatedPlayer.Elo - currentPlayer.Elo} → {updatedPlayer.Elo}");
    }

    private static void UpdateElo(RegisteredPlayer playerA, RegisteredPlayer playerB, double resultA, int k = 32)
    {
        var expectedA = 1.0 / (1.0 + Math.Pow(10, (playerB.Elo - playerA.Elo) / 400.0));
        var expectedB = 1.0 / (1.0 + Math.Pow(10, (playerA.Elo - playerB.Elo) / 400.0));

        playerA.Elo += (short)(k * (resultA - expectedA));
        playerB.Elo += (short)(k * ((1 - resultA) - expectedB));
    }

    public Task<Result> PlayedMoveAsync(Position from, Position to, TimeSpan timeSpent, CancellationToken cancellationToken)
    {
        lock (_scheduler.SyncLock)
        {
            if (GameId is null)
                return Task.FromResult(Result.Fail("Not in game"));

            return _scheduler.QueueTask(async () =>
            {
                try
                {
                    Console.WriteLine($"Creating game turn: {from} to {to}");
                    var gameTurn = new GameTurn()
                    {
                        GameId = GameId,
                        FromIndex = (byte)from.GetIndex(),
                        ToIndex = (byte)to.GetIndex(),
                        CreatedAt = DateTime.UtcNow,
                        TimeSpentInSeconds = timeSpent.TotalSeconds
                    };
                    await _gameTurnsCollectionHandler.InsertAsync(gameTurn, cancellationToken);
                    Console.WriteLine($"Created game turn: {from} to {to}");

                    TurnId = gameTurn.Id;
                    return Result.Ok();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex}");
                    return Result.Fail(ex.Message);
                }
            });
        }
    }

    public Task<Result> HandleRemotePlayerTurnAsync()
    {
        lock (_scheduler.SyncLock)
        {
            if (GameId is null)
                return Task.FromResult(Result.Fail("Not in game"));

            return _scheduler.QueueTask(async () =>
            {
                try
                {
                    Console.WriteLine($"Waiting for his turn with id greater than: {TurnId}");
                    try
                    {
                        var hisTurnResult = TurnId is null
                            ? await _gameTurnsCollectionHandler.WaitForFirstTurnAsync(GameId, IMultiplayerGameService.TurnTimeout)
                            : await _gameTurnsCollectionHandler.WaitForNextTurnAsync(TurnId, GameId, IMultiplayerGameService.TurnTimeout);

                        if (!hisTurnResult.TryGetValue(out var hisTurn))
                        {
                            RequestPlayMove?.Invoke(this,
                                (Position.FromIndex(IMultiplayerGameService.NotRespondingMessage),
                                    Position.None,
                                    TimeSpan.FromMinutes(2)));
                            return Result.Ok();
                        }

                        Console.WriteLine($"Found his turn with id: {hisTurn.Id}");

                        if (hisTurn.FromIndex >= 64)
                        {
                            RequestPlayMove?.Invoke(this, new ValueTuple<Position, Position, TimeSpan>(
                                Position.FromIndex(hisTurn.FromIndex),
                                Position.FromIndex(hisTurn.ToIndex),
                                TimeSpan.FromSeconds(hisTurn.TimeSpentInSeconds)));
                            return Result.Ok();
                        }

                        RequestPlayMove?.Invoke(this, new ValueTuple<Position, Position, TimeSpan>(
                            GetPositionOfOppositePlayer(hisTurn.FromIndex),
                            GetPositionOfOppositePlayer(hisTurn.ToIndex),
                            TimeSpan.FromSeconds(hisTurn.TimeSpentInSeconds)));
                        return Result.Ok();
                    }
                    catch (OperationCanceledException)
                    {
                        RequestPlayMove?.Invoke(this,
                            (Position.FromIndex(IMultiplayerGameService.NotRespondingMessage),
                                Position.None,
                                TimeSpan.FromMinutes(2)));
                        return Result.Ok();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex}");
                    return Result.Fail(ex.Message);
                }
            });
        }
    }

    private static Position GetPositionOfOppositePlayer(int index)
    {
        return RelativePositionHelper.GetRelative(Player.White, Position.FromIndex(index));
    }

    private async Task<Result> DeleteGameTurnsAsync(CancellationToken cancellationToken)
    {
        var serverTimeResult = await _databaseTimeProvider.GetServerTimeAsync();
        if (!serverTimeResult.TryGetValue(out var serverTime)) return Result.Fail(serverTimeResult.ToString());

        var oldestKeepTime = serverTime - TimeSpan.FromMinutes(20);
        return await _gameTurnsCollectionHandler.RemoveOlderThanAsync(oldestKeepTime, cancellationToken);
    }
}
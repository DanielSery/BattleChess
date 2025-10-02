using CrownsGuard.Core;
using CrownsGuard.Database.Database;
using CrownsGuard.Database.Game;
using CrownsGuard.Database.Players;
using CrownsGuard.Game;
using CrownsGuard.Multiplayer.Players;
using CrownsGuard.Multiplayer.Utilities;
using FluentResults;

namespace CrownsGuard.Multiplayer.Game;

internal sealed class MultiplayerGameService : IMultiplayerGameService
{
    private readonly IMultiplayerPlayerService _playerService;
    private readonly IPlayersCollectionHandler _players;
    private readonly IGameTurnsCollectionHandler _gameTurns;
    private readonly IDatabaseTimeProvider _databaseTimeProvider;

    public MultiplayerGameService(
        IMultiplayerPlayerService playerService,
        IPlayersCollectionHandler players,
        IGameTurnsCollectionHandler gameTurns,
        IDatabaseTimeProvider databaseTimeProvider)
    {
        _playerService = playerService;
        _players = players;
        _gameTurns = gameTurns;
        _databaseTimeProvider = databaseTimeProvider;
    }

    public event EventHandler<(byte, byte, TimeSpan)>? RequestPlayMove;

    private MultiplayerGameType GameType { get; set; }
    private string? GameId { get; set; }
    private string? TurnId { get; set; }

    public void StartGame(MultiplayerGameType gameType, string? rankedGameId)
    {
        GameType = gameType;
        GameId = rankedGameId;
        TurnId = null;
    }

    public async Task<Result<string?>> HandleWinAsync(
        bool notifyOther, WinType winType, IOnlinePlayerInfo won, IOnlinePlayerInfo lost, CancellationToken cancellationToken)
    {
        if (GameId is null)
            return Result.Ok<string?>(null);

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

        Console.WriteLine("Creating game result");
        var gameTurn = new GameTurn
        {
            GameId = GameId,
            FromIndex = (byte)messageIndex,
            ToIndex = 0,
            CreatedAt = DateTime.UtcNow,
            TimeSpentInSeconds = 0
        };
        await _gameTurns.InsertTurnAsync(gameTurn, cancellationToken);
        Console.WriteLine("Created game result");
    }

    private async Task<Result<string?>> GetUpdatedElo(IOnlinePlayerInfo lost, CancellationToken cancellationToken)
    {
        if (lost.PlayerId is null) throw new ArgumentNullException(nameof(lost));

        var updatedPlayerResult = await _players.FindPlayerByIdAsync(lost.PlayerId, cancellationToken);
        if (!updatedPlayerResult.TryGetValue(out var updatedPlayer)) return Result.Fail("Could not find losing player");
        if (updatedPlayer.Elo != lost.Elo)
        {
            _playerService.LoggedInPlayer!.Elo = updatedPlayer.Elo;
            return Result.Ok<string?>($"Elo {updatedPlayer.Elo - lost.Elo} → {updatedPlayer.Elo}");
        }

        updatedPlayerResult = await _players.WaitForPlayerEloUpdateAsync(lost.PlayerId, cancellationToken);
        if (!updatedPlayerResult.TryGetValue(out updatedPlayer)) return Result.Fail("Could not find losing player");

        _playerService.LoggedInPlayer!.Elo = updatedPlayer.Elo;
        return Result.Ok<string?>($"Elo {updatedPlayer.Elo - lost.Elo} → {updatedPlayer.Elo}");
    }

    private async Task<Result<string?>> UpdatePlayersElo(IOnlinePlayerInfo won, IOnlinePlayerInfo lost, CancellationToken cancellationToken)
    {
        if (won.PlayerId is null) throw new ArgumentNullException(nameof(won));
        if (lost.PlayerId is null) throw new ArgumentNullException(nameof(lost));

        var winningPlayerResult = await _players.FindPlayerByIdAsync(won.PlayerId, cancellationToken);
        if (!winningPlayerResult.TryGetValue(out var winningPlayer)) return Result.Fail("Could not find winning player");

        var losingPlayerResult = await _players.WaitForPlayerEloUpdateAsync(lost.PlayerId, cancellationToken);
        if (!losingPlayerResult.TryGetValue(out var losingPlayer)) return Result.Fail("Could not find losing player");

        winningPlayer.Elo = (short)won.Elo!;
        losingPlayer.Elo = (short)lost.Elo!;
        UpdateElo(winningPlayer, losingPlayer, 1d);

        var updateWinningPlayerResult = await _players.UpdatePlayerEloAsync(winningPlayer.Id, winningPlayer.Elo, cancellationToken);
        var updateLosingPlayerResult = await _players.UpdatePlayerEloAsync(losingPlayer.Id, losingPlayer.Elo, cancellationToken);

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

    public async Task<Result> PlayedMoveAsync(byte from, byte to, TimeSpan timeSpent, CancellationToken cancellationToken)
    {
        if (GameId is null)
            return Result.Fail("Not in game");

        var gameTurn = new GameTurn()
        {
            GameId = GameId,
            FromIndex = from,
            ToIndex = to,
            CreatedAt = DateTime.UtcNow,
            TimeSpentInSeconds = timeSpent.TotalSeconds
        };
        await _gameTurns.InsertTurnAsync(gameTurn, cancellationToken);
        TurnId = gameTurn.Id;
        return Result.Ok();
    }

    public async Task<Result> HandleRemotePlayerTurnAsync()
    {
        if (GameId is null)
            return Result.Fail("Not in game");

        Console.WriteLine($"Waiting for his turn with id greater than: {TurnId}");
        var hisTurnResult = TurnId is null
            ? await _gameTurns.WaitForFirstTurnAsync(GameId, CancellationToken.None, IMultiplayerGameService.TurnTimeoutSeconds)
            : await _gameTurns.WaitForNextTurnAsync(TurnId, GameId, CancellationToken.None, IMultiplayerGameService.TurnTimeoutSeconds);

        if (!hisTurnResult.TryGetValue(out var hisTurn))
        {
            RequestPlayMove?.Invoke(this, (IMultiplayerGameService.NotRespondingMessage, 255, TimeSpan.FromMinutes(2)));
            return Result.Ok();
        }

        Console.WriteLine($"Found his turn with id: {hisTurn.Id}");
        if (hisTurn.FromIndex >= 64)
        {
            RequestPlayMove?.Invoke(this, new ValueTuple<byte, byte, TimeSpan>(
                hisTurn.FromIndex,
                hisTurn.ToIndex,
                TimeSpan.FromSeconds(hisTurn.TimeSpentInSeconds)));
            return Result.Ok();
        }

        RequestPlayMove?.Invoke(this, new ValueTuple<byte, byte, TimeSpan>(
            GetPositionOfOppositePlayer(hisTurn.FromIndex),
            GetPositionOfOppositePlayer(hisTurn.ToIndex),
            TimeSpan.FromSeconds(hisTurn.TimeSpentInSeconds)));
        return Result.Ok();
    }

    private static byte GetPositionOfOppositePlayer(int index)
    {
        var y = index / 8;
        var x = index % 8;
        y = Constants.BoardLength - y - 1;
        return (byte)(x + y * Constants.BoardLength);
    }

    private async Task<Result> DeleteGameTurnsAsync(CancellationToken cancellationToken)
    {
        var serverTimeResult = await _databaseTimeProvider.GetServerTimeAsync();
        if (!serverTimeResult.TryGetValue(out var serverTime)) return Result.Fail(serverTimeResult.ToString());

        var oldestKeepTime = serverTime - TimeSpan.FromMinutes(20);
        return await _gameTurns.RemoveTurnsOlderThanAsync(oldestKeepTime, cancellationToken);
    }
}
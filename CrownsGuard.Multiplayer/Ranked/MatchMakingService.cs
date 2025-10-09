using CrownsGuard.Core.Figures;
using CrownsGuard.Database.Errors;
using CrownsGuard.Database.Ranked;
using CrownsGuard.Multiplayer.Utilities;
using FluentResults;
using Microsoft.Extensions.Logging;

namespace CrownsGuard.Multiplayer.Ranked;

internal class MatchMakingService : IMatchmakingService
{
    private readonly IRankedGamesCollectionHandler _gameRequests;
    private readonly IMultiplayerRankedService _multiplayerRankedService;
    private readonly ILogger<IMatchmakingService> _logger;

    public MatchMakingService(
        IMultiplayerRankedService multiplayerRankedService,
        IRankedGamesCollectionHandler gameRequests,
        ILogger<IMatchmakingService> logger)
    {
        _multiplayerRankedService = multiplayerRankedService;
        _gameRequests = gameRequests;
        _logger = logger;
    }
    
    public async Task<Result<(bool isHost, RankedGame gameSearch, RankedGameJoin gameSearchJoin)>> FindRankedGameAsync(
        string currentPlayerId, short currentPlayerElo, Figure[] currentPlayerMap, CancellationToken cancellationToken)
    {
        const int startingEloSearchDiff = 50;
        var myMapData = currentPlayerMap.GetIntData();
        
        var closeGameResult = await TryJoinClosestGameAsync(currentPlayerId, currentPlayerElo, startingEloSearchDiff, myMapData, cancellationToken);
        if (closeGameResult.IsSuccess) return closeGameResult;
        
        return await PerformGameSearchAsync(currentPlayerId, currentPlayerElo, startingEloSearchDiff, myMapData, cancellationToken);
    }

    internal async Task<Result<(bool isHost, RankedGame gameSearch, RankedGameJoin gameSearchJoin)>> PerformGameSearchAsync(
        string currentPlayerId, short currentPlayerElo, 
        int eloDifference, int[] myMapData, CancellationToken cancellationToken)
    {
        var createdGameSearchResult = await _multiplayerRankedService.CreateGameSearchAsync(currentPlayerId, currentPlayerElo, myMapData, cancellationToken);
        if (!createdGameSearchResult.TryGetValue(out var createdGameSearch)) return createdGameSearchResult.ToResult();
        
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var (waitResult, foundSearch, foundSearchJoin) = await _multiplayerRankedService.WaitForGameFindOrJoinAsync(createdGameSearch.Id, currentPlayerElo, eloDifference, 20, cancellationToken);
                switch (waitResult)
                {
                    case IMultiplayerRankedService.WaitResult.GameJoin:
                    {
                        var confirmationResult = await _gameRequests.ConfirmGameJoinAsync(createdGameSearch.Id, foundSearchJoin!.Id, cancellationToken);
                        if (confirmationResult.IsSuccess) return (true, createdGameSearch, foundSearchJoin);
                        break;
                    }
                    case IMultiplayerRankedService.WaitResult.GameSearch:
                    {
                        var joinResult = await _multiplayerRankedService.TryToJoinGameAsync(foundSearch!.Id, currentPlayerId, myMapData, cancellationToken);
                        if (joinResult.IsSuccess) return (false, foundSearch, joinResult.Value);
                        break;
                    }
                    default:
                    {
                        if (eloDifference < 300)
                        {
                            _logger.LogInformation("No game found with elo difference {eloDifference}, increasing to {nextEloDifference}", eloDifference, eloDifference + 50);
                            eloDifference += 50;
                            break;
                        }

                        _logger.LogInformation("No game found with elo difference {eloDifference}, continuing search", eloDifference);
                        break;
                    }
                }
            }

            return Result.Fail(CancelledError.Instance);
        }
        finally
        {
            if (string.IsNullOrEmpty(createdGameSearch.JoinedId))
            {
                await _multiplayerRankedService.DeleteGameSearchAsync(createdGameSearch.Id);
            }
        }
    }

    internal async Task<Result<(bool isHost, RankedGame gameSearch, RankedGameJoin gameSearchJoin)>> TryJoinClosestGameAsync(
        string playerId, short playerElo,
        int eloDifference, int[] myMapData, CancellationToken cancellationToken)
    {
        var closestGameSearchResult = await _gameRequests.GetClosestGameSearchAsync(playerElo, cancellationToken);
        if (!closestGameSearchResult.TryGetValue(out var closestGameSearch)) closestGameSearch = null;
        
        while (closestGameSearch is not null && Math.Abs(closestGameSearch.Elo - playerElo) <= eloDifference)
        {
            var joinResult = await _multiplayerRankedService.TryToJoinGameAsync(closestGameSearch.Id, playerId, myMapData, cancellationToken);
            if (joinResult.IsSuccess)
            {
                return (false, closestGameSearch, joinResult.Value);
            }

            closestGameSearchResult = await _gameRequests.GetClosestGameSearchAsync(playerElo, cancellationToken);
            if (!closestGameSearchResult.TryGetValue(out closestGameSearch)) closestGameSearch = null;
        }

        return Result.Fail("Did not find close game");
    }
}
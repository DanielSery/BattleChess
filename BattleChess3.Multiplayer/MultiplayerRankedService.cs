using System.Reflection;
using BattleChess3.Maps;
using BattleChess3.Multiplayer.Tables;
using BattleChess3.Multiplayer.Utilities;
using FluentResults;
using MongoDB.Bson;
using MongoDB.Driver;

namespace BattleChess3.Multiplayer;

internal class MultiplayerRankedService : IMultiplayerRankedService
{
    private readonly int _version;
    
    private readonly IMultiplayerScheduler _scheduler;
    private readonly IMultiplayerPlayerService _multiplayerPlayerService;
    
    private readonly IMongoCollection<RankedGame> _rankedGamesCollection;
    private readonly IMongoCollection<RankedGameJoin> _rankedGameJoinsCollection;

    public MultiplayerRankedService(
        IMultiplayerScheduler scheduler,
        IMultiplayerPlayerService multiplayerPlayerService)
    {
        _scheduler = scheduler;
        _multiplayerPlayerService = multiplayerPlayerService;
        
        var client = new MongoClient(Secrets.ConnectionString);
        var database = client.GetDatabase("BattleChess");
        _rankedGamesCollection = database.GetCollection<RankedGame>("RankedGames");
        _rankedGameJoinsCollection = database.GetCollection<RankedGameJoin>("RankedGameJoins");

        var version = Assembly.GetExecutingAssembly().GetName().Version;
        _version = version is not null 
            ? ((byte)version.Major) << 16 | (byte)version.Minor << 8 | (byte)version.Revision
            : -1;
    }

    public Task<Result<(bool isHost, RankedGame gameSearch, RankedGameJoin gameSearchJoin)>> FindRankedGameAsync(MapBlueprint myMap, CancellationToken cancellationToken)
    {
        lock (_scheduler.SyncLock)
        {
            var currentPlayer = _multiplayerPlayerService.LoggedInPlayer;
            if (currentPlayer is null)
            {
                return Task.FromResult(Result.Fail<(bool, RankedGame, RankedGameJoin)>("No player logged in"));
            }
            
            var random = new Random();
            var isHostStarting = random.Next(0, 1) == 1;
            
            return _scheduler.QueueTask(async () =>
            {
                var myMapData = myMap.GetByteData();
                try
                {
                    var eloDifference = 50;
                    var closestGameSearch = await GetClosestGameSearchAsync(currentPlayer, eloDifference, cancellationToken);
                    while (closestGameSearch is not null && Math.Abs(closestGameSearch.Elo - currentPlayer.Elo) <= eloDifference)
                    {
                        var joinResult = await TryToJoinGameAsync(closestGameSearch, currentPlayer, myMapData, cancellationToken);
                        if (joinResult.IsSuccess)
                        {
                            return Result.Ok<(bool, RankedGame, RankedGameJoin)>((false, closestGameSearch, joinResult.Value));
                        }
                        
                        closestGameSearch = await GetClosestGameSearchAsync(currentPlayer, eloDifference, cancellationToken);
                    }

                    var createdGameSearch = await CreateGameSearchAsync(myMapData, currentPlayer, isHostStarting, cancellationToken);
                    try
                    {
                        while (true)
                        {
                            var (waitResult, foundSearch, foundSearchJoin) = await WaitForLobbyOrJoinAsync(createdGameSearch.Id, currentPlayer.Elo, eloDifference, 20, cancellationToken);
                            if (waitResult == WaitResult.GameJoin)
                            {
                                var confirmationResult = await TryConfirmGameJoin(createdGameSearch, foundSearchJoin);
                                if (confirmationResult.IsSuccess)
                                {
                                    return Result.Ok((true, createdGameSearch, foundSearchJoin!));
                                }
                            }
                            else if (waitResult == WaitResult.GameSearch)
                            {
                                var joinResult = await TryToJoinGameAsync(foundSearch!, currentPlayer, myMapData, cancellationToken);
                                if (joinResult.IsSuccess)
                                {
                                    return Result.Ok<(bool, RankedGame, RankedGameJoin)>((false, foundSearch!, joinResult.Value));
                                }
                            }
                            else if (eloDifference < 300)
                            {
                                Console.WriteLine($"No game found with elo difference {eloDifference}, increasing to {eloDifference + 50}");
                                eloDifference += 50;
                            }
                            else
                            {
                                Console.WriteLine($"No game found with elo difference {eloDifference}, continuing search");
                            }
                        }                    
                    }
                    finally
                    {
                        if (string.IsNullOrEmpty(createdGameSearch.JoinedId))
                        {
                            await DeleteGameSearch(createdGameSearch);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex}");
                    return Result.Fail<(bool, RankedGame, RankedGameJoin)>("Failed to create lobby");
                }
            });
        }
    }

    private async Task<Result> TryConfirmGameJoin(RankedGame createdGameSearch, RankedGameJoin? foundSearchJoin)
    {
        Console.WriteLine("Confirming game join");
        var filter = Builders<RankedGame>.Filter.Eq(l => l.Id, createdGameSearch.Id);
        var update = Builders<RankedGame>.Update.Set(x => x.JoinedId, foundSearchJoin!.Id);
        // ReSharper disable once MethodSupportsCancellation
        var result = await _rankedGamesCollection.UpdateOneAsync(filter, update);
        if (!result.IsAcknowledged)
        {
            return Result.Fail("Failed to update game confirmation");
        }
        
        Console.WriteLine($"Confirmed game join for request: {foundSearchJoin.Id}");
        return Result.Ok();
    }

    private async Task DeleteGameSearch(RankedGame deletedGame)
    {
        Console.WriteLine("Deleting game search");
        
        var gameSearchFilter = Builders<RankedGame>.Filter.Eq(l => l.Id, deletedGame.Id);
        var gameSearchDeletion = await _rankedGamesCollection.DeleteManyAsync(gameSearchFilter);
                        
        Console.WriteLine($"Deleted game search count: {gameSearchDeletion.DeletedCount}");
        
        Console.WriteLine("Deleting game joins");
        
        var gameJoinFilter = Builders<RankedGameJoin>.Filter.Eq(l => l.GameId, deletedGame.Id);
        var gameJoinDeletion = await _rankedGameJoinsCollection.DeleteManyAsync(gameJoinFilter);
                        
        Console.WriteLine($"Deleted game join count: {gameJoinDeletion.DeletedCount}");
    }

    private enum WaitResult
    {
        Timeout,
        GameSearch,
        GameJoin
    }
    
    private async Task<(WaitResult result, RankedGame? search, RankedGameJoin? searchJoin)> WaitForLobbyOrJoinAsync(
        string gameId,
        short targetElo,
        int eloDifference,
        int timeoutSeconds,
        CancellationToken cancellationToken)
    {
        var timeoutTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(timeoutSeconds));
        var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutTokenSource.Token);

        var gameObjectId = ObjectId.Parse(gameId);
        var searchesPipeline = new EmptyPipelineDefinition<ChangeStreamDocument<RankedGame>>()
            .Match(change => change.OperationType == ChangeStreamOperationType.Insert &&
                             change.DocumentKey["_id"] > gameObjectId &&
                             change.FullDocument.Version == _version &&
                             string.IsNullOrEmpty(change.FullDocument.JoinedId) &&
                             Math.Abs(change.FullDocument.Elo - targetElo) < eloDifference);

        var joinPipeline = new EmptyPipelineDefinition<ChangeStreamDocument<RankedGameJoin>>()
            .Match(change => change.OperationType == ChangeStreamOperationType.Insert &&
                             change.FullDocument.GameId == gameId);

        var gameSearch = Task.Run<RankedGame?>(async () =>
        {
            using var cursor = await _rankedGamesCollection.WatchAsync(searchesPipeline, cancellationToken: cancellationTokenSource.Token);

            var filter = Builders<RankedGame>.Filter.And(
                Builders<RankedGame>.Filter.Gt(g => g.Id, gameId),
                Builders<RankedGame>.Filter.Eq(g => g.Version, _version),
                Builders<RankedGame>.Filter.Eq(g => g.JoinedId, null),
                Builders<RankedGame>.Filter.Where(g => Math.Abs(g.Elo - targetElo) < eloDifference));
            // ReSharper disable once PossiblyMistakenUseOfCancellationToken
            var foundGames = await _rankedGamesCollection.FindAsync(filter, cancellationToken: cancellationToken);
            // ReSharper disable once PossiblyMistakenUseOfCancellationToken
            var foundGame = await foundGames.FirstOrDefaultAsync(cancellationToken);
            if (foundGame is not null)
            {
                return foundGame;
            }
            
            while (await cursor.MoveNextAsync(cancellationTokenSource.Token))
            {
                foreach (var change in cursor.Current)
                {
                    return change.FullDocument;
                }
            }

            return null;
        }, cancellationTokenSource.Token);

        var joinTask = Task.Run<RankedGameJoin?>(async () =>
        {
            using var cursor = await _rankedGameJoinsCollection.WatchAsync(joinPipeline, cancellationToken: cancellationTokenSource.Token);

            var filter = Builders<RankedGameJoin>.Filter.Eq(g => g.GameId, gameId);
            // ReSharper disable once PossiblyMistakenUseOfCancellationToken
            var foundGames = await _rankedGameJoinsCollection.FindAsync(filter, cancellationToken: cancellationToken);
            // ReSharper disable once PossiblyMistakenUseOfCancellationToken
            var foundGame = await foundGames.FirstOrDefaultAsync(cancellationToken);
            if (foundGame is not null)
            {
                return foundGame;
            }
            
            while (await cursor.MoveNextAsync(cancellationTokenSource.Token))
            {
                foreach (var change in cursor.Current)
                {
                    return change.FullDocument;
                }
            }

            return null;
        }, cancellationTokenSource.Token);

        var completedTask = await Task.WhenAny(gameSearch, joinTask, Task.Delay(TimeSpan.FromSeconds(timeoutSeconds), cancellationTokenSource.Token));
        if (completedTask == gameSearch)
        {
            await cancellationTokenSource.CancelAsync(); // stop the other watch
            return (WaitResult.GameSearch, gameSearch.Result, null);
        }
        
        if (completedTask == gameSearch || completedTask == joinTask)
        {
            await cancellationTokenSource.CancelAsync(); // stop the other watch
            return (WaitResult.GameJoin, null, joinTask.Result);
        }

        return (WaitResult.Timeout, null, null);
    }

    private async Task<RankedGame?> WaitForGameAccept(
        RankedGame joinedGame, 
        int timeoutSeconds, 
        CancellationToken cancellationToken)
    {
        try
        {
            using var timeoutTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(timeoutSeconds));
            using var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutTokenSource.Token);
        
            var pipeline = new EmptyPipelineDefinition<ChangeStreamDocument<RankedGame>>()
                .Match(change =>
                    (change.OperationType == ChangeStreamOperationType.Update || change.OperationType == ChangeStreamOperationType.Delete) &&
                    change.DocumentKey["_id"] == ObjectId.Parse(joinedGame.Id));

            using var cursor = await _rankedGamesCollection.WatchAsync(
                pipeline,
                new ChangeStreamOptions { FullDocument = ChangeStreamFullDocumentOption.UpdateLookup },
                cancellationTokenSource.Token
            );
            
            Console.WriteLine($"Getting game with id: {joinedGame.Id}");
            var filter = Builders<RankedGame>.Filter.Eq("Id", joinedGame.Id);
            // ReSharper disable once PossiblyMistakenUseOfCancellationToken
            var foundGames = await _rankedGamesCollection.FindAsync(filter, cancellationToken: cancellationToken);
            // ReSharper disable once PossiblyMistakenUseOfCancellationToken
            var foundGame = await foundGames.FirstOrDefaultAsync(cancellationToken: cancellationToken);
            if (foundGame is null)
            {
                Console.WriteLine("Did not find game");
                return null;
            }

            if (!string.IsNullOrEmpty(foundGame.JoinedId))
            {
                await cancellationTokenSource.CancelAsync();
                return foundGame;
            }
            
            Console.WriteLine($"Found game with id: {foundGame.Id}");

            while (await cursor.MoveNextAsync(cancellationTokenSource.Token))
            {
                foreach (var change in cursor.Current)
                {
                    if (change.OperationType == ChangeStreamOperationType.Delete)
                    {
                        return null;
                    }
                    
                    if (change.FullDocument.Id == joinedGame.Id)
                    {
                        return change.FullDocument;
                    }
                }
            }

            return null;        
        }
        catch (OperationCanceledException)
        {
            return null;
        }
    }

    private async Task<RankedGame> CreateGameSearchAsync(byte[] myMapData, RegisteredPlayer currentPlayer, bool isHostStarting, CancellationToken cancellationToken)
    {
        Console.WriteLine("Creating game request");
        var game = new RankedGame()
        {
            Map = myMapData,
            PlayerId = currentPlayer.Id,
            Elo = currentPlayer.Elo,
            Version = _version,
            IsHostStarting = isHostStarting,
        };
        await _rankedGamesCollection.InsertOneAsync(game, cancellationToken: cancellationToken);
        Console.WriteLine($"Created game request: {game.Id}");
        return game;
    }

    private async Task<Result<RankedGameJoin>> TryToJoinGameAsync(
        RankedGame joinedGame, 
        RegisteredPlayer currentPlayer, 
        byte[] myMapData, 
        CancellationToken cancellationToken)
    {
        Console.WriteLine($"Creating join game: {joinedGame.Id}");
        var gameJoin = new RankedGameJoin()
        {
            GameId = joinedGame.Id,
            PlayerId = currentPlayer.Id,
            Map = myMapData,
        };
        await _rankedGameJoinsCollection.InsertOneAsync(gameJoin, cancellationToken: cancellationToken);
        Console.WriteLine($"Created join request with id: {joinedGame.Id}"); 
                        
        Console.WriteLine("Waiting for join request confirmation");
        var updatedJoinedGame = await WaitForGameAccept(joinedGame, 20, cancellationToken);
        if (updatedJoinedGame is null)
        {
            await DeleteGameSearch(joinedGame);
            Console.WriteLine("Invalid game search");
            return Result.Fail("Joining timed out");
        }
        else if (updatedJoinedGame.JoinedId == gameJoin.Id)
        {
            await DeleteGameSearch(updatedJoinedGame);
            Console.WriteLine($"Confirmed join request with id: {gameJoin.Id}");
            return Result.Ok(gameJoin);
        }
        else
        {
            Console.WriteLine("The lobby is already full");
            return Result.Fail("The lobby is already full");
        }
    }

    private async Task<RankedGame?> GetClosestGameSearchAsync(RegisteredPlayer currentPlayer, int eloDifference, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Searching for ranked game with elo: {currentPlayer.Elo - eloDifference}-{currentPlayer.Elo + eloDifference}");
        var closestGameSearch = await _rankedGamesCollection.Aggregate()
            .Match(l => l.Version == _version && string.IsNullOrEmpty(l.JoinedId))
            .Project(lobby => new
            {
                Lobby = lobby,
                EloDifference = Math.Abs(lobby.Elo - currentPlayer.Elo)
            })
            .SortBy(x => x.EloDifference)
            .Limit(1)
            .Project(x => x.Lobby)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);
        Console.WriteLine($"Closest game search elo: {closestGameSearch?.Elo ?? null}");
        return closestGameSearch;
    }

}
using System.Reflection;
using BattleChess3.Maps;
using BattleChess3.Multiplayer.Tables;
using FluentResults;
using MongoDB.Bson;
using MongoDB.Driver;

namespace BattleChess3.Multiplayer;

public class MultiplayerRankedService : IMultiplayerRankedService
{
    
    private readonly int _version;
    
    private readonly IMultiplayerScheduler _scheduler;
    private readonly IMultiplayerPlayerService _multiplayerPlayerService;
    
    private readonly IMongoCollection<RankedGame> _rankedGamesCollection;
    private readonly IMongoCollection<RankedGameJoins> _rankedGameJoinsCollection;

    public MultiplayerRankedService(
        IMultiplayerScheduler scheduler,
        IMultiplayerPlayerService multiplayerPlayerService)
    {
        _scheduler = scheduler;
        _multiplayerPlayerService = multiplayerPlayerService;
        
        var client = new MongoClient(DbSecrets.ConnectionString);
        var database = client.GetDatabase("BattleChess");
        _rankedGamesCollection = database.GetCollection<RankedGame>("RankedGames");
        _rankedGameJoinsCollection = database.GetCollection<RankedGameJoins>("RankedGameJoins");

        var version = Assembly.GetExecutingAssembly().GetName().Version;
        _version = version is not null 
            ? ((byte)version.Major) << 16 | (byte)version.Minor << 8 | (byte)version.Revision
            : -1;
    }

    public Task<Result<(bool isHost, RankedGame gameSearch, RankedGameJoins gameSearchJoin)>> FindRankedGameAsync(MapBlueprint myMap)
    {
        lock (_scheduler.SyncLock)
        {
            var currentPlayer = _multiplayerPlayerService.LoggedInPlayer;
            if (currentPlayer is null)
            {
                return Task.FromResult(Result.Fail<(bool, RankedGame, RankedGameJoins)>("No player logged in"));
            }
            
            var random = new Random();
            var isHostStarting = random.Next(0, 1) == 1;
            
            return _scheduler.QueueTask(async () =>
            {
                var myMapData = GetMapData(myMap);
                try
                {
                    var closestGameSearch = await GetClosestGameSearchAsync(currentPlayer);
                    while (closestGameSearch is not null && Math.Abs(closestGameSearch.Elo - currentPlayer.Elo) <= 50)
                    {
                        var joinResult = await TryToJoinGameAsync(closestGameSearch, currentPlayer, myMapData);
                        if (joinResult.IsSuccess)
                        {
                            return Result.Ok<(bool, RankedGame, RankedGameJoins)>((false, closestGameSearch, joinResult.Value));
                        }
                    }
                    
                    var eloDifference = 50;
                    var createdGameSearch = await CreateGameSearchAsync(myMapData, currentPlayer, isHostStarting);
                    while (true)
                    {
                        var (waitResult, foundSearch, foundSearchJoin) = await WaitForLobbyOrJoinAsync(createdGameSearch.Id, currentPlayer.Elo, eloDifference, 30);
                        if (waitResult == WaitResult.GameJoin)
                        {
                            Console.WriteLine("Confirming game join");
                            var filter = Builders<RankedGame>.Filter.Eq(l => l.Id, createdGameSearch.Id);
                            var update = Builders<RankedGame>.Update.Set(x => x.JoinedId, foundSearchJoin!.Id);
                            var result = await _rankedGamesCollection.UpdateOneAsync(filter, update);
                            if (!result.IsAcknowledged)
                            {
                                Result.Fail("Failed to update game confirmation");
                                continue;
                            }
                            Console.WriteLine($"Confirmed game join for request: {foundSearchJoin.Id}");
                            return Result.Ok<(bool, RankedGame, RankedGameJoins)>((true, createdGameSearch, foundSearchJoin));
                        }
                        else if (waitResult == WaitResult.GameSearch)
                        {
                            var joinResult = await TryToJoinGameAsync(foundSearch!, currentPlayer, myMapData);
                            if (joinResult.IsSuccess)
                            {
                                Console.WriteLine("Confirming game join");
                                var filter = Builders<RankedGame>.Filter.Eq(l => l.Id, createdGameSearch.Id);
                                var update = Builders<RankedGame>.Update.Set(x => x.JoinedId, joinResult.Value!.Id);
                                var result = await _rankedGamesCollection.UpdateOneAsync(filter, update);
                                if (!result.IsAcknowledged)
                                {
                                    Console.WriteLine("Failed to update game confirmation");
                                    continue;
                                }
                                Console.WriteLine($"Confirmed game join for request: {joinResult.Value.Id}");
                                return Result.Ok<(bool, RankedGame, RankedGameJoins)>((false, foundSearch!, joinResult.Value));
                            }
                        }
                        else
                        {
                            Console.WriteLine($"No game found with elo difference {eloDifference}, increasing to {eloDifference + 50}");
                            eloDifference += 50;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex}");
                    return Result.Fail<(bool, RankedGame, RankedGameJoins)>("Failed to create lobby");
                }
            });
        }
    }

    private enum WaitResult
    {
        Timeout,
        GameSearch,
        GameJoin
    }
    
    private async Task<(WaitResult result, RankedGame? search, RankedGameJoins? searchJoin)> WaitForLobbyOrJoinAsync(
        string gameId,
        short targetElo,
        int eloDifference,
        int timeoutSeconds)
    {
        var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(timeoutSeconds));

        var searchesPipeline = new EmptyPipelineDefinition<ChangeStreamDocument<RankedGame>>()
            .Match(change => change.OperationType == ChangeStreamOperationType.Insert &&
                             change.FullDocument.Version == _version &&
                             string.IsNullOrEmpty(change.FullDocument.JoinedId) &&
                             Math.Abs(change.FullDocument.Elo - targetElo) < eloDifference);

        var joinPipeline = new EmptyPipelineDefinition<ChangeStreamDocument<RankedGameJoins>>()
            .Match(change => change.OperationType == ChangeStreamOperationType.Insert &&
                             change.FullDocument.GameId == gameId);

        var gameSearch = Task.Run<RankedGame?>(async () =>
        {
            using var cursor = await _rankedGamesCollection.WatchAsync(searchesPipeline, cancellationToken: cancellationTokenSource.Token);
            while (await cursor.MoveNextAsync(cancellationTokenSource.Token))
            {
                foreach (var change in cursor.Current)
                {
                    return change.FullDocument;
                }
            }

            return null;
        }, cancellationTokenSource.Token);

        var joinTask = Task.Run<RankedGameJoins?>(async () =>
        {
            using var cursor = await _rankedGameJoinsCollection.WatchAsync(joinPipeline, cancellationToken: cancellationTokenSource.Token);
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
    
    public async Task<RankedGame?> WaitForGameAccept(string lobbyId, int timeoutSeconds = 30)
    {
        var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(timeoutSeconds));
        
        var pipeline = new EmptyPipelineDefinition<ChangeStreamDocument<RankedGame>>()
            .Match(change =>
                change.OperationType == ChangeStreamOperationType.Update &&
                change.DocumentKey["_id"] == ObjectId.Parse(lobbyId));

        
        using var cursor = await _rankedGamesCollection.WatchAsync(
            pipeline,
            new ChangeStreamOptions { FullDocument = ChangeStreamFullDocumentOption.UpdateLookup },
            cancellationTokenSource.Token
        );

        while (await cursor.MoveNextAsync(cancellationTokenSource.Token))
        {
            foreach (var change in cursor.Current)
            {
                if (change.FullDocument.Id == lobbyId)
                {
                    return change.FullDocument;
                }
            }
        }

        return null;
    }

    private async Task<RankedGame> CreateGameSearchAsync(byte[] myMapData, Player currentPlayer, bool isHostStarting)
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
        await _rankedGamesCollection.InsertOneAsync(game);
        Console.WriteLine($"Created game request: {game.Id}");
        return game;
    }

    private async Task<Result<RankedGameJoins>> TryToJoinGameAsync(RankedGame rankedGame, Player currentPlayer, byte[] myMapData)
    {
        Console.WriteLine($"Creating join game: {rankedGame.Id}");
        var gameJoin = new RankedGameJoins()
        {
            GameId = rankedGame.Id,
            PlayerId = currentPlayer.Id,
            Map = myMapData,
        };
        await _rankedGameJoinsCollection.InsertOneAsync(gameJoin);
        Console.WriteLine($"Created join request with id: {rankedGame.Id}"); 
                        
        Console.WriteLine("Waiting for join request confirmation");
        var lobbyUpdate = await WaitForGameAccept(rankedGame.Id);
        if (lobbyUpdate is null || lobbyUpdate.JoinedId != gameJoin.Id)
        {
            Console.WriteLine("The lobby is already full");
            return Result.Fail("The lobby is already full");
        }
        Console.WriteLine($"Confirmed join request with id: {gameJoin.Id}");
        return Result.Ok(gameJoin);
    }

    private async Task<RankedGame?> GetClosestGameSearchAsync(Player currentPlayer)
    {
        Console.WriteLine($"Searching for ranked game with elo: {currentPlayer.Elo - 50}-{currentPlayer.Elo + 50}");
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
            .FirstOrDefaultAsync();
        Console.WriteLine($"Closest game search elo: {closestGameSearch?.Elo ?? null}");
        return closestGameSearch;
    }

    private static byte[] GetMapData(MapBlueprint map)
    {
        var myMapData = new byte[32];
        for (var i = 0; i < map.Figures.Length; i++)
        {
            var index = i * 2;
            myMapData[index] = (byte)(map.Figures[i].PlayerId + (map.Figures[i].IsKing ? 128 : 0));
            myMapData[index + 1] = (byte)(map.Figures[i].FigureId);
        }

        return myMapData;
    }

    public Task<Result> DeleteGameAsync(string gameId)
    {
        lock (_scheduler.SyncLock)
        {
            return _scheduler.QueueTask(async () =>
            {
                try
                {
                    Console.WriteLine("Deleting GameJoins");
                    var filter = Builders<RankedGameJoins>.Filter.Eq(gj => gj.GameId, gameId);
                    var result = await _rankedGameJoinsCollection.DeleteManyAsync(filter);
                    Console.WriteLine($"Deleted GameJoins: {result.DeletedCount}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
                
                try
                {
                    Console.WriteLine("Deleting GameLobbies");
                    var filter = Builders<RankedGame>.Filter.Eq(gj => gj.Id, gameId);
                    var result = await _rankedGamesCollection.DeleteManyAsync(filter);
                    Console.WriteLine($"Deleted GameLobbies: {result.DeletedCount}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }

                return Result.Ok();
            });
        }
    }
}
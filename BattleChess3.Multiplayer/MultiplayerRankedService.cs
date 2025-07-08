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
    private readonly IMultiplayerLoginService _multiplayerLoginService;
    
    private readonly IMongoCollection<GameSearch> _gameSearchesCollection;
    private readonly IMongoCollection<GameSearchJoin> _gameSearchJoinsCollection;

    public MultiplayerRankedService(
        IMultiplayerScheduler scheduler,
        IMultiplayerLoginService multiplayerLoginService)
    {
        _scheduler = scheduler;
        _multiplayerLoginService = multiplayerLoginService;
        
        var client = new MongoClient(DbSecrets.ConnectionString);
        var database = client.GetDatabase("BattleChess");
        _gameSearchesCollection = database.GetCollection<GameSearch>("GameSearches");
        _gameSearchJoinsCollection = database.GetCollection<GameSearchJoin>("GameSearchJoins");

        var version = Assembly.GetExecutingAssembly().GetName().Version;
        _version = version is not null 
            ? ((byte)version.Major) << 16 | (byte)version.Minor << 8 | (byte)version.Revision
            : -1;
    }

    public Task<Result<(bool isHost, GameSearch gameSearch, GameSearchJoin gameSearchJoin)>> FindRankedGame(MapBlueprint myMap)
    {
        lock (_scheduler.SyncLock)
        {
            var currentPlayer = _multiplayerLoginService.LoggedInPlayer;
            if (currentPlayer is null)
            {
                return Task.FromResult(Result.Fail<(bool, GameSearch, GameSearchJoin)>("No player logged in"));
            }
            
            var random = new Random();
            var isHostStarting = random.Next(0, 1) == 1;
            
            return _scheduler.QueueTask(async () =>
            {
                var myMapData = GetMapData(myMap);
                try
                {
                    var closestGameSearch = await GetClosestGameSearchAsync(currentPlayer!);
                    while (closestGameSearch is not null && Math.Abs(closestGameSearch.Elo - currentPlayer!.Elo) <= 50)
                    {
                        var joinResult = await TryToJoinGameAsync(closestGameSearch, currentPlayer, myMapData);
                        if (joinResult.IsSuccess)
                        {
                            return Result.Ok<(bool, GameSearch, GameSearchJoin)>((false, closestGameSearch, joinResult.Value));
                        }
                    }
                    
                    var eloDifference = 50;
                    var createdGameSearch = await CreateGameSearchAsync(myMapData, currentPlayer!, isHostStarting);
                    while (true)
                    {
                        var (waitResult, foundSearch, foundSearchJoin) = await WaitForLobbyOrJoinAsync(createdGameSearch.Id, currentPlayer!.Elo, eloDifference, 30);
                        if (waitResult == WaitResult.GameJoin)
                        {
                            Console.WriteLine("Confirming game join");
                            var filter = Builders<GameSearch>.Filter.Eq(l => l.Id, createdGameSearch.Id);
                            var update = Builders<GameSearch>.Update.Set(x => x.JoinedId, foundSearchJoin!.Id);
                            var result = await _gameSearchesCollection.UpdateOneAsync(filter, update);
                            if (!result.IsAcknowledged)
                            {
                                Result.Fail("Failed to update game confirmation");
                                continue;
                            }

                            Console.WriteLine($"Confirmed game join for request: {foundSearchJoin.Id}");
                            return Result.Ok<(bool, GameSearch, GameSearchJoin)>((true, createdGameSearch, foundSearchJoin!));
                        }
                        else if (waitResult == WaitResult.GameSearch)
                        {
                            var joinResult = await TryToJoinGameAsync(foundSearch!, currentPlayer, myMapData);
                            if (joinResult.IsSuccess)
                            {
                                return Result.Ok<(bool, GameSearch, GameSearchJoin)>((false, foundSearch!, joinResult.Value));
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
                    return Result.Fail<(bool, GameSearch, GameSearchJoin)>("Failed to create lobby");
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
    
    private async Task<(WaitResult result, GameSearch? search, GameSearchJoin? searchJoin)> WaitForLobbyOrJoinAsync(
        string gameId,
        short targetElo,
        int eloDifference,
        int timeoutSeconds)
    {
        var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(timeoutSeconds));

        var searchesPipeline = new EmptyPipelineDefinition<ChangeStreamDocument<GameSearch>>()
            .Match(change => change.OperationType == ChangeStreamOperationType.Insert &&
                             change.FullDocument.Version == _version &&
                             string.IsNullOrEmpty(change.FullDocument.JoinedId) &&
                             Math.Abs(change.FullDocument.Elo - targetElo) < eloDifference);

        var joinPipeline = new EmptyPipelineDefinition<ChangeStreamDocument<GameSearchJoin>>()
            .Match(change => change.OperationType == ChangeStreamOperationType.Insert &&
                             change.FullDocument.GameId == gameId);

        var gameSearch = Task.Run<GameSearch?>(async () =>
        {
            using var cursor = await _gameSearchesCollection.WatchAsync(searchesPipeline, cancellationToken: cancellationTokenSource.Token);
            while (await cursor.MoveNextAsync(cancellationTokenSource.Token))
            {
                foreach (var change in cursor.Current)
                {
                    return change.FullDocument;
                }
            }

            return null;
        }, cancellationTokenSource.Token);

        var joinTask = Task.Run<GameSearchJoin?>(async () =>
        {
            using var cursor = await _gameSearchJoinsCollection.WatchAsync(joinPipeline, cancellationToken: cancellationTokenSource.Token);
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
    
    public async Task<GameSearch?> WaitForGameAccept(string lobbyId, int timeoutSeconds = 30)
    {
        var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(timeoutSeconds));
        
        var pipeline = new EmptyPipelineDefinition<ChangeStreamDocument<GameSearch>>()
            .Match(change =>
                change.OperationType == ChangeStreamOperationType.Update &&
                change.DocumentKey["_id"] == ObjectId.Parse(lobbyId));

        
        using var cursor = await _gameSearchesCollection.WatchAsync(
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

    private async Task<GameSearch> CreateGameSearchAsync(byte[] myMapData, Player currentPlayer, bool isHostStarting)
    {
        Console.WriteLine("Creating game request");
        var game = new GameSearch()
        {
            Map = myMapData,
            PlayerId = currentPlayer.Id,
            Elo = currentPlayer.Elo,
            Version = _version,
            IsHostStarting = isHostStarting,
        };
        await _gameSearchesCollection.InsertOneAsync(game);
        Console.WriteLine($"Created game request: {game.Id}");
        return game;
    }

    private async Task<Result<GameSearchJoin>> TryToJoinGameAsync(GameSearch gameSearch, Player currentPlayer, byte[] myMapData)
    {
        Console.WriteLine($"Creating join game: {gameSearch.Id}");
        var gameJoin = new GameSearchJoin()
        {
            GameId = gameSearch.Id,
            PlayerId = currentPlayer.Id,
            Map = myMapData,
        };
        await _gameSearchJoinsCollection.InsertOneAsync(gameJoin);
        Console.WriteLine($"Created join request with id: {gameSearch.Id}"); 
                        
        Console.WriteLine("Waiting for join request confirmation");
        var lobbyUpdate = await WaitForGameAccept(gameSearch.Id);
        if (lobbyUpdate is null || lobbyUpdate.JoinedId != gameJoin.Id)
        {
            Console.WriteLine("The lobby is already full");
            return Result.Fail("The lobby is already full");
        }
        Console.WriteLine($"Confirmed join request with id: {gameJoin.Id}");
        return Result.Ok(gameJoin);
    }

    private async Task<GameSearch?> GetClosestGameSearchAsync(Player currentPlayer)
    {
        Console.WriteLine($"Searching for ranked game with elo: {currentPlayer.Elo - 50}-{currentPlayer.Elo + 50}");
        var closestGameSearch = await _gameSearchesCollection.Aggregate()
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
                    var filter = Builders<GameSearchJoin>.Filter.Eq(gj => gj.GameId, gameId);
                    var result = await _gameSearchJoinsCollection.DeleteManyAsync(filter);
                    Console.WriteLine($"Deleted GameJoins: {result.DeletedCount}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
                
                try
                {
                    Console.WriteLine("Deleting GameLobbies");
                    var filter = Builders<GameSearch>.Filter.Eq(gj => gj.Id, gameId);
                    var result = await _gameSearchesCollection.DeleteManyAsync(filter);
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
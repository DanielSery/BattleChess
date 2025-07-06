using System.Collections.Concurrent;
using System.Reflection;
using BattleChess3.Game.Board;
using BattleChess3.Game.Figures;
using BattleChess3.Maps;
using BattleChess3.Multiplayer.Tables;
using MongoDB.Bson;
using MongoDB.Driver;

namespace BattleChess3.Multiplayer;

internal sealed class MultiplayerService : IMultiplayerService
{
    private string? _gameId;
    private readonly object _syncLock = new object();
    private Task? _runningTask;
    private readonly ConcurrentQueue<Func<Task>> _queuedTasks = new ConcurrentQueue<Func<Task>>();
    private readonly IMongoCollection<GameRequest> _gameRequestsCollection;
    private readonly IMongoCollection<GameJoin> _gameJoinsCollection;
    private readonly IMongoCollection<GameConfirm> _gameConfirmsCollection;
    private readonly IMongoCollection<GameTurn> _gameTurnsCollection;
    private readonly int _version;

    public MultiplayerService()
    {
        var client = new MongoClient(DbSecrets.ConnectionString);
        var database = client.GetDatabase("BattleChess");
        _gameRequestsCollection = database.GetCollection<GameRequest>("GameRequests");
        _gameJoinsCollection = database.GetCollection<GameJoin>("GameJoins");
        _gameConfirmsCollection = database.GetCollection<GameConfirm>("GameConfirms");
        _gameTurnsCollection = database.GetCollection<GameTurn>("GameTurns");

        var version = Assembly.GetExecutingAssembly().GetName().Version;
        _version = version is not null 
            ? ((byte)version.Major) << 16 | (byte)version.Minor << 8 | (byte)version.Revision
            : -1;
        
        AppDomain.CurrentDomain.ProcessExit += CurrentDomainOnProcessExit;
    }

    public bool IsConnected => IsHost || IsGuest;

    public bool IsHost { get; private set; }
    public bool IsGuest { get; private set; }

    public event EventHandler<(Position, Position)>? RequestPlayMove;
    public event EventHandler<MapBlueprint>? RequestLoadMap;
    public event EventHandler<string>? RequestDisplayMessage; 

    public Task<string> Host(bool isPublic, bool isHostStarting, MapBlueprint myMap)
    {
        lock (_syncLock)
        {
            if (IsConnected)
                return Task.FromResult(string.Empty);
        
            IsHost = true;
            IsGuest = false;
            var taskCompletionSource = new TaskCompletionSource<string>();
            
            QueueTask(async () =>
            {
                var myMapData = GetMapData(myMap);
                try
                {
                    Console.WriteLine("Creating game request");
                    var game = new GameRequest
                    {
                        Map = myMapData,
                        Elo = 1000,
                        IsPublic = isPublic,
                        Version = _version,
                        IsHostStarting = isHostStarting
                    };
                    await _gameRequestsCollection.InsertOneAsync(game);
                    Console.WriteLine($"Created game request: {game.Id}");
                    _gameId = game.Id;
                    taskCompletionSource.SetResult(_gameId);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    taskCompletionSource.SetException(ex);
                }
            });
            
            return taskCompletionSource.Task;
        }
    }

    public Task WaitForHostConfirmation(bool isHostStarting, MapBlueprint myMap)
    {
        lock (_syncLock)
        {
            if (!IsConnected || _gameId is null)
                return Task.CompletedTask;
        
            var taskCompletionSource = new TaskCompletionSource();
            QueueTask(async () =>
            {
                try
                {
                    Console.WriteLine($"Waiting for join request for game: {_gameId}");
                    var joinRequest = await WaitForGameJoinAsync(_gameId, 5 * 60);
                    Console.WriteLine($"Found join request: {joinRequest.Id}");
                    
                    Console.WriteLine("Creating game confirmation");
                    var gameConfirm = new GameConfirm
                    {
                        GameId = _gameId,
                        RequestId = joinRequest.Id,
                    };
                    var gameConfirmTask = _gameConfirmsCollection.InsertOneAsync(gameConfirm);
                    Console.WriteLine($"Created game confirmation for request: {joinRequest.Id}");
                    
                    var hisMap = GetMap(joinRequest.Map);
                    var joinedMap = GetMapBlueprint(myMap.Figures, hisMap, isHostStarting);
                    
                    RequestLoadMap?.Invoke(this, joinedMap);
                    await gameConfirmTask;
                    taskCompletionSource.SetResult();

                    if (joinedMap.StartingPlayer != 1)
                    {
                        await HandleHisTurn(null);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    taskCompletionSource.SetException(ex);
                } 
            });

            return taskCompletionSource.Task;
        }
    }

    public void Join(string gameId, MapBlueprint myMap)
    {
        lock (_syncLock)
        {
            if (IsConnected)
                return;
        
            IsGuest = true;
            IsHost = false;
            _gameId = gameId;
            QueueTask(async () =>
            {
                try
                {         
                    Console.WriteLine($"Searching for game request with id: {gameId}");
                    var filter = Builders<GameRequest>.Filter.And(
                        Builders<GameRequest>.Filter.Eq(g => g.Id, gameId),
                        Builders<GameRequest>.Filter.Eq(g => g.Version, _version)
                    );
                    var foundGames = await _gameRequestsCollection.FindAsync(filter);
                    var gameRequest = foundGames.FirstOrDefault();
                    Console.WriteLine($"Found game request with id: {gameId}");

                    Console.WriteLine($"Creating join request for game: {gameId}");
                    var gameJoin = new GameJoin
                    {
                        GameId = gameId,
                        Map = GetMapData(myMap),
                    };
                    var gameJoinTask = _gameJoinsCollection.InsertOneAsync(gameJoin);
                    
                    var hisMap = GetMap(gameRequest.Map);
                    var joinedMap = GetMapBlueprint(myMap.Figures, hisMap, !gameRequest.IsHostStarting);

                    await gameJoinTask;
                    Console.WriteLine($"Created join request with id: {gameJoin.Id}");
                    
                    Console.WriteLine("Waiting for join request confirmation");
                    var confirmation = await WaitForGameConfirmAsync(gameId);
                    Console.WriteLine($"Confirmed join request with id: {confirmation.RequestId}");
                    
                    RequestLoadMap?.Invoke(this, joinedMap);

                    if (joinedMap.StartingPlayer != 1)
                    {
                        await HandleHisTurn(null);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            });
        }
    }

    public void Stop()
    {
        lock (_syncLock)
        {
            if (!IsConnected || _gameId is null)
                return;
        
            var gameId = _gameId;
            IsHost = false;
            IsGuest = false;
            _gameId = null;
            QueueTask(async () =>
            {
                try
                {
                    Console.WriteLine("Deleting GameJoins");
                    var filter = Builders<GameJoin>.Filter.Eq(gj => gj.GameId, gameId);
                    var result = await _gameJoinsCollection.DeleteManyAsync(filter);
                    Console.WriteLine($"Deleted GameJoins: {result.DeletedCount}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
        
                try
                {
                    Console.WriteLine("Deleting GameTurns");
                    var filter = Builders<GameTurn>.Filter.Eq(gj => gj.GameId, gameId);
                    var result = await _gameTurnsCollection.DeleteManyAsync(filter);
                    Console.WriteLine($"Deleted GameTurns: {result.DeletedCount}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
        
                try
                {
                    Console.WriteLine("Deleting GameConfirms");
                    var filter = Builders<GameConfirm>.Filter.Eq(gj => gj.GameId, gameId);
                    var result = await _gameConfirmsCollection.DeleteManyAsync(filter);
                    Console.WriteLine($"Deleted GameConfirms: {result.DeletedCount}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
        
                try
                {
                    Console.WriteLine("Deleting GameRequests");
                    var filter = Builders<GameRequest>.Filter.Eq(gj => gj.Id, gameId);
                    var result = await _gameRequestsCollection.DeleteManyAsync(filter);
                    Console.WriteLine($"Deleted GameRequests: {result.DeletedCount}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            });
        }
    }

    public void PlayedMove(Position from, Position to)
    {
        lock (_syncLock)
        {
            if (!IsConnected || _gameId is null)
                return;
        
            QueueTask(async () =>
            {
                try
                {
                    Console.WriteLine($"Creating game turn: {from} to {to}");
                    var gameTurn = new GameTurn()
                    {
                        GameId = _gameId,
                        FromIndex = (byte)from.Index,
                        ToIndex = (byte)to.Index
                    };
                    await _gameTurnsCollection.InsertOneAsync(gameTurn);
                    Console.WriteLine($"Created game turn: {from} to {to}");

                    await HandleHisTurn(gameTurn.Id);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                } 
            });
        }
    }

    private async Task HandleHisTurn(string? turnId)
    {
        if (_gameId is null) throw new ArgumentNullException(nameof(turnId));
        
        Console.WriteLine($"Waiting for his turn with id greater than: {turnId}");
        var hisTurn = turnId is null
            ? await WaitForNextTurnAsync(_gameId)
            : await WaitForNextTurnAsync(turnId, _gameId);
        Console.WriteLine($"Found his turn with id: {hisTurn.Id}");

        RequestPlayMove?.Invoke(this, new ValueTuple<Position, Position>(
            GetPositionOfOppositePlayer(hisTurn.FromIndex),
            GetPositionOfOppositePlayer(hisTurn.ToIndex)));
    }

    private async Task<GameTurn?> WaitForNextTurnAsync(string gameId, int timeoutSeconds = 30)
    {
        var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(timeoutSeconds));
        var filter = Builders<ChangeStreamDocument<GameTurn>>.Filter.Eq(cs => cs.FullDocument.GameId, gameId);

        var pipeline = new EmptyPipelineDefinition<ChangeStreamDocument<GameTurn>>()
            .Match(filter);

        using var cursor = await _gameTurnsCollection.WatchAsync(
            pipeline,
            new ChangeStreamOptions { FullDocument = ChangeStreamFullDocumentOption.UpdateLookup },
            cancellationTokenSource.Token
        );

        while (await cursor.MoveNextAsync(cancellationTokenSource.Token))
        {
            foreach (var change in cursor.Current)
            {
                var turn = change.FullDocument;
                if (turn.GameId == gameId)
                {
                    return turn;
                }
            }
        }

        return null;
    }

    private async Task<GameTurn?> WaitForNextTurnAsync(string turnId, string gameId, int timeoutSeconds = 30)
    {
        var afterObjectId = ObjectId.Parse(turnId);
        var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(timeoutSeconds));

        var filter = Builders<ChangeStreamDocument<GameTurn>>.Filter.And(
            Builders<ChangeStreamDocument<GameTurn>>.Filter.Eq(cs => cs.FullDocument.GameId, gameId),
            Builders<ChangeStreamDocument<GameTurn>>.Filter.Gt(cs => cs.FullDocument.Id, turnId)
        );

        var pipeline = new EmptyPipelineDefinition<ChangeStreamDocument<GameTurn>>()
            .Match(filter);

        using var cursor = await _gameTurnsCollection.WatchAsync(
            pipeline,
            new ChangeStreamOptions { FullDocument = ChangeStreamFullDocumentOption.UpdateLookup },
            cancellationTokenSource.Token
        );

        while (await cursor.MoveNextAsync(cancellationTokenSource.Token))
        {
            foreach (var change in cursor.Current)
            {
                var turn = change.FullDocument;
                if (turn.GameId == gameId && ObjectId.Parse(turn.Id) > afterObjectId)
                {
                    return turn;
                }
            }
        }

        return null;
    }

    private async Task<GameJoin?> WaitForGameJoinAsync(string gameId, int timeoutSeconds = 30)
    {
        var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(timeoutSeconds));

        var pipeline = new EmptyPipelineDefinition<ChangeStreamDocument<GameJoin>>()
            .Match(Builders<ChangeStreamDocument<GameJoin>>.Filter
                .Eq(cs => cs.FullDocument.GameId, gameId));

        using var cursor = await _gameJoinsCollection.WatchAsync(
            pipeline,
            new ChangeStreamOptions { FullDocument = ChangeStreamFullDocumentOption.UpdateLookup },
            cancellationTokenSource.Token
        );

        while (await cursor.MoveNextAsync(cancellationTokenSource.Token))
        {
            foreach (var change in cursor.Current)
            {
                if (change.FullDocument.GameId == gameId)
                {
                    return change.FullDocument;
                }
            }
        }

        return null;
    }

    private async Task<GameConfirm?> WaitForGameConfirmAsync(string gameId, int timeoutSeconds = 30)
    {
        var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(timeoutSeconds));

        var pipeline = new EmptyPipelineDefinition<ChangeStreamDocument<GameConfirm>>()
            .Match(Builders<ChangeStreamDocument<GameConfirm>>.Filter
                .Eq(cs => cs.FullDocument.GameId, gameId));

        using var cursor = await _gameConfirmsCollection.WatchAsync(
            pipeline,
            new ChangeStreamOptions { FullDocument = ChangeStreamFullDocumentOption.UpdateLookup },
            cancellationTokenSource.Token
        );

        while (await cursor.MoveNextAsync(cancellationTokenSource.Token))
        {
            foreach (var change in cursor.Current)
            {
                if (change.FullDocument.GameId == gameId)
                {
                    return change.FullDocument;
                }
            }
        }

        return null;
    }

    private static MapBlueprint GetMapBlueprint(FigureIdentifier[] myFigures, FigureIdentifier[] hisFigures, bool amStarting)
    {
        var figures = new FigureIdentifier[64];
        var blueprint = new MapBlueprint
        {
            StartingPlayer = amStarting ? 1 : 2,
            Figures = figures
        };
            
        for (var i = 0; i < myFigures.Length; i++)
        {
            figures[i + 48] = myFigures[i];
        }

        for (var i = 16; i < 48; i++)
        {
            figures[i] = new FigureIdentifier(0, 0, false);
        }

        for (var i = 0; i < hisFigures.Length; i++)
        {
            figures[GetIndexOfOppositePlayer(i + 48)] = hisFigures[i];
        }

        return blueprint;
    }

    private static FigureIdentifier[] GetMap(byte[] map)
    {
        var figures = new FigureIdentifier[16];
        for (var i = 0; i < figures.Length; i++)
        {
            var index = i * 2;
            var playerId = map[index] % 128;
            if (playerId != 0)
                playerId = 3 - playerId;
            
            figures[i] = new FigureIdentifier(
                playerId,
                map[index + 1],
                map[index] / 128 == 1);
        }

        return figures;
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

    private static Position GetPositionOfOppositePlayer(int index)
    {
        return Position.FromIndex(index)
            .GetPlayerPOVPosition(1);
    }

    private static int GetIndexOfOppositePlayer(int index)
    {
        return GetPositionOfOppositePlayer(index).Index;
    }

    private void QueueTask(Func<Task> getTask)
    {
        lock (_syncLock)
        {
            _queuedTasks.Enqueue(getTask);
            _runningTask ??= Task.Run(async () =>
            {
                while (TryGetTaskToRun(out var task))
                {
                    await task;
                }
            });
        }
    }

    private bool TryGetTaskToRun(out Task task)
    {
        lock (_syncLock)
        {
            if (_queuedTasks.TryDequeue(out var getTask))
            {
                task = getTask.Invoke();
                return true;
            }

            task = Task.CompletedTask;
            _runningTask = null;
            return false;
        }
    }

    private void CurrentDomainOnProcessExit(object? sender, EventArgs e)
    {
        AppDomain.CurrentDomain.ProcessExit -= CurrentDomainOnProcessExit;
        Stop();
        _runningTask?.Wait();
    }
}
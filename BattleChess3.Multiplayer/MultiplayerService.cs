using System.Collections.Concurrent;
using System.Reflection;
using System.Timers;
using BattleChess3.Game.Board;
using BattleChess3.Game.Figures;
using BattleChess3.Maps;
using BattleChess3.Multiplayer.Tables;
using MongoDB.Driver;
using Timer = System.Timers.Timer;

namespace BattleChess3.Multiplayer;

internal sealed class MultiplayerService : IMultiplayerService
{
    private readonly Timer _timer;
    
    private string? _gameId;
    private readonly object _syncLock = new object();
    private Task? _runningTask;
    private readonly ConcurrentQueue<Func<Task>> _queuedTasks = new ConcurrentQueue<Func<Task>>();
    private readonly IMongoCollection<GameRequest> _gameRequestsCollection;
    private readonly IMongoCollection<GameJoin> _gameJoinsCollection;
    private readonly IMongoCollection<GameConfirm> _gameConfirmsCollection;
    private readonly int _version;

    public MultiplayerService()
    {
        var client = new MongoClient(DbSecrets.ConnectionString);
        var database = client.GetDatabase("BattleChess");
        _gameRequestsCollection = database.GetCollection<GameRequest>("GameRequests");
        _gameJoinsCollection = database.GetCollection<GameJoin>("GameJoins");
        _gameConfirmsCollection = database.GetCollection<GameConfirm>("GameConfirms");

        var version = Assembly.GetExecutingAssembly().GetName().Version;
        _version = version is not null 
            ? ((byte)version.Major) << 16 | (byte)version.Minor << 8 | (byte)version.Revision
            : -1;
        
        _timer = new Timer
        {
            AutoReset = true,
            Interval = 500,
            Enabled = true
        };
        _timer.Elapsed += TimerOnElapsed;
        
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
                    
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            });
        }
    }
    
    public async Task<GameJoin?> WaitForGameJoinAsync(string gameId, int timeoutSeconds = 30)
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
    
    public async Task<GameConfirm?> WaitForGameConfirmAsync(string gameId, int timeoutSeconds = 30)
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

    public void Stop()
    {
        // const string deleteGamesSql = "DELETE FROM Games WHERE Id = @id";
        // const string deleteTurnsSql = "DELETE FROM Turns WHERE GameId = @gameId";
        //
        // lock (_syncLock)
        // {
        //     if (!IsConnected || _gameId is null)
        //         return;
        //
        //     var gameId = _gameId;
        //     IsHost = false;
        //     IsGuest = false;
        //     _gameId = null;
        //     QueueTask(async () =>
        //     {
        //         try
        //         {
        //             await using var connection = new MySqlConnection(DbSecrets.ConnectionString);
        //             connection.Open();
        //             
        //             await using var command = new MySqlCommand(deleteGamesSql, connection);
        //             command.Parameters.AddWithValue("@id", unchecked((int)gameId));
        //
        //             Console.WriteLine("Before games deletion");
        //             var rowsDeleted = await command.ExecuteNonQueryAsync();
        //             Console.WriteLine($"Games deleted: {rowsDeleted}");
        //         }
        //         catch (Exception ex)
        //         {
        //             Console.WriteLine($"Error: {ex.Message}");
        //         }
        //
        //         try
        //         {
        //             await using var connection = new MySqlConnection(DbSecrets.ConnectionString);
        //             connection.Open();
        //
        //             await using var command = new MySqlCommand(deleteTurnsSql, connection);
        //             command.Parameters.AddWithValue("@gameId", unchecked((int)gameId));
        //
        //             Console.WriteLine("Before turns deletion");
        //             var rowsDeleted = command.ExecuteNonQuery();
        //             Console.WriteLine($"Turns deleted: {rowsDeleted}");
        //         }
        //         catch (Exception ex)
        //         {
        //             Console.WriteLine($"Error: {ex.Message}");
        //         }
        //     });
        // }
    }

    public void PlayedMove(Position from, Position to)
    {
        // const string insertSql = "INSERT INTO Turns (GameId, FromPosition, ToPosition) VALUES (@gameId, @fromPosition, @toPosition)";
        //
        // lock (_syncLock)
        // {
        //     if (!IsConnected || _gameId is null)
        //         return;
        //
        //     QueueTask(async () =>
        //     {
        //         try
        //         {
        //             await using var connection = new MySqlConnection(DbSecrets.ConnectionString);
        //             connection.Open();
        //             
        //             await using var command = new MySqlCommand(insertSql, connection);
        //             command.Parameters.AddWithValue("@gameId", unchecked((int)_gameId));
        //             command.Parameters.AddWithValue("@fromPosition", (byte)from.Index);
        //             command.Parameters.AddWithValue("@toPosition", (byte)to.Index);
        //
        //             Console.WriteLine("Before played move");
        //             await command.ExecuteNonQueryAsync();
        //             _lastProcessedTurn = (int)command.LastInsertedId;
        //             Console.WriteLine($"Played move: {command.LastInsertedId}");
        //         }
        //         catch (Exception ex)
        //         {
        //             Console.WriteLine($"Error: {ex.Message}");
        //         } 
        //     });
        // }
    }

    private void TimerOnElapsed(object? sender, ElapsedEventArgs e)
    {
        // const string selectSql = "SELECT Id, FromPosition, ToPosition FROM Turns WHERE GameId = @gameId AND Id > @lastProcessedTurn";
        //
        // lock (_syncLock)
        // {
        //     if (!IsConnected || _gameId is null)
        //         return;
        //
        //     QueueTask(async () =>
        //     {
        //         try
        //         {
        //             await using var connection = new MySqlConnection(DbSecrets.ConnectionString);
        //             connection.Open();
        //             
        //             Console.WriteLine($"Before update: {_lastProcessedTurn}");
        //             await using var command = new MySqlCommand(selectSql, connection);
        //             command.Parameters.AddWithValue("@gameId", unchecked((int)_gameId));
        //             command.Parameters.AddWithValue("@lastProcessedTurn", _lastProcessedTurn);
        //
        //             await using var reader = await command.ExecuteReaderAsync();
        //
        //             while (await reader.ReadAsync())
        //             {
        //                 _lastProcessedTurn = reader.GetInt32(0);
        //                 var fromPosition = reader.GetInt16(1);
        //                 var toPosition = reader.GetInt16(2);
        //                 RequestPlayMove?.Invoke(this, new ValueTuple<Position, Position>(
        //                     Position.FromIndex(fromPosition),
        //                     Position.FromIndex(toPosition)));
        //                 Console.WriteLine($"Requested move {_lastProcessedTurn}: {fromPosition} to {toPosition}");
        //             }
        //         }
        //         catch (Exception ex)
        //         {
        //             Console.WriteLine($"Error: {ex.Message}");
        //         }
        //     });
        // }
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
            var redPosition = Position.FromIndex(i + 48);
            figures[redPosition.GetPlayerPOVPosition(1).Index] = hisFigures[i];
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
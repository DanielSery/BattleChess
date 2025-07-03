using System.Collections.Concurrent;
using System.Timers;
using BattleChess3.Game.Board;
using BattleChess3.Game.Figures;
using BattleChess3.Maps;
using MySql.Data.MySqlClient;
using Timer = System.Timers.Timer;

namespace BattleChess3.Multiplayer;

internal sealed class MultiplayerService : IMultiplayerService
{
    private readonly Timer _timer;
    
    private uint? _gameId;
    private int _lastProcessedTurn;
    
    private readonly object _syncLock = new object();
    private Task? _runningTask;
    private readonly ConcurrentQueue<Func<Task>> _queuedTasks = new ConcurrentQueue<Func<Task>>();

    public MultiplayerService()
    {
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

    public void Host(uint gameId, MapBlueprint map)
    {
        const string insertSql = "INSERT INTO Games (Id, StartingPlayer, Map) VALUES (@id, @startingPlayer, @map)";

        lock (_syncLock)
        {
            if (IsConnected)
                return;

            IsHost = true;
            IsGuest = false;
            _gameId = gameId;
            QueueTask(async () =>
            {
                var startingPlayer = (byte)map.StartingPlayer;
                var mapData = new byte[192];
        
                for (var i = 0; i < map.Figures.Length; i++)
                {
                    var index = i * 3;
                    mapData[index] = (byte)(map.Figures[i].PlayerId + (map.Figures[i].IsKing ? 128 : 0));
                    mapData[index + 1] = (byte)(map.Figures[i].UniqueUnitId / 256);
                    mapData[index + 2] = (byte)(map.Figures[i].UniqueUnitId % 256);
                }
                
                try
                {
                    await using var connection = new MySqlConnection(DbSecrets.ConnectionString);
                    connection.Open();
                    
                    await using var command = new MySqlCommand(insertSql, connection);
                    command.Parameters.AddWithValue("@id", unchecked((int)gameId));
                    command.Parameters.AddWithValue("@startingPlayer", startingPlayer);
                    command.Parameters.AddWithValue("@map", mapData);

                    Console.WriteLine("Before start hosting");
                    await command.ExecuteNonQueryAsync();
                    _lastProcessedTurn = 0;
                    Console.WriteLine("After start hosting");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            });
        }
    }

    public void Join(uint? gameId)
    {
        const string selectSql = "SELECT StartingPlayer, Map FROM Games WHERE Id = @id";

        lock (_syncLock)
        {
            if (IsConnected || gameId is null)
                return;

            IsGuest = true;
            IsHost = false;
            _gameId = gameId;
            QueueTask(async () =>
            {
                try
                {         
                    await using var connection = new MySqlConnection(DbSecrets.ConnectionString);
                    connection.Open();
                    
                    await using var command = new MySqlCommand(selectSql, connection);
                    command.Parameters.AddWithValue("@id", unchecked((int)gameId));
                    
                    Console.WriteLine("Before start client");
                    await using var reader = await command.ExecuteReaderAsync();

                    if (await reader.ReadAsync())
                    {
                        var startingPlayer = reader.GetByte(0);

                        var mapData = new byte[192];
                        reader.GetBytes(1, 0, mapData, 0, mapData.Length);

                        var figures = new FigureIdentifier[64];
                        for (var i = 0; i < figures.Length; i++)
                        {
                            var index = i * 3;
                            figures[i] = new FigureIdentifier(mapData[index] % 128,
                                mapData[index + 1] * 256 + mapData[index + 2],
                                mapData[index] / 128 == 1);
                        }

                        var joinedMap = new MapBlueprint
                        {
                            StartingPlayer = startingPlayer,
                            Figures = figures
                        };
                        RequestLoadMap?.Invoke(this, joinedMap);
                        Console.WriteLine("After start client");
                        _lastProcessedTurn = 0;
                    }
                    else
                    {
                        Console.WriteLine($"No row found with Id = {gameId}");
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
        const string deleteGamesSql = "DELETE FROM Games WHERE Id = @id";
        const string deleteTurnsSql = "DELETE FROM Turns WHERE GameId = @gameId";
        
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
                    await using var connection = new MySqlConnection(DbSecrets.ConnectionString);
                    connection.Open();
                    
                    await using var command = new MySqlCommand(deleteGamesSql, connection);
                    command.Parameters.AddWithValue("@id", unchecked((int)gameId));

                    Console.WriteLine("Before games deletion");
                    var rowsDeleted = await command.ExecuteNonQueryAsync();
                    Console.WriteLine($"Games deleted: {rowsDeleted}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
        
                try
                {
                    await using var connection = new MySqlConnection(DbSecrets.ConnectionString);
                    connection.Open();

                    await using var command = new MySqlCommand(deleteTurnsSql, connection);
                    command.Parameters.AddWithValue("@gameId", unchecked((int)gameId));

                    Console.WriteLine("Before turns deletion");
                    var rowsDeleted = command.ExecuteNonQuery();
                    Console.WriteLine($"Turns deleted: {rowsDeleted}");
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
        const string insertSql = "INSERT INTO Turns (GameId, FromPosition, ToPosition) VALUES (@gameId, @fromPosition, @toPosition)";
        
        lock (_syncLock)
        {
            if (!IsConnected || _gameId is null)
                return;
       
            QueueTask(async () =>
            {
                try
                {
                    await using var connection = new MySqlConnection(DbSecrets.ConnectionString);
                    connection.Open();
                    
                    await using var command = new MySqlCommand(insertSql, connection);
                    command.Parameters.AddWithValue("@gameId", unchecked((int)_gameId));
                    command.Parameters.AddWithValue("@fromPosition", (byte)from.Index);
                    command.Parameters.AddWithValue("@toPosition", (byte)to.Index);

                    Console.WriteLine("Before played move");
                    await command.ExecuteNonQueryAsync();
                    _lastProcessedTurn = (int)command.LastInsertedId;
                    Console.WriteLine($"Played move: {command.LastInsertedId}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                } 
            });
        }
    }

    private void TimerOnElapsed(object? sender, ElapsedEventArgs e)
    {
        const string selectSql = "SELECT Id, FromPosition, ToPosition FROM Turns WHERE GameId = @gameId AND Id > @lastProcessedTurn";
        
        lock (_syncLock)
        {
            if (!IsConnected || _gameId is null)
                return;
       
            QueueTask(async () =>
            {
                try
                {
                    await using var connection = new MySqlConnection(DbSecrets.ConnectionString);
                    connection.Open();
                    
                    Console.WriteLine($"Before update: {_lastProcessedTurn}");
                    await using var command = new MySqlCommand(selectSql, connection);
                    command.Parameters.AddWithValue("@gameId", unchecked((int)_gameId));
                    command.Parameters.AddWithValue("@lastProcessedTurn", _lastProcessedTurn);

                    await using var reader = await command.ExecuteReaderAsync();

                    while (await reader.ReadAsync())
                    {
                        _lastProcessedTurn = reader.GetInt32(0);
                        var fromPosition = reader.GetInt16(1);
                        var toPosition = reader.GetInt16(2);
                        RequestPlayMove?.Invoke(this, new ValueTuple<Position, Position>(
                            Position.FromIndex(fromPosition),
                            Position.FromIndex(toPosition)));
                        Console.WriteLine($"Requested move {_lastProcessedTurn}: {fromPosition} to {toPosition}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            });
        }
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
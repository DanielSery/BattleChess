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
    private object _syncLock = new object();

    public MultiplayerService()
    {
        _timer = new Timer
        {
            AutoReset = true,
            Interval = 300,
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
        if (IsConnected)
            return;
        
        // Example values:
        var startingPlayer = (byte)map.StartingPlayer;
        var mapData = new byte[192];
        
        for (var i = 0; i < map.Figures.Length; i++)
        {
            var index = i * 3;
            mapData[index] = (byte)map.Figures[i].PlayerId;
            mapData[index + 1] = (byte)(map.Figures[i].UniqueUnitId / 256);
            mapData[index + 2] = (byte)(map.Figures[i].UniqueUnitId % 256);
        }

        // Insert SQL, exclude Id (auto-increment)
        const string insertSql = "INSERT INTO Games (Id, StartingPlayer, Map) VALUES (@id, @startingPlayer, @map)";

        lock (_syncLock)
        {
            try
            {
                var connection = new MySqlConnection(DbSecrets.ConnectionString);
                connection.Open();

                using var cmd = new MySqlCommand(insertSql, connection);
                cmd.Parameters.AddWithValue("@id", unchecked((int)gameId));
                cmd.Parameters.AddWithValue("@startingPlayer", startingPlayer);
                cmd.Parameters.AddWithValue("@map", mapData);

                var rowsInserted = cmd.ExecuteNonQuery();
                IsHost = true;
                _gameId = gameId;
                _lastProcessedTurn = 0;
                Console.WriteLine($"Rows inserted: {rowsInserted}");
            
                connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }

    public void Join(uint? gameId)
    {
        if (IsConnected || gameId is null)
            return;
        
        const string selectSql = "SELECT StartingPlayer, Map FROM Games WHERE Id = @id";

        lock (_syncLock)
        {
            try
            {
                var connection = new MySqlConnection(DbSecrets.ConnectionString);
                connection.Open();

                using var cmd = new MySqlCommand(selectSql, connection);
                cmd.Parameters.AddWithValue("@id", unchecked((int)gameId));

                using var reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    var startingPlayer = reader.GetByte("StartingPlayer");

                    // Map is binary(144), get bytes:
                    var mapData = new byte[192];
                    reader.GetBytes(reader.GetOrdinal("Map"), 0, mapData, 0, mapData.Length);

                    var figures = new FigureIdentifier[64];
                    for (var i = 0; i < figures.Length; i++)
                    {
                        var index = i * 3;
                        figures[i] = new FigureIdentifier(mapData[index],
                            mapData[index + 1] * 256 + mapData[index + 2]);
                    }

                    var joinedMap = new MapBlueprint
                    {
                        StartingPlayer = startingPlayer,
                        Figures = figures
                    };
                    RequestLoadMap?.Invoke(this, joinedMap);
                    Console.WriteLine("Requested load map.");
                    IsGuest = true;
                    _gameId = gameId;
                    _lastProcessedTurn = 0;
                }
                else
                {
                    Console.WriteLine($"No row found with Id = {gameId}");
                }
            
                connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }

    public void Stop()
    {
        Disconnect();
    }

    public void PlayedMove(Position from, Position to)
    {
        if (!IsConnected || _gameId is null)
            return;
        
        // Insert SQL, exclude Id (auto-increment)
        const string insertSql = "INSERT INTO Turns (GameId, FromPosition, ToPosition) VALUES (@gameId, @fromPosition, @toPosition)";

        lock (_syncLock)
        {
            try
            {
                var connection = new MySqlConnection(DbSecrets.ConnectionString);
                connection.Open();

                using var cmd = new MySqlCommand(insertSql, connection);
                cmd.Parameters.AddWithValue("@gameId", unchecked((int)_gameId));
                cmd.Parameters.AddWithValue("@fromPosition", (byte)from.Index);
                cmd.Parameters.AddWithValue("@toPosition", (byte)to.Index);

                var rowsInserted = cmd.ExecuteNonQuery();
                _lastProcessedTurn = (int)cmd.LastInsertedId;
                Console.WriteLine($"Rows inserted: {rowsInserted}");
            
                connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }

    private void TimerOnElapsed(object? sender, ElapsedEventArgs e)
    {
        if (!IsConnected || _gameId is null)
            return;
        
        const string selectSql = "SELECT Id, FromPosition, ToPosition FROM Turns WHERE GameId = @gameId AND Id > @lastProcessedTurn";

        lock (_syncLock)
        {
            try
            {
                var connection = new MySqlConnection(DbSecrets.ConnectionString);
                connection.Open();

                using var cmd = new MySqlCommand(selectSql, connection);
                cmd.Parameters.AddWithValue("@gameId", unchecked((int)_gameId));
                cmd.Parameters.AddWithValue("@lastProcessedTurn", _lastProcessedTurn);

                using var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    _lastProcessedTurn = reader.GetInt32("Id");
                    var fromPosition = reader.GetInt16("FromPosition");
                    var toPosition = reader.GetInt16("ToPosition");
                    RequestPlayMove?.Invoke(this, new ValueTuple<Position, Position>(fromPosition, toPosition));
                    Console.WriteLine($"Requested move: {fromPosition} to {toPosition}");
                }
            
                connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }

    private void Disconnect()
    {
        if (!IsConnected || _gameId is null)
            return;
        
        const string deleteGamesSql = "DELETE FROM Games WHERE Id = @id";

        lock (_syncLock)
        {
            try
            {
                var connection = new MySqlConnection(DbSecrets.ConnectionString);
                connection.Open();
            
                using var cmd = new MySqlCommand(deleteGamesSql, connection);
                cmd.Parameters.AddWithValue("@id", unchecked((int)_gameId));

                var rowsDeleted = cmd.ExecuteNonQuery();
                Console.WriteLine($"Rows deleted: {rowsDeleted}");
            
                connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        
            const string deleteTurnsSql = "DELETE FROM Turns WHERE GameId = @gameId";

            try
            {
                var connection = new MySqlConnection(DbSecrets.ConnectionString);
                connection.Open();
            
                using var cmd = new MySqlCommand(deleteTurnsSql, connection);
                cmd.Parameters.AddWithValue("@gameId", unchecked((int)_gameId));

                var rowsDeleted = cmd.ExecuteNonQuery();
                Console.WriteLine($"Rows deleted: {rowsDeleted}");
            
                connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        
            IsHost = false;
            IsGuest = false;
            _gameId = null;
        }
    }

    private void CurrentDomainOnProcessExit(object? sender, EventArgs e)
    {
        AppDomain.CurrentDomain.ProcessExit -= CurrentDomainOnProcessExit;
        Disconnect();
    }
}
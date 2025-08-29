using BattleChess3.Game.Figures;
using BattleChess3.Game.Players;
using Xunit;
using Assert = Xunit.Assert;

namespace BattleChess3.Game.Test;

public class GameServiceTest
{
    [Fact]
    public void StartGame_SetsCorrect_ForSingleWhite()
    {
        var gameService = new GameService();
        var player1 = CreatePlayer(Player.White, true);
        var player2 = CreatePlayer(Player.Black, true);

        gameService.StartGame(player1, player2, Player.White, multiplayer: false, hasTimer: false);

        Assert.True(gameService.CanMove, "gameService.CanMove");
        Assert.False(gameService.IsWaitingForMove, "gameService.IsWaitingForMove");
        Assert.False(gameService.HasTimer, "gameService.HasTimer");
        Assert.False(gameService.IsMultiplayer, "gameService.IsMultiplayer");
        Assert.Equal(Player.White, gameService.CurrentPlayerInfo.Player);
        Assert.False(gameService.CurrentPlayerInfo.CurrentStopwatch.IsRunning);
    }

    [Fact]
    public void StartGame_SetsCorrect_ForSingleBlack()
    {
        var gameService = new GameService();
        var player1 = CreatePlayer(Player.White, true);
        var player2 = CreatePlayer(Player.Black, true);

        gameService.StartGame(player1, player2, Player.Black, multiplayer: false, hasTimer: true);

        Assert.True(gameService.CanMove, "gameService.CanMove");
        Assert.False(gameService.IsWaitingForMove, "gameService.IsWaitingForMove");
        Assert.True(gameService.HasTimer, "gameService.HasTimer");
        Assert.False(gameService.IsMultiplayer, "gameService.IsMultiplayer");
        Assert.Equal(Player.Black, gameService.CurrentPlayerInfo.Player);
        Assert.True(gameService.CurrentPlayerInfo.CurrentStopwatch.IsRunning);
    }

    [Fact]
    public void StartGame_SetsCorrect_ForMultiplayerWhite()
    {
        var gameService = new GameService();
        var player1 = CreatePlayer(Player.White, true);
        var player2 = CreatePlayer(Player.Black, true);

        gameService.StartGame(player1, player2, Player.White, multiplayer: true, hasTimer: true);

        Assert.True(gameService.CanMove, "gameService.CanMove");
        Assert.False(gameService.IsWaitingForMove, "gameService.IsWaitingForMove");
        Assert.True(gameService.HasTimer, "gameService.HasTimer");
        Assert.True(gameService.IsMultiplayer, "gameService.IsMultiplayer");
        Assert.Equal(Player.White, gameService.CurrentPlayerInfo.Player);
        Assert.True(gameService.CurrentPlayerInfo.CurrentStopwatch.IsRunning);
    }

    [Fact]
    public void StartGame_SetsCorrect_ForMultiplayerBlack()
    {
        var gameService = new GameService();
        var player1 = CreatePlayer(Player.White, true);
        var player2 = CreatePlayer(Player.Black, true);

        gameService.StartGame(player1, player2, Player.Black, multiplayer: true, hasTimer: false);

        Assert.False(gameService.CanMove, "gameService.CanMove");
        Assert.True(gameService.IsWaitingForMove, "gameService.IsWaitingForMove");
        Assert.False(gameService.HasTimer, "gameService.HasTimer");
        Assert.True(gameService.IsMultiplayer, "gameService.IsMultiplayer");
        Assert.Equal(Player.Black, gameService.CurrentPlayerInfo.Player);
        Assert.False(gameService.CurrentPlayerInfo.CurrentStopwatch.IsRunning);
    }

    [Fact]
    public void StartTurn_SetsCorrect_ForSingleWhite()
    {
        var gameService = new GameService();
        var player1 = CreatePlayer(Player.White, true);
        var player2 = CreatePlayer(Player.Black, true);

        gameService.StartGame(player1, player2, Player.White, multiplayer: false, hasTimer: false);
        gameService.EndTurn();
        gameService.StartTurn();

        Assert.True(gameService.CanMove, "gameService.CanMove");
        Assert.False(gameService.IsWaitingForMove, "gameService.IsWaitingForMove");
        Assert.False(gameService.HasTimer, "gameService.HasTimer");
        Assert.False(gameService.IsMultiplayer, "gameService.IsMultiplayer");
        Assert.Equal(Player.Black, gameService.CurrentPlayerInfo.Player);
        Assert.False(gameService.CurrentPlayerInfo.CurrentStopwatch.IsRunning);
    }

    [Fact]
    public void StartTurn_SetsCorrect_ForSingleBlack()
    {
        var gameService = new GameService();
        var player1 = CreatePlayer(Player.White, true);
        var player2 = CreatePlayer(Player.Black, true);

        gameService.StartGame(player1, player2, Player.Black, multiplayer: false, hasTimer: true);
        gameService.EndTurn();
        gameService.StartTurn();

        Assert.True(gameService.CanMove, "gameService.CanMove");
        Assert.False(gameService.IsWaitingForMove, "gameService.IsWaitingForMove");
        Assert.True(gameService.HasTimer, "gameService.HasTimer");
        Assert.False(gameService.IsMultiplayer, "gameService.IsMultiplayer");
        Assert.Equal(Player.White, gameService.CurrentPlayerInfo.Player);
        Assert.True(gameService.CurrentPlayerInfo.CurrentStopwatch.IsRunning);
    }

    [Fact]
    public void StartTurn_SetsCorrect_ForMultiplayerWhite()
    {
        var gameService = new GameService();
        var player1 = CreatePlayer(Player.White, true);
        var player2 = CreatePlayer(Player.Black, true);

        gameService.StartGame(player1, player2, Player.White, multiplayer: true, hasTimer: true);
        gameService.EndTurn();
        gameService.StartTurn();

        Assert.False(gameService.CanMove, "gameService.CanMove");
        Assert.True(gameService.IsWaitingForMove, "gameService.IsWaitingForMove");
        Assert.True(gameService.HasTimer, "gameService.HasTimer");
        Assert.True(gameService.IsMultiplayer, "gameService.IsMultiplayer");
        Assert.Equal(Player.Black, gameService.CurrentPlayerInfo.Player);
        Assert.True(gameService.CurrentPlayerInfo.CurrentStopwatch.IsRunning);
    }

    [Fact]
    public void StartTurn_SetsCorrect_ForMultiplayerBlack()
    {
        var gameService = new GameService();
        var player1 = CreatePlayer(Player.White, true);
        var player2 = CreatePlayer(Player.Black, true);

        gameService.StartGame(player1, player2, Player.Black, multiplayer: true, hasTimer: false);
        gameService.EndTurn();
        gameService.StartTurn();

        Assert.True(gameService.CanMove, "gameService.CanMove");
        Assert.False(gameService.IsWaitingForMove, "gameService.IsWaitingForMove");
        Assert.False(gameService.HasTimer, "gameService.HasTimer");
        Assert.True(gameService.IsMultiplayer, "gameService.IsMultiplayer");
        Assert.Equal(Player.White, gameService.CurrentPlayerInfo.Player);
        Assert.False(gameService.CurrentPlayerInfo.CurrentStopwatch.IsRunning);
    }

    private static LocalHumanPlayerInfo CreatePlayer(Player player, bool hasKing)
    {
        var playerInfo = new LocalHumanPlayerInfo(player, string.Empty, null, null);
        if (hasKing)
        {
            playerInfo.Figures.Add(new Figure(playerInfo, NoneFigureType.Instance, true));
        }
        
        return playerInfo;
    }
}
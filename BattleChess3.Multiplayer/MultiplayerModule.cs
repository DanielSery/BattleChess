using Autofac;
using BattleChess3.Multiplayer.DatabaseAccess;
using BattleChess3.Multiplayer.Game;
using BattleChess3.Multiplayer.Lobby;
using BattleChess3.Multiplayer.Mail;
using BattleChess3.Multiplayer.Players;
using BattleChess3.Multiplayer.Ranked;
using BattleChess3.Multiplayer.Scheduling;

namespace BattleChess3.Multiplayer;

public static class MultiplayerModule
{
    public static void RegisterMultiplayerModule(this ContainerBuilder builder)
    {
        builder.RegisterType<DatabaseClient>()
            .As<IDatabaseClient>()
            .SingleInstance();
        builder.RegisterType<PlayersCollectionHandler>()
            .As<IPlayersCollectionHandler>()
            .SingleInstance();
        builder.RegisterType<EmailClient>()
            .As<IEmailClient>()
            .SingleInstance();
        
        builder.RegisterType<MultiplayerRankedService>()
            .As<IMultiplayerRankedService>()
            .SingleInstance();
        builder.RegisterType<MultiplayerLobbyService>()
            .As<IMultiplayerLobbyService>()
            .SingleInstance();
        builder.RegisterType<MultiplayerScheduler>()
            .As<IMultiplayerScheduler>()
            .SingleInstance();
        builder.RegisterType<MultiplayerPlayerService>()
            .As<IMultiplayerPlayerService>()
            .SingleInstance();
        builder.RegisterType<MultiplayerGameService>()
            .As<IMultiplayerGameService>()
            .SingleInstance();
    }
}
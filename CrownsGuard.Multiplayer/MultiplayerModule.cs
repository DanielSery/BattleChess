using Autofac;
using CrownsGuard.Multiplayer.DatabaseAccess;
using CrownsGuard.Multiplayer.Game;
using CrownsGuard.Multiplayer.Lobby;
using CrownsGuard.Multiplayer.Mail;
using CrownsGuard.Multiplayer.Players;
using CrownsGuard.Multiplayer.Ranked;
using CrownsGuard.Multiplayer.Scheduling;

namespace CrownsGuard.Multiplayer;

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
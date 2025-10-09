using Autofac;
using CrownsGuard.Multiplayer.Game;
using CrownsGuard.Multiplayer.Lobby;
using CrownsGuard.Multiplayer.Mail;
using CrownsGuard.Multiplayer.Players;
using CrownsGuard.Multiplayer.Ranked;

namespace CrownsGuard.Multiplayer;

public static class MultiplayerModule
{
    public static void RegisterMultiplayerModule(this ContainerBuilder builder)
    {
        builder.RegisterType<EmailClient>()
            .As<IEmailClient>()
            .SingleInstance();
        builder.RegisterType<MatchMakingService>()
            .As<IMatchmakingService>()
            .SingleInstance();
        builder.RegisterType<MultiplayerRankedService>()
            .As<IMultiplayerRankedService>()
            .SingleInstance();
        builder.RegisterType<MultiplayerLobbyService>()
            .As<IMultiplayerLobbyService>()
            .SingleInstance();
        builder.RegisterType<MultiplayerPlayerService>()
            .As<IMultiplayerPlayerService>()
            .SingleInstance();
        builder.RegisterType<MultiplayerGameService>()
            .As<IMultiplayerGameService>()
            .SingleInstance();
    }
}
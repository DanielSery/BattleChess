using Autofac;

namespace BattleChess3.Multiplayer;

public static class MultiplayerModule
{
    public static void RegisterMultiplayerModule(this ContainerBuilder builder)
    {
        builder.RegisterType<MultiplayerRankedService>()
            .As<IMultiplayerRankedService>()
            .SingleInstance();
        builder.RegisterType<MultiplayerLobbyService>()
            .As<IMultiplayerLobbyService>()
            .SingleInstance();
        builder.RegisterType<MultiplayerScheduler>()
            .As<IMultiplayerScheduler>()
            .SingleInstance();
        builder.RegisterType<MultiplayerLoginService>()
            .As<IMultiplayerLoginService>()
            .SingleInstance();
        builder.RegisterType<MultiplayerGameService>()
            .As<IMultiplayerGameService>()
            .SingleInstance();
    }
}
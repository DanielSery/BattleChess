using Autofac;

namespace BattleChess3.Multiplayer;

public static class MultiplayerModule
{
    public static void RegisterMultiplayerModule(this ContainerBuilder builder)
    {
        builder.RegisterType<MultiplayerScheduler>()
            .As<IMultiplayerScheduler>()
            .SingleInstance();
        builder.RegisterType<LoginService>()
            .As<ILoginService>()
            .SingleInstance();
        builder.RegisterType<MultiplayerService>()
            .As<IMultiplayerService>()
            .SingleInstance();
    }
}
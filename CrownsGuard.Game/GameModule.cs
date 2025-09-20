using Autofac;

namespace CrownsGuard.Game;

public static class GameModule
{
    public static void RegisterGamesModule(this ContainerBuilder builder)
    {
        builder.RegisterType<GameService>()
            .As<IGameService>()
            .As<IPlayersOwner>()
            .SingleInstance();
    }
}
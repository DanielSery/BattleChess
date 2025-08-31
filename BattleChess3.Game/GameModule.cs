using Autofac;
using BattleChess3.Core;

namespace BattleChess3.Game;

public static class GameModule
{
    public static void RegisterGamesModule(this ContainerBuilder builder)
    {
        builder.RegisterType<GameService>()
            .As<IGameService>()
            .As<IFigureOwnersHolder>()
            .SingleInstance();
    }
}
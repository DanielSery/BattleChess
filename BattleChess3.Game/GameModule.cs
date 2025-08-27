using Autofac;
using BattleChess3.Game.Figures;
using BattleChess3.Game.Players;

namespace BattleChess3.Game;

public static class GameModule
{
    public static void RegisterGamesModule(this ContainerBuilder builder)
    {
        builder.RegisterType<GameService>()
            .As<IGameService>()
            .SingleInstance();
    }
}
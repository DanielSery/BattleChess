using Autofac;
using CrownsGuard.Database.Database;
using CrownsGuard.Database.Game;
using CrownsGuard.Database.Lobby;
using CrownsGuard.Database.Players;
using CrownsGuard.Database.Ranked;

namespace CrownsGuard.Database;

public static class DatabaseModule
{
    public static void RegisterDatabaseModule(this ContainerBuilder builder)
    {
        builder.RegisterType<DatabaseClient>()
            .As<IDatabaseClient>()
            .SingleInstance();
        builder.RegisterType<DatabaseTimeProvider>()
            .As<IDatabaseTimeProvider>()
            .SingleInstance();
        builder.RegisterType<PlayersCollectionHandler>()
            .As<IPlayersCollectionHandler>()
            .SingleInstance();
        builder.RegisterType<GameTurnsCollectionHandler>()
            .As<IGameTurnsCollectionHandler>()
            .SingleInstance();
        builder.RegisterType<GameLobbiesCollectionHandler>()
            .As<IGameLobbiesCollectionHandler>()
            .SingleInstance();
        builder.RegisterType<GameLobbyJoinsCollectionHandler>()
            .As<IGameLobbyJoinsCollectionHandler>()
            .SingleInstance();
        builder.RegisterType<RankedGameJoinsCollectionHandler>()
            .As<IRankedGameJoinsCollectionHandler>()
            .SingleInstance();
        builder.RegisterType<RankedGamesCollectionHandler>()
            .As<IRankedGamesCollectionHandler>()
            .SingleInstance();

        // builder.RegisterType<DatabaseClearer>()
            // .As<IDatabaseClearer>()
            // .SingleInstance();
    }
}
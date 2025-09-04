using Autofac;
using BattleChess3.Maps.BoardBlueprints;
using BattleChess3.Maps.Figures;
using BattleChess3.Maps.IO;

namespace BattleChess3.Maps;

public static class MapsModule
{
    public static void RegisterMapsComponent(this ContainerBuilder builder)
    {
        builder.RegisterType<FileHandler>()
            .As<IFileHandler>()
            .SingleInstance();
        builder.RegisterType<DirectoryHandler>()
            .As<IDirectoryHandler>()
            .SingleInstance();
        builder.RegisterType<BoardBlueprintService>()
            .As<IBoardBlueprintService>()
            .SingleInstance();
        builder.RegisterType<BoardLoader>()
            .As<IBoardLoader>()
            .SingleInstance();
        builder.RegisterType<FigureCreator>()
            .As<IFigureCreator>()
            .SingleInstance();
    }
}
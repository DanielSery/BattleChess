using Autofac;
using CrownsGuard.Maps.BoardBlueprints;
using CrownsGuard.Maps.Figures;
using CrownsGuard.Maps.IO;

namespace CrownsGuard.Maps;

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
        builder.RegisterType<SetupLoadingService>()
            .As<ISetupLoadingService>()
            .SingleInstance();
        builder.RegisterType<BoardLoader>()
            .As<IBoardLoader>()
            .SingleInstance();
        builder.RegisterType<FigureCreator>()
            .As<IFigureCreator>()
            .SingleInstance();
    }
}
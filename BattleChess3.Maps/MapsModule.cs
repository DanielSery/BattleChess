using Autofac;
using BattleChess3.Maps.BoardBlueprints;
using BattleChess3.Maps.Figures;

namespace BattleChess3.Maps;

public static class MapsModule
{
    public static void RegisterMapsComponent(this ContainerBuilder builder)
    {
        builder.RegisterType<BoardBlueprintService>()
            .As<IBoardBlueprintService>()
            .SingleInstance();
        builder.RegisterType<BoardBlueprintLoader>()
            .As<IBoardBlueprintLoader>()
            .SingleInstance();
        builder.RegisterType<FigureCreator>()
            .As<IFigureCreator>()
            .SingleInstance();
    }
}
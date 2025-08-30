using Autofac;
using BattleChess3.Game.Figures;

namespace BattleChess3.CrossFireFigures;

public static class CrossFireFiguresModule
{
    public static void RegisterCrossFireFiguresModule(this ContainerBuilder builder)
    {
        builder.RegisterType<CrossFireFigureGroup>()
            .As<IFigureGroup>()
            .SingleInstance();
    }
}
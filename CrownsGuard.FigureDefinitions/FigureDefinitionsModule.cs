using Autofac;
using CrownsGuard.Core.Figures;

namespace CrownsGuard.FigureDefinitions;

public static class FigureDefinitionsModule
{
    public static void RegisterFigureDefinitionsModule(this ContainerBuilder builder)
    {
        builder.RegisterType<CrownsGuardFigureGroup>()
            .As<IFigureGroup>()
            .SingleInstance();
    }
}
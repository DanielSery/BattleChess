using Autofac;
using CrownsGuard.Core.Figures;

namespace CrownsGuard.FigureDefinitions;

public static class FigureDefinitionsModule
{
    public static void RegisterFigureDefinitionsModule(this ContainerBuilder builder)
    {
        builder.RegisterType<CrownsGuardFigureTypeInfoGroup>()
            .As<IFigureTypeInfoGroup>()
            .SingleInstance();
    }
}
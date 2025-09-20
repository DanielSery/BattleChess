using Autofac;

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
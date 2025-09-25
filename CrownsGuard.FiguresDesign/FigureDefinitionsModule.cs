using Autofac;

namespace CrownsGuard.FiguresDesign;

public static class FigureDefinitionsModule
{
    public static void RegisterFigureDefinitionsModule(this ContainerBuilder builder)
    {
        builder.RegisterType<CrownsGuardFigureTypeInfoGroup>()
            .As<IFigureTypeInfoGroup>()
            .SingleInstance();
    }
}
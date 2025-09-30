using Autofac;
using Microsoft.Extensions.Logging;

namespace CrownsGuard.Core;

public static class CoreModule
{
    public static void RegisterCoreModule(this ContainerBuilder builder)
    {
        builder.Register(_ =>
            {
                return LoggerFactory.Create(b =>
                {
                    b.AddConsole();
                });
            })
            .As<ILoggerFactory>()
            .SingleInstance();
        
        builder.RegisterGeneric(typeof(Logger<>))
            .As(typeof(ILogger<>))
            .SingleInstance();
    }
}
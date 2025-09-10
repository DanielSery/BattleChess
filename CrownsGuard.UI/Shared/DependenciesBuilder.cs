using System.Buffers;
using Autofac;
using Autofac.Extras.CommonServiceLocator;
using CrownsGuard.FigureDefinitions;
using CrownsGuard.Game;
using CrownsGuard.Maps;
using CrownsGuard.Multiplayer;
using CommonServiceLocator;
using CrownsGuard.Core.Figures;
using CrownsGuard.Database;
using CrownsGuard.UI.Editor;
using CrownsGuard.UI.Game;
using CrownsGuard.UI.MainWindow;
using CrownsGuard.UI.Multiplayer;
using CrownsGuard.UI.Services;
using CrownsGuard.UI.Settings;

namespace CrownsGuard.UI.Shared;

public static class DependenciesBuilder
{
    public static void Initialize()
    {
        if (ServiceLocator.IsLocationProviderSet) 
            return;

        ArrayPool<Figure>.Create(64, 50);
        ArrayPool<FigureAction>.Create(64, 50);
        
        var builder = new ContainerBuilder();
        SetUpComponents(builder);
        SetUpServiceLocator(builder);
            
        var locator = new AutofacServiceLocator(builder.Build());
        ServiceLocator.SetLocatorProvider(() => locator);
    }

    private static void SetUpComponents(ContainerBuilder builder)
    {
        builder.RegisterGamesModule();
        builder.RegisterFigureDefinitionsModule();
        builder.RegisterMapsComponent();
        builder.RegisterDatabaseModule();
        builder.RegisterMultiplayerModule();
    }

    private static void SetUpServiceLocator(ContainerBuilder builder)
    {
        builder.RegisterType<NotificationService>()
            .As<INotificationService>()
            .SingleInstance();
        builder.RegisterType<LoadingService>()
            .As<ILoadingService>()
            .SingleInstance();
        builder.RegisterType<SoundService>()
            .As<ISoundService>()
            .SingleInstance();
        
        builder.RegisterType<SettingsViewModel>()
            .SingleInstance();
        builder.RegisterType<MapsViewModel>()
            .SingleInstance();
        builder.RegisterType<BoardViewModel>()
            .SingleInstance();
        builder.RegisterType<EditorViewModel>()
            .SingleInstance();
        builder.RegisterType<EditorUnitsViewModel>()
            .SingleInstance();
        builder.RegisterType<TeamBoardViewModel>()
            .SingleInstance();
        builder.RegisterType<MultiplayerViewModel>()
            .SingleInstance();
        builder.RegisterType<LeaderboardViewModel>()
            .SingleInstance();
        builder.RegisterType<LoginViewModel>()
            .SingleInstance();
        builder.RegisterType<MenuViewModel>()
            .SingleInstance();
        builder.RegisterType<SignUpViewModel>()
            .SingleInstance();
        builder.RegisterType<GameViewModel>()
            .SingleInstance();
        builder.RegisterType<MainWindowViewModel>()
            .SingleInstance();
    }
}
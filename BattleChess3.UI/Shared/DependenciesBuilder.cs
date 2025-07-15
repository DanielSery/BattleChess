using Autofac;
using Autofac.Extras.CommonServiceLocator;
using BattleChess3.Game;
using BattleChess3.Maps;
using BattleChess3.Multiplayer;
using BattleChess3.UI.Editor;
using BattleChess3.UI.Game;
using BattleChess3.UI.MainWindow;
using BattleChess3.UI.Menu;
using BattleChess3.UI.Services;
using CommonServiceLocator;

namespace BattleChess3.UI.Shared;

public static class DependenciesBuilder
{
    public static void Initialize()
    {
        if (ServiceLocator.IsLocationProviderSet) 
            return;
        
        var builder = new ContainerBuilder();
        SetUpComponents(builder);
        SetUpServiceLocator(builder);
            
        var locator = new AutofacServiceLocator(builder.Build());
        ServiceLocator.SetLocatorProvider(() => locator);
    }

    private static void SetUpComponents(ContainerBuilder builder)
    {
        builder.RegisterGamesModule();
        builder.RegisterMapsComponent();
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
        builder.RegisterType<PlayersViewModel>()
            .SingleInstance();
        builder.RegisterType<MainWindowViewModel>()
            .SingleInstance();
    }
}
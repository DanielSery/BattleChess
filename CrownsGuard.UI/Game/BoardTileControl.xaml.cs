using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using CrownsGuard.Maps.GameBoard;
using CrownsGuard.UI.Shared;

namespace CrownsGuard.UI.Game;

public partial class BoardTileControl 
{
    private TileInfoViewModel _infoViewModel = new TileInfoViewModel(Position.None);
    private ColorAnimation? _animation;
    private static readonly Color MoveToColor = (Color)ColorConverter.ConvertFromString("#60309976");
    private static readonly Color DiedColor = (Color)ColorConverter.ConvertFromString("#40B25035");
    private static readonly Color CreatedColor = (Color)ColorConverter.ConvertFromString("#A0B28679");
    
    public BoardTileControl()
    {
        InitializeComponent();
    }

    private void Button_OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        _infoViewModel.Died -= InfoViewModelOnDied;
        _infoViewModel.MovedTo -= InfoViewModelOnMovedTo;
        _infoViewModel.Created -= InfoViewModelOnCreated;

        if (e.NewValue is not TileInfoViewModel tileViewModel) 
            return;
        
        _infoViewModel = tileViewModel;
        _infoViewModel.Died += InfoViewModelOnDied;
        _infoViewModel.MovedTo += InfoViewModelOnMovedTo;
        _infoViewModel.Created += InfoViewModelOnCreated;
    }

    private void InfoViewModelOnCreated(object? sender, EventArgs e)
    {
        Application.Current.Dispatcher.Invoke(() => AnimatedAction(CreatedColor));
    }

    private void InfoViewModelOnMovedTo(object? sender, EventArgs e)
    {
        Application.Current.Dispatcher.Invoke(() => AnimatedAction(MoveToColor));
    }

    private void InfoViewModelOnDied(object? sender, EventArgs e)
    {
        Application.Current.Dispatcher.Invoke(() => AnimatedAction(DiedColor));
    }

    private void AnimatedAction(Color targetColor)
    {
        if (_animation is not null)
            return;
        
        _animation = new ColorAnimation
        {
            From = Colors.Transparent,
            To = targetColor,
            Duration = TimeSpan.FromSeconds(0.4),
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut },
            AutoReverse = true,
            RepeatBehavior = new RepeatBehavior(1), // play once forward and once backward
            FillBehavior = FillBehavior.Stop // Prevents storyboard from holding value
        };

        _animation.Completed += (_, _) =>
        {
            TileAnimatedBackground.Color = Colors.Transparent;
            _animation = null;
        };

        TileAnimatedBackground.BeginAnimation(SolidColorBrush.ColorProperty, _animation);
    }
}
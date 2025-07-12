using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using BattleChess3.Game.Board;
using BattleChess3.UI.Shared;

namespace BattleChess3.UI.Game;

public partial class BoardTileControl 
{
    private TileViewModel _viewModel = new TileViewModel(Position.None);
    private static readonly Color MoveToColor = (Color)ColorConverter.ConvertFromString("#60309976");
    private static readonly Color DiedColor = (Color)ColorConverter.ConvertFromString("#40B25035");
    private static readonly Color CreatedColor = (Color)ColorConverter.ConvertFromString("#A0B28679");
    
    public BoardTileControl()
    {
        InitializeComponent();
    }

    private void Button_OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        _viewModel.Died -= ViewModelOnDied;
        _viewModel.MovedTo -= ViewModelOnMovedTo;
        _viewModel.Created -= ViewModelOnCreated;

        if (e.NewValue is not TileViewModel tileViewModel) 
            return;
        
        _viewModel = tileViewModel;
        _viewModel.Died += ViewModelOnDied;
        _viewModel.MovedTo += ViewModelOnMovedTo;
        _viewModel.Created += ViewModelOnCreated;
    }

    private void ViewModelOnCreated(object? sender, EventArgs e)
    {
        Application.Current.Dispatcher.Invoke(() => AnimatedAction(CreatedColor));
    }

    private void ViewModelOnMovedTo(object? sender, EventArgs e)
    {
        Application.Current.Dispatcher.Invoke(() => AnimatedAction(MoveToColor));
    }

    private void ViewModelOnDied(object? sender, EventArgs e)
    {
        Application.Current.Dispatcher.Invoke(() => AnimatedAction(DiedColor));
    }

    private void AnimatedAction(Color targetColor)
    {
        var animation = new ColorAnimation
        {
            From = Colors.Transparent,
            To = targetColor,
            Duration = TimeSpan.FromSeconds(1),
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut },
            AutoReverse = true,
            RepeatBehavior = new RepeatBehavior(1), // play once forward and once backward
            FillBehavior = FillBehavior.Stop // Prevents storyboard from holding value
        };

        animation.Completed += (_, _) =>
        {
            // Freeze to final color
            TileAnimatedBackground.Color = Colors.Transparent;
        };

        TileAnimatedBackground.BeginAnimation(SolidColorBrush.ColorProperty, animation);
    }
}
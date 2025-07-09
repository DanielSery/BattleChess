using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using BattleChess3.UI.Services;

namespace BattleChess3.UI.Views;

public partial class NotificationControl : UserControl
{
    public NotificationControl()
    {
        InitializeComponent();
        DataContextChanged += MainWindow_DataContextChanged;
    }

    public INotificationService? ViewModel { get; private set; }

    private void MainWindow_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (ViewModel is not null)
        {
            ViewModel.OnShownMessage -= ViewModelOnOnShownMessage;
        }

        if (DataContext is not INotificationService loadingService)
            return;

        ViewModel = loadingService;
        ViewModel.OnShownMessage += ViewModelOnOnShownMessage;
    }

    private async void ViewModelOnOnShownMessage(object? sender, ShownMessage e)
    {
        if (string.IsNullOrEmpty(e.Content))
            return;

        if (e.Type == ShownMessage.MessageType.Info)
            NotificationButton.Background = (Brush)Application.Current.Resources["ButtonBlueBackground"];
        else if (e.Type == ShownMessage.MessageType.Error)
            NotificationButton.Background = (Brush)Application.Current.Resources["ButtonOrangeBackground"];
        else if (e.Type == ShownMessage.MessageType.Warning)
            NotificationButton.Background = (Brush)Application.Current.Resources["ButtonRedBackground"];
        
        NotificationButton.Visibility = Visibility.Visible;
        NotificationButton.Opacity = 1;
        NotificationTextBlock.Text = e.Content;

        await Task.Delay(1000);

        var fadeOut = new DoubleAnimation
        {
            From = 1,
            To = 0,
            Duration = TimeSpan.FromSeconds(0.5),
            EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseIn },
            FillBehavior = FillBehavior.Stop
        };

        fadeOut.Completed += (_, _) =>
        {
            NotificationButton.Visibility = Visibility.Collapsed;
            NotificationButton.Opacity = 1; // Reset for next time
        };

        NotificationButton.BeginAnimation(OpacityProperty, fadeOut);
    }
}
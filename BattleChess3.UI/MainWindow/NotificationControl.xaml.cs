using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using BattleChess3.UI.Services;

namespace BattleChess3.UI.MainWindow;

public partial class NotificationControl
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

        if (NotificationButton.Visibility == Visibility.Visible)
        {
            await Task.Delay(4000);
        }

        Application.Current.Dispatcher.Invoke(() =>
        {
            NotificationButton.Background = e.Type switch
            {
                ShownMessage.MessageType.Info => (Brush)Application.Current.Resources["ButtonBlueBackground"],
                ShownMessage.MessageType.Error => (Brush)Application.Current.Resources["ButtonOrangeBackground"],
                ShownMessage.MessageType.Warning => (Brush)Application.Current.Resources["ButtonRedBackground"],
                ShownMessage.MessageType.Success => (Brush)Application.Current.Resources["ButtonGreenBackground"],
                _ => NotificationButton.Background
            };
            NotificationButton.Opacity = 1;
            NotificationTextBlock.Text = e.Content;
            NotificationButton.Visibility = Visibility.Visible;
        });

        await Task.Delay(3000);
        
        Application.Current.Dispatcher.Invoke(() =>
        {
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
        });
    }
}
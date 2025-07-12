using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using BattleChess3.UI.Services;

namespace BattleChess3.UI.Shared;

public partial class LoadingControl
{
    private readonly Brush _innerMouseOnBrush;
    private readonly Brush _spinnerMouseOnBrush;
    private readonly Brush _spinnerMouseOffBrush;
    
    public LoadingControl()
    {
        InitializeComponent();
        DataContextChanged += MainWindow_DataContextChanged;
        
        _innerMouseOnBrush = new SolidColorBrush(Color.FromArgb(32, 193, 182, 164));
        _innerMouseOnBrush.Freeze();
        
        _spinnerMouseOffBrush = new SolidColorBrush(Color.FromArgb(192, 193, 182, 164));
        _spinnerMouseOffBrush.Freeze();
        
        _spinnerMouseOnBrush =  new SolidColorBrush(Color.FromArgb(255, 193, 182, 164));
        _spinnerMouseOnBrush.Freeze();
    }

    public ILoadingService? ViewModel { get; private set; }

    private void MainWindow_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (ViewModel is not null)
        {
            ViewModel.LoadingChanged -= ViewModelOnLoadingChanged;
        }

        if (DataContext is not ILoadingService loadingService)
            return;

        ViewModel = loadingService;
        ViewModel.LoadingChanged += ViewModelOnLoadingChanged;
    }

    private void ViewModelOnLoadingChanged(object? sender, bool e)
    {
        Task.Delay(250).ContinueWith(_ =>
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (ViewModel?.IsLoading ?? false)
                {
                    StartSpinner();
                }
                else
                {
                    StopSpinner();
                }
            });
        });
    }
    
    private void StartSpinner()
    {
        LoadingOverlay.Visibility = Visibility.Visible;
        Focus();
        Storyboard sb = (Storyboard)Resources["SpinnerAnimation"];
        sb.Begin(this, true);
    }

    private void StopSpinner()
    {
        Storyboard sb = (Storyboard)Resources["SpinnerAnimation"];
        sb.Stop(this);
        LoadingOverlay.Visibility = Visibility.Collapsed;
    }

    private void Spinner_MouseEnter(object sender, MouseEventArgs e)
    {
        MessageTextBlock.Visibility = Visibility.Collapsed;
        CancelTextBlock.Visibility = Visibility.Visible;
        InnerEllipse.Fill = _innerMouseOnBrush;
        SpinnerPath.Stroke = _spinnerMouseOnBrush;
    }

    private void Spinner_MouseLeave(object sender, MouseEventArgs e)
    {
        MessageTextBlock.Visibility = Visibility.Visible;
        CancelTextBlock.Visibility = Visibility.Collapsed;
        InnerEllipse.Fill = Brushes.Transparent;
        SpinnerPath.Stroke = _spinnerMouseOffBrush;
    }

    private void Spinner_MouseUp(object sender, MouseButtonEventArgs e)
    {
        if (ViewModel is null)
            return;
        
        ViewModel.CancelCommand.Execute(null);
    }
}
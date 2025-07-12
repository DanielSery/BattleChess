using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using BattleChess3.UI.Game;
using BattleChess3.UI.Shared;

namespace BattleChess3.UI.MainWindow;

public partial class MainWindow
{
    public MainWindow()
    {
        InitializeComponent();
        TextElement.FontFamilyProperty.OverrideMetadata(
            typeof(TextElement),
            new FrameworkPropertyMetadata(
                new FontFamily("Britannic Bold")));

        TextBlock.FontFamilyProperty.OverrideMetadata(
            typeof(TextBlock),
            new FrameworkPropertyMetadata(
                new FontFamily("Britannic Bold")));
    }

    private void DragButton_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ButtonState == MouseButtonState.Pressed)
        {
            DragMove();
        }   
    }

    private void MinimizeButton_Click(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }

    private void MaximizeButton_Click(object sender, RoutedEventArgs e)
    {
        if (WindowState == WindowState.Normal)
        {
            WindowState = WindowState.Maximized;
            MaximizeButtonTextBlock.Text = "🗗"; // Change to restore icon
        }
        else if (WindowState == WindowState.Maximized)
        {
            WindowState = WindowState.Normal;
            MaximizeButtonTextBlock.Text = "🗖"; // Change to maximize icon
        }
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void Control_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (WindowState == WindowState.Normal)
        {
            WindowState = WindowState.Maximized;
            MaximizeButtonTextBlock.Text = "🗗"; // Change to restore icon
        }
        else if (WindowState == WindowState.Maximized)
        {
            WindowState = WindowState.Normal;
            MaximizeButtonTextBlock.Text = "🗖"; // Change to maximize icon
        }
    }

    private void GameBoard_OnLoaded(object sender, RoutedEventArgs e)
        => GameSideControlSetAppearAnimation(null, (sender as Border)?.DataContext);
    
    private void GameBoard_OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        => GameSideControlSetAppearAnimation(e.OldValue, e.NewValue);
    
    private void GameSideControlSetAppearAnimation(object? oldDataContext, object? newDataContext)
    {
        if (oldDataContext is MainWindowViewModel oldViewModel)
            oldViewModel.PropertyChanged -= ViewModelOnPropertyChanged;
    
        if (newDataContext is MainWindowViewModel newViewModel)
        {
            GameBoardBorder.Visibility = newViewModel.GameTabSelected ? Visibility.Visible : Visibility.Hidden;
            // GameSideControl.Visibility = newViewModel.GameTabSelected ? Visibility.Visible : Visibility.Hidden;
            newViewModel.PropertyChanged += ViewModelOnPropertyChanged;
        }
    
        void ViewModelOnPropertyChanged(object? o, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (propertyChangedEventArgs.PropertyName == nameof(MainWindowViewModel.GameTabSelected))
            {
                if ((o as MainWindowViewModel)!.GameTabSelected)
                {
                    ShowGameTabAnimation();
                }
                else
                {
                    HideGameTabAnimation();
                }
            }
        }
    }

    private void HideGameTabAnimation()
    {
        var parentHeight = ParentGameBoardBorder.ActualHeight;
        OffsetBorder.Height = 0;
        GameBoardTransform.Y = 0;
        MenuTransform.Y = -parentHeight * 3 / 4;
        GameBoardBorder.Visibility = Visibility.Visible;
        MenuBorder.Visibility = Visibility.Visible;
        
        var storyBoard = new Storyboard();
        var offsetAnimation = new DoubleAnimation
        {
            BeginTime = TimeSpan.Zero,
            From = 0, 
            To = parentHeight * 3 / 4,
            Duration = TimeSpan.FromMilliseconds(600),
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut },
            FillBehavior = FillBehavior.Stop
        };
        Storyboard.SetTarget(offsetAnimation, OffsetBorder);
        Storyboard.SetTargetProperty(offsetAnimation, new PropertyPath(HeightProperty));
        storyBoard.Children.Add(offsetAnimation);

        var transformAnimation = new DoubleAnimation
        {
            BeginTime = TimeSpan.Zero,
            From = 0,
            To = - parentHeight * 3 / 4,
            Duration = TimeSpan.FromMilliseconds(600),
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut },
            FillBehavior = FillBehavior.Stop
        };
        Storyboard.SetTarget(transformAnimation, TranslatedBorder);
        Storyboard.SetTargetProperty(transformAnimation, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.Y)"));
        storyBoard.Children.Add(transformAnimation);
        
        var menuTransformAnimation = new DoubleAnimation
        {
            BeginTime = TimeSpan.Zero,
            From =  -parentHeight * 3 / 4,
            To = 0,
            Duration = TimeSpan.FromMilliseconds(600),
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut },
            FillBehavior = FillBehavior.Stop
        };
        Storyboard.SetTarget(menuTransformAnimation, MenuBorder);
        Storyboard.SetTargetProperty(menuTransformAnimation, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.Y)"));
        storyBoard.Children.Add(menuTransformAnimation);
        
        storyBoard.Completed += StoryBoardOnCompleted;
        storyBoard.Begin();

        void StoryBoardOnCompleted(object? sender, EventArgs e)
        {
            GameBoardBorder.Visibility = Visibility.Hidden;
            OffsetBorder.Height = parentHeight * 3 / 4;
            GameBoardTransform.Y = -parentHeight * 3 / 4;
            MenuTransform.Y = 0;
        }
    }

    private void ShowGameTabAnimation()
    {
        var parentHeight = ParentGameBoardBorder.ActualHeight;
        OffsetBorder.Height = parentHeight * 3 / 4;
        GameBoardTransform.Y = -parentHeight * 3 / 4;
        MenuTransform.Y = 0;
        GameBoardBorder.Visibility = Visibility.Visible;
        MenuBorder.Visibility = Visibility.Visible;
        
        var storyBoard = new Storyboard();
        var offsetAnimation = new DoubleAnimation
        {
            BeginTime = TimeSpan.Zero,
            From = parentHeight * 3 / 4, 
            To = 0,
            Duration = TimeSpan.FromMilliseconds(600),
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut },
            FillBehavior = FillBehavior.Stop
        };
        Storyboard.SetTarget(offsetAnimation, OffsetBorder);
        Storyboard.SetTargetProperty(offsetAnimation, new PropertyPath(HeightProperty));
        storyBoard.Children.Add(offsetAnimation);

        var transformAnimation = new DoubleAnimation
        {
            BeginTime = TimeSpan.Zero,
            From = - parentHeight * 3 / 4,
            To = 0,
            Duration = TimeSpan.FromMilliseconds(600),
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut },
            FillBehavior = FillBehavior.Stop
        };
        Storyboard.SetTarget(transformAnimation, TranslatedBorder);
        Storyboard.SetTargetProperty(transformAnimation, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.Y)"));
        storyBoard.Children.Add(transformAnimation);
        
        var menuTransformAnimation = new DoubleAnimation
        {
            BeginTime = TimeSpan.Zero,
            From = 0,
            To = -parentHeight * 3 / 4,
            Duration = TimeSpan.FromMilliseconds(600),
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut },
            FillBehavior = FillBehavior.Stop
        };
        Storyboard.SetTarget(menuTransformAnimation, MenuBorder);
        Storyboard.SetTargetProperty(menuTransformAnimation, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.Y)"));
        storyBoard.Children.Add(menuTransformAnimation);
        
        storyBoard.Completed += StoryBoardOnCompleted;
        storyBoard.Begin();

        void StoryBoardOnCompleted(object? sender, EventArgs e)
        {
            MenuBorder.Visibility = Visibility.Hidden;
            OffsetBorder.Height = 0;
            GameBoardTransform.Y = 0;
            MenuTransform.Y = -parentHeight * 3 / 4;
        }
    }
}
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace CrownsGuard.UI.MainWindow;

public partial class MainWindow
{
    private Storyboard? _storyBoard;
    private EventHandler? _finishStoryboard;

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

    private void MainArea_OnLoaded(object sender, RoutedEventArgs e)
        => MainAreaAppearAnimation(null, (sender as Border)?.DataContext);
    
    private void MainArea_OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        => MainAreaAppearAnimation(e.OldValue, e.NewValue);
    
    private void MainAreaAppearAnimation(object? oldDataContext, object? newDataContext)
    {
        if (oldDataContext is MainWindowViewModel oldViewModel)
            oldViewModel.SelectedTabChanged -= OldViewModelOnSelectedTabChanged;
    
        if (newDataContext is MainWindowViewModel newViewModel)
            newViewModel.SelectedTabChanged += OldViewModelOnSelectedTabChanged;
    

        void OldViewModelOnSelectedTabChanged(object? sender, (SelectedMainWindowTab oldTab, SelectedMainWindowTab newTab) e)
        {
            if (_storyBoard is not null)
            {
                _storyBoard.Stop();
                _finishStoryboard?.Invoke(this, EventArgs.Empty);
            }
            
            switch (e)
            {
                case { oldTab: SelectedMainWindowTab.Menu, newTab: SelectedMainWindowTab.Game }:
                    MenuToGameTabAnimation();
                    break;
                case {oldTab: SelectedMainWindowTab.Game, newTab: SelectedMainWindowTab.Menu}:
                    GameTabToMenuAnimation();
                    break;
                case { oldTab:SelectedMainWindowTab.Menu, newTab: SelectedMainWindowTab.Editor}:
                    MenuToEditorTabAnimation();
                    break;
                case { oldTab: SelectedMainWindowTab.Editor, newTab: SelectedMainWindowTab.Menu}:
                    EditorToMenuTabAnimation();
                    break;
            }
        }
    }

    private void GameTabToMenuAnimation()
    {
        var parentHeight = ParentGameBoardBorder.ActualHeight;
        GameBoardInnerTransform.Y = parentHeight / 4;
        
        var storyBoard = new Storyboard();
        storyBoard.Children.Add(YTranslateAnimation(GameBoardMiddleControl, -parentHeight / 4, -parentHeight));
        storyBoard.Children.Add(YTranslateAnimation(GameBoardOuterControl, 0, parentHeight * 3 / 4));
        storyBoard.Children.Add(YTranslateAnimation(MenuControl, -parentHeight * 3 / 4, 0));
        storyBoard.Children.Add(OpacityAnimation(TeamBoardControl, 1, 0.85));
        storyBoard.Children.Add(OpacityAnimation(GameBoardOuterControl, 1, 0.85));
        storyBoard.Children.Add(YTranslateAnimation(MenuSideControl, -ParentMenuSideControl.ActualHeight, 0));
        storyBoard.Children.Add(YTranslateAnimation(GameSideControl, 0, ParentGameSideControl.ActualHeight));
        
        storyBoard.Completed += StoryBoardOnCompleted;
        _finishStoryboard = StoryBoardOnCompleted;
        storyBoard.Freeze();
        storyBoard.Begin();
        _storyBoard = storyBoard;
        
        void StoryBoardOnCompleted(object? sender, EventArgs e)
        {
            _storyBoard = null;
            GameBoardOuterControl.Visibility = Visibility.Collapsed;
            GameSideControl.Visibility = Visibility.Collapsed;
            
            MenuSideTransform.Y = 0;
            MenuTransform.Y = 0;
            TeamBoardControl.Opacity = 0.85;
        }
    }

    private void MenuToGameTabAnimation()
    {
        var parentHeight = ParentGameBoardBorder.ActualHeight;
        GameBoardInnerTransform.Y = parentHeight / 4;
        
        var storyBoard = new Storyboard();
        storyBoard.Children.Add(YTranslateAnimation(GameBoardMiddleControl, -parentHeight, -parentHeight / 4));
        storyBoard.Children.Add(YTranslateAnimation(GameBoardOuterControl, parentHeight * 3 / 4, 0));
        storyBoard.Children.Add(YTranslateAnimation(MenuControl, 0, -parentHeight * 3 / 4));
        storyBoard.Children.Add(OpacityAnimation(TeamBoardControl, 0.85, 1));
        storyBoard.Children.Add(OpacityAnimation(GameBoardOuterControl, 0.85, 1));
        storyBoard.Children.Add(YTranslateAnimation(MenuSideControl, 0, -ParentMenuSideControl.ActualHeight));
        storyBoard.Children.Add(YTranslateAnimation(GameSideControl, ParentGameSideControl.ActualHeight, 0));
        
        storyBoard.Completed += StoryBoardOnCompleted;
        _finishStoryboard = StoryBoardOnCompleted;
        storyBoard.Freeze();
        storyBoard.Begin();
        _storyBoard = storyBoard;
        
        void StoryBoardOnCompleted(object? sender, EventArgs e)
        {
            _storyBoard = null;
            MenuControl.Visibility = Visibility.Collapsed;
            TeamBoardControl.Visibility = Visibility.Collapsed;
            MenuSideControl.Visibility = Visibility.Collapsed;
            
            GameBoardOuterControl.Opacity = 1;
            GameBoardInnerTransform.Y = 0;
            GameBoardMiddleTransform.Y = 0;
            GameBoardOuterTransform.Y = 0;
            GameSideTransform.Y = 0;
        }
    }
    
    private void MenuToEditorTabAnimation()
    {
        var parentHeight = ParentGameBoardBorder.ActualHeight;
        
        var storyBoard = new Storyboard();
        storyBoard.Children.Add(YTranslateAnimation(EditorUnitsInnerControl, -parentHeight * 3 / 4, 0));
        storyBoard.Children.Add(YTranslateAnimation(EditorUnitsOuterControl, parentHeight * 3 / 4, 0));
        storyBoard.Children.Add(YTranslateAnimation(MenuControl, 0, -parentHeight * 3 / 4));
        storyBoard.Children.Add(OpacityAnimation(TeamBoardControl, 0.85, 1));
        storyBoard.Children.Add(YTranslateAnimation(MenuSideControl, 0, -ParentMenuSideControl.ActualHeight));
        storyBoard.Children.Add(YTranslateAnimation(EditorSideControl, ParentMenuSideControl.ActualHeight, 0));
        
        storyBoard.Completed += StoryBoardOnCompleted;
        _finishStoryboard = StoryBoardOnCompleted;
        storyBoard.Freeze();
        storyBoard.Begin();
        _storyBoard = storyBoard;
        
        void StoryBoardOnCompleted(object? sender, EventArgs e)
        {
            _storyBoard = null;
            MenuControl.Visibility = Visibility.Collapsed;
            MenuSideControl.Visibility = Visibility.Collapsed;

            EditorUnitsInnerTransform.Y = 0;
            EditorUnitsOuterTransform.Y = 0;
            EditorSideTransform.Y = 0;
            
            TeamBoardControl.Opacity = 1;
            TeamBoardControl.IsEnabled = true;
        }
    }
    
    private void EditorToMenuTabAnimation()
    {
        var parentHeight = ParentGameBoardBorder.ActualHeight;
        
        var storyBoard = new Storyboard();
        storyBoard.Children.Add(YTranslateAnimation(EditorUnitsInnerControl, 0, -parentHeight * 3 / 4));
        storyBoard.Children.Add(YTranslateAnimation(EditorUnitsOuterControl, 0, parentHeight * 3 / 4));
        storyBoard.Children.Add(YTranslateAnimation(MenuControl, -parentHeight * 3 / 4, 0));
        storyBoard.Children.Add(OpacityAnimation(TeamBoardControl, 1, 0.85));
        storyBoard.Children.Add(YTranslateAnimation(MenuSideControl, -ParentMenuSideControl.ActualHeight, 0));
        storyBoard.Children.Add(YTranslateAnimation(EditorSideControl, 0, ParentMenuSideControl.ActualHeight));
        
        storyBoard.Completed += StoryBoardOnCompleted;
        _finishStoryboard = StoryBoardOnCompleted;
        storyBoard.Freeze();
        storyBoard.Begin();
        _storyBoard = storyBoard;
        
        void StoryBoardOnCompleted(object? sender, EventArgs e)
        {
            _storyBoard = null;
            EditorUnitsOuterControl.Visibility = Visibility.Collapsed;
            EditorSideControl.Visibility = Visibility.Collapsed;

            MenuTransform.Y = 0;
            MenuSideTransform.Y = 0;
            
            TeamBoardControl.Opacity = 0.85;
            TeamBoardControl.IsEnabled = false;
        }
    }
    
    private void ParentSubMenuSideControl_OnLoaded(object sender, RoutedEventArgs e)
        => SideAreaAppearAnimation(null, sender);

    private void ParentSubMenuSideControl_OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        => SideAreaAppearAnimation(e.OldValue, e.NewValue);
    
    
    private void SideAreaAppearAnimation(object? oldDataContext, object? newDataContext)
    {
        if (oldDataContext is MenuViewModel oldViewModel)
            oldViewModel.SelectedTabChanged -= OldViewModelOnSelectedTabChanged;
    
        if (newDataContext is MenuViewModel newViewModel)
            newViewModel.SelectedTabChanged += OldViewModelOnSelectedTabChanged;
    

        void OldViewModelOnSelectedTabChanged(object? sender, (SelectedMenuTab oldTab, SelectedMenuTab newTab) e)
        {
            if (_storyBoard is not null)
            {
                _storyBoard.Stop();
                _finishStoryboard?.Invoke(this, EventArgs.Empty);
            }

            var parentHeight = ParentSubMenuSideControl.ActualHeight;
            var (oldControl, oldTransform) = GetSideTabControl(e.oldTab);
            var (newControl, newTransform) = GetSideTabControl(e.newTab);

            var storyBoard = new Storyboard();
            if (oldControl is not null)
            {
                storyBoard.Children.Add(YTranslateAnimation(oldControl, 0, -parentHeight));
            }

            if (newControl is not null)
            {
                storyBoard.Children.Add(YTranslateAnimation(newControl, parentHeight, 0));
            }
        
            storyBoard.Completed += StoryBoardOnCompleted;
            _finishStoryboard = StoryBoardOnCompleted;
            storyBoard.Freeze();
            storyBoard.Begin();
            _storyBoard = storyBoard;
        
            void StoryBoardOnCompleted(object? sender1, EventArgs e1)
            {
                _storyBoard = null;
                if (oldControl is not null)
                {
                    oldTransform!.Y = -parentHeight;
                    oldControl.Visibility = Visibility.Collapsed;
                }

                if (newControl is not null)
                {
                    newTransform!.Y = 0;
                    newControl.Visibility = Visibility.Visible;
                }
            }
        }
    }

    private (Border? control, TranslateTransform? transform) GetSideTabControl(SelectedMenuTab selectedMenuTab)
    {
        return selectedMenuTab switch
        {
            SelectedMenuTab.None => (null, null),
            SelectedMenuTab.Lobby => (LobbySideControl, LobbySideTransform),
            SelectedMenuTab.Login => (LoginSideControl, LoginSideTransform),
            SelectedMenuTab.SignUp => (SignupSideControl, SignupSideTransform),
            SelectedMenuTab.Settings => (SettingsSideControl, SettingsSideTransform),
            SelectedMenuTab.Leaderboard => (LeaderboardSideControl, LeaderboardSideTransform),
            _ => throw new ArgumentOutOfRangeException(nameof(selectedMenuTab), selectedMenuTab, null)
        };
    }

    private static DoubleAnimation OpacityAnimation(UIElement control, double from, double to)
    {
        control.Opacity = from;
        control.Visibility = Visibility.Visible;
        
        var teamBoardOpacityAnimation = new DoubleAnimation
        {
            BeginTime = TimeSpan.Zero,
            From = from,
            To = to,
            Duration = TimeSpan.FromMilliseconds(1000),
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut },
            FillBehavior = FillBehavior.Stop
        };
        Storyboard.SetTarget(teamBoardOpacityAnimation, control);
        Storyboard.SetTargetProperty(teamBoardOpacityAnimation, new PropertyPath("(UIElement.Opacity)"));
        teamBoardOpacityAnimation.Freeze();
        return teamBoardOpacityAnimation;
    }

    private static DoubleAnimation YTranslateAnimation(UIElement control, double from, double to)
    {
        (control.RenderTransform as TranslateTransform)!.Y = from;
        control.Visibility = Visibility.Visible;
        
        var innerTransformAnimation = new DoubleAnimation
        {
            BeginTime = TimeSpan.Zero,
            From = from,
            To = to, 
            Duration = TimeSpan.FromMilliseconds(1000),
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut },
            FillBehavior = FillBehavior.Stop
        };
        Storyboard.SetTarget(innerTransformAnimation, control);
        Storyboard.SetTargetProperty(innerTransformAnimation, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.Y)"));
        innerTransformAnimation.Freeze();
        return innerTransformAnimation;
    }
}
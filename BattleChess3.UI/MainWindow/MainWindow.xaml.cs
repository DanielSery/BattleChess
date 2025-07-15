using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using BattleChess3.UI.Menu;

namespace BattleChess3.UI.MainWindow;

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
        GameBoardMiddleTransform.Y = -parentHeight / 4;
        GameBoardOuterTransform.Y = 0;
        GameBoardOuterControl.Opacity = 1;
        GameSideTransform.Y = 0;
        
        MenuTransform.Y = 0;
        TeamBoardControl.Opacity = 1;
        MenuSideTransform.Y = -ParentMenuSideControl.ActualHeight;
        
        GameBoardOuterControl.Visibility = Visibility.Visible;
        GameSideControl.Visibility = Visibility.Visible;
        TeamBoardControl.Visibility = Visibility.Visible;
        MenuControl.Visibility = Visibility.Visible;
        MenuSideControl.Visibility = Visibility.Visible;
        
        var storyBoard = new Storyboard();
        var middleTransformAnimation = new DoubleAnimation
        {
            BeginTime = TimeSpan.Zero,
            From = -parentHeight / 4, 
            To = -parentHeight,
            Duration = TimeSpan.FromMilliseconds(1000),
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut },
            FillBehavior = FillBehavior.Stop
        };
        Storyboard.SetTarget(middleTransformAnimation, GameBoardMiddleControl);
        Storyboard.SetTargetProperty(middleTransformAnimation, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.Y)"));
        storyBoard.Children.Add(middleTransformAnimation);
        
        var outerTransformAnimation = new DoubleAnimation
        {
            BeginTime = TimeSpan.Zero,
            From = 0,
            To = parentHeight * 3 / 4,
            Duration = TimeSpan.FromMilliseconds(1000),
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut },
            FillBehavior = FillBehavior.Stop
        };
        Storyboard.SetTarget(outerTransformAnimation, GameBoardOuterControl);
        Storyboard.SetTargetProperty(outerTransformAnimation, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.Y)"));
        storyBoard.Children.Add(outerTransformAnimation);
        
        var menuTransformAnimation = new DoubleAnimation
        {
            BeginTime = TimeSpan.Zero,
            From = -parentHeight * 3 / 4,
            To = 0,
            Duration = TimeSpan.FromMilliseconds(1000),
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut },
            FillBehavior = FillBehavior.Stop
        };
        Storyboard.SetTarget(menuTransformAnimation, MenuControl);
        Storyboard.SetTargetProperty(menuTransformAnimation, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.Y)"));
        storyBoard.Children.Add(menuTransformAnimation);

        var teamBoardOpacityAnimation = new DoubleAnimation
        {
            BeginTime = TimeSpan.Zero,
            From = 1,
            To = 0.85,
            Duration = TimeSpan.FromMilliseconds(1000),
            FillBehavior = FillBehavior.Stop
        };
        Storyboard.SetTarget(teamBoardOpacityAnimation, TeamBoardControl);
        Storyboard.SetTargetProperty(teamBoardOpacityAnimation, new PropertyPath("(UIElement.Opacity)"));
        storyBoard.Children.Add(teamBoardOpacityAnimation);
        
        var outerControlOpacityAnimation = new DoubleAnimation
        {
            BeginTime = TimeSpan.Zero,
            From = 1,
            To = 0.85,
            Duration = TimeSpan.FromMilliseconds(1000),
            FillBehavior = FillBehavior.Stop
        };
        Storyboard.SetTarget(outerControlOpacityAnimation, GameBoardOuterControl);
        Storyboard.SetTargetProperty(outerControlOpacityAnimation, new PropertyPath("(UIElement.Opacity)"));
        storyBoard.Children.Add(outerControlOpacityAnimation);

        var gameSideTransformAnimation = new DoubleAnimation
        {
            BeginTime = TimeSpan.Zero,
            From = 0,
            To = ParentGameSideControl.ActualHeight,
            Duration = TimeSpan.FromMilliseconds(1000),
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut },
            FillBehavior = FillBehavior.Stop
        };
        Storyboard.SetTarget(gameSideTransformAnimation, GameSideControl);
        Storyboard.SetTargetProperty(gameSideTransformAnimation, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.Y)"));
        storyBoard.Children.Add(gameSideTransformAnimation);

        var menuSideTransformAnimation = new DoubleAnimation
        {
            BeginTime = TimeSpan.Zero,
            From = -ParentMenuSideControl.ActualHeight,
            To = 0,
            Duration = TimeSpan.FromMilliseconds(1000),
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut },
            FillBehavior = FillBehavior.Stop
        };
        Storyboard.SetTarget(menuSideTransformAnimation, MenuSideControl);
        Storyboard.SetTargetProperty(menuSideTransformAnimation, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.Y)"));
        storyBoard.Children.Add(menuSideTransformAnimation);
        
        storyBoard.Completed += StoryBoardOnCompleted;
        _finishStoryboard = StoryBoardOnCompleted;
        storyBoard.Freeze();
        storyBoard.Begin();
        _storyBoard = storyBoard;
        
        void StoryBoardOnCompleted(object? sender, EventArgs e)
        {
            _storyBoard = null;
            TeamBoardControl.Visibility = Visibility.Visible;
            MenuControl.Visibility = Visibility.Visible;
            MenuSideControl.Visibility = Visibility.Visible;
            GameBoardOuterControl.Visibility = Visibility.Collapsed;
            GameSideControl.Visibility = Visibility.Collapsed;
            
            GameBoardOuterControl.Opacity = 0.85;
            GameBoardInnerTransform.Y = parentHeight / 4;
            GameBoardMiddleTransform.Y = -parentHeight;
            GameBoardOuterTransform.Y = parentHeight * 3 / 4;
            GameSideTransform.Y = ParentGameSideControl.ActualHeight;
            
            MenuSideTransform.Y = 0;
            MenuTransform.Y = 0;
            TeamBoardControl.Opacity = 0.85;
        }
    }

    private void MenuToGameTabAnimation()
    {
        var parentHeight = ParentGameBoardBorder.ActualHeight;
        GameBoardInnerTransform.Y = parentHeight / 4;
        GameBoardMiddleTransform.Y = -parentHeight;
        GameBoardOuterTransform.Y = parentHeight * 3 / 4;
        GameBoardOuterControl.Opacity = 0.85;
        GameSideTransform.Y = ParentGameSideControl.ActualHeight;
        
        MenuTransform.Y = 0;
        TeamBoardControl.Opacity = 0.85;
        MenuSideTransform.Y = 0;
        
        GameBoardOuterControl.Visibility = Visibility.Visible;
        TeamBoardControl.Visibility = Visibility.Visible;
        MenuControl.Visibility = Visibility.Visible;
        MenuSideControl.Visibility = Visibility.Visible;
        GameSideControl.Visibility = Visibility.Visible;
        
        var storyBoard = new Storyboard();
        var middleTransformAnimation = new DoubleAnimation
        {
            BeginTime = TimeSpan.Zero,
            From = -parentHeight, 
            To = -parentHeight / 4,
            Duration = TimeSpan.FromMilliseconds(1000),
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut },
            FillBehavior = FillBehavior.Stop
        };
        Storyboard.SetTarget(middleTransformAnimation, GameBoardMiddleControl);
        Storyboard.SetTargetProperty(middleTransformAnimation, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.Y)"));
        storyBoard.Children.Add(middleTransformAnimation);
        
        var outerTransformAnimation = new DoubleAnimation
        {
            BeginTime = TimeSpan.Zero,
            From = parentHeight * 3 / 4,
            To = 0,
            Duration = TimeSpan.FromMilliseconds(1000),
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut },
            FillBehavior = FillBehavior.Stop
        };
        Storyboard.SetTarget(outerTransformAnimation, GameBoardOuterControl);
        Storyboard.SetTargetProperty(outerTransformAnimation, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.Y)"));
        storyBoard.Children.Add(outerTransformAnimation);
        
        var menuTransformAnimation = new DoubleAnimation
        {
            BeginTime = TimeSpan.Zero,
            From = 0,
            To = -parentHeight * 3 / 4,
            Duration = TimeSpan.FromMilliseconds(1000),
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut },
            FillBehavior = FillBehavior.Stop
        };
        Storyboard.SetTarget(menuTransformAnimation, MenuControl);
        Storyboard.SetTargetProperty(menuTransformAnimation, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.Y)"));
        storyBoard.Children.Add(menuTransformAnimation);

        var teamBoardOpacityAnimation = new DoubleAnimation
        {
            BeginTime = TimeSpan.Zero,
            From = 0.85,
            To = 1,
            Duration = TimeSpan.FromMilliseconds(1000),
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut },
            FillBehavior = FillBehavior.Stop
        };
        Storyboard.SetTarget(teamBoardOpacityAnimation, TeamBoardControl);
        Storyboard.SetTargetProperty(teamBoardOpacityAnimation, new PropertyPath("(UIElement.Opacity)"));
        storyBoard.Children.Add(teamBoardOpacityAnimation);

        var outerControlOpacityAnimation = new DoubleAnimation
        {
            BeginTime = TimeSpan.Zero,
            From = 0.85,
            To = 1,
            Duration = TimeSpan.FromMilliseconds(1000),
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut },
            FillBehavior = FillBehavior.Stop
        };
        Storyboard.SetTarget(outerControlOpacityAnimation, GameBoardOuterControl);
        Storyboard.SetTargetProperty(outerControlOpacityAnimation, new PropertyPath("(UIElement.Opacity)"));
        storyBoard.Children.Add(outerControlOpacityAnimation);

        var menuSideTransformAnimation = new DoubleAnimation
        {
            BeginTime = TimeSpan.Zero,
            From = 0,
            To = -ParentMenuSideControl.ActualHeight,
            Duration = TimeSpan.FromMilliseconds(1000),
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut },
            FillBehavior = FillBehavior.Stop
        };
        Storyboard.SetTarget(menuSideTransformAnimation, MenuSideControl);
        Storyboard.SetTargetProperty(menuSideTransformAnimation, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.Y)"));
        storyBoard.Children.Add(menuSideTransformAnimation);

        var gameSideTransformAnimation = new DoubleAnimation
        {
            BeginTime = TimeSpan.Zero,
            From = ParentGameSideControl.ActualHeight,
            To = 0,
            Duration = TimeSpan.FromMilliseconds(1000),
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut },
            FillBehavior = FillBehavior.Stop
        };
        Storyboard.SetTarget(gameSideTransformAnimation, GameSideControl);
        Storyboard.SetTargetProperty(gameSideTransformAnimation, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.Y)"));
        storyBoard.Children.Add(gameSideTransformAnimation);
        
        storyBoard.Completed += StoryBoardOnCompleted;
        _finishStoryboard = StoryBoardOnCompleted;
        storyBoard.Freeze();
        storyBoard.Begin();
        _storyBoard = storyBoard;
        
        void StoryBoardOnCompleted(object? sender, EventArgs e)
        {
            _storyBoard = null;
            GameBoardOuterControl.Visibility = Visibility.Visible;
            GameSideControl.Visibility = Visibility.Visible;
            MenuControl.Visibility = Visibility.Collapsed;
            TeamBoardControl.Visibility = Visibility.Collapsed;
            MenuSideControl.Visibility = Visibility.Collapsed;
            
            GameBoardOuterControl.Opacity = 1;
            GameBoardInnerTransform.Y = 0;
            GameBoardMiddleTransform.Y = 0;
            GameBoardOuterTransform.Y = 0;
            GameSideTransform.Y = 0;
            
            MenuTransform.Y = -parentHeight * 3 / 4;
            TeamBoardControl.Opacity = 0.85;
            MenuSideTransform.Y = -ParentMenuSideControl.ActualHeight;
        }
    }
    
    private void MenuToEditorTabAnimation()
    {
        var parentHeight = ParentGameBoardBorder.ActualHeight;
        EditorUnitsInnerTransform.Y = -parentHeight * 3 / 4;
        EditorUnitsOuterTransform.Y = parentHeight * 3 / 4;
        EditorSideTransform.Y = ParentEditorSideControl.ActualHeight;
        
        MenuTransform.Y = 0;
        TeamBoardControl.Opacity = 0.85;
        MenuSideTransform.Y = 0;
        TeamBoardControl.IsEnabled = true;
        
        EditorUnitsOuterControl.Visibility = Visibility.Visible;
        EditorSideControl.Visibility = Visibility.Visible;
        TeamBoardControl.Visibility = Visibility.Visible;
        MenuControl.Visibility = Visibility.Visible;
        MenuSideControl.Visibility = Visibility.Visible;
        
        var storyBoard = new Storyboard();
        var innerTransformAnimation = new DoubleAnimation
        {
            BeginTime = TimeSpan.Zero,
            From = -parentHeight * 3 / 4, 
            To = 0,
            Duration = TimeSpan.FromMilliseconds(1000),
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut },
            FillBehavior = FillBehavior.Stop
        };
        Storyboard.SetTarget(innerTransformAnimation, EditorUnitsInnerControl);
        Storyboard.SetTargetProperty(innerTransformAnimation, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.Y)"));
        storyBoard.Children.Add(innerTransformAnimation);
        
        var outerTransformAnimation = new DoubleAnimation
        {
            BeginTime = TimeSpan.Zero,
            From = parentHeight * 3 / 4,
            To = 0,
            Duration = TimeSpan.FromMilliseconds(1000),
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut },
            FillBehavior = FillBehavior.Stop
        };
        Storyboard.SetTarget(outerTransformAnimation, EditorUnitsOuterControl);
        Storyboard.SetTargetProperty(outerTransformAnimation, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.Y)"));
        storyBoard.Children.Add(outerTransformAnimation);
        
        var menuTransformAnimation = new DoubleAnimation
        {
            BeginTime = TimeSpan.Zero,
            From = 0,
            To = -parentHeight * 3 / 4,
            Duration = TimeSpan.FromMilliseconds(1000),
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut },
            FillBehavior = FillBehavior.Stop
        };
        Storyboard.SetTarget(menuTransformAnimation, MenuControl);
        Storyboard.SetTargetProperty(menuTransformAnimation, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.Y)"));
        storyBoard.Children.Add(menuTransformAnimation);

        var teamBoardOpacityAnimation = new DoubleAnimation
        {
            BeginTime = TimeSpan.Zero,
            From = 0.85,
            To = 1,
            Duration = TimeSpan.FromMilliseconds(1000),
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut },
            FillBehavior = FillBehavior.Stop
        };
        Storyboard.SetTarget(teamBoardOpacityAnimation, TeamBoardControl);
        Storyboard.SetTargetProperty(teamBoardOpacityAnimation, new PropertyPath("(UIElement.Opacity)"));
        storyBoard.Children.Add(teamBoardOpacityAnimation);

        var menuSideTransformAnimation = new DoubleAnimation
        {
            BeginTime = TimeSpan.Zero,
            From = 0,
            To = -ParentMenuSideControl.ActualHeight,
            Duration = TimeSpan.FromMilliseconds(1000),
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut },
            FillBehavior = FillBehavior.Stop
        };
        Storyboard.SetTarget(menuSideTransformAnimation, MenuSideControl);
        Storyboard.SetTargetProperty(menuSideTransformAnimation, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.Y)"));
        storyBoard.Children.Add(menuSideTransformAnimation);

        var editorSideTransformAnimation = new DoubleAnimation
        {
            BeginTime = TimeSpan.Zero,
            From = ParentEditorSideControl.ActualHeight,
            To = 0,
            Duration = TimeSpan.FromMilliseconds(1000),
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut },
            FillBehavior = FillBehavior.Stop
        };
        Storyboard.SetTarget(editorSideTransformAnimation, EditorSideControl);
        Storyboard.SetTargetProperty(editorSideTransformAnimation, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.Y)"));
        storyBoard.Children.Add(editorSideTransformAnimation);
        
        storyBoard.Completed += StoryBoardOnCompleted;
        _finishStoryboard = StoryBoardOnCompleted;
        storyBoard.Freeze();
        storyBoard.Begin();
        _storyBoard = storyBoard;
        
        void StoryBoardOnCompleted(object? sender, EventArgs e)
        {
            _storyBoard = null;
            EditorUnitsOuterControl.Visibility = Visibility.Visible;
            EditorSideControl.Visibility = Visibility.Visible;
            TeamBoardControl.Visibility = Visibility.Visible;
            MenuControl.Visibility = Visibility.Collapsed;
            MenuSideControl.Visibility = Visibility.Collapsed;

            EditorUnitsInnerTransform.Y = 0;
            EditorUnitsOuterTransform.Y = 0;
            EditorSideTransform.Y = 0;
            
            MenuTransform.Y = -parentHeight * 3 / 4;
            TeamBoardControl.Opacity = 1;
            MenuSideTransform.Y = -ParentMenuSideControl.ActualHeight;
            TeamBoardControl.IsEnabled = true;
        }
    }
    
    private void EditorToMenuTabAnimation()
    {
        var parentHeight = ParentGameBoardBorder.ActualHeight;
        EditorUnitsInnerTransform.Y = 0;
        EditorUnitsOuterTransform.Y = 0;
        EditorSideTransform.Y = 0;
        
        MenuTransform.Y = -parentHeight * 3 / 4;
        TeamBoardControl.Opacity = 1;
        MenuSideTransform.Y = -ParentMenuSideControl.ActualHeight;
        
        EditorUnitsOuterControl.Visibility = Visibility.Visible;
        EditorSideControl.Visibility = Visibility.Visible;
        TeamBoardControl.Visibility = Visibility.Visible;
        MenuControl.Visibility = Visibility.Visible;
        MenuSideControl.Visibility = Visibility.Visible;
        
        var storyBoard = new Storyboard();
        var innerTransformAnimation = new DoubleAnimation
        {
            BeginTime = TimeSpan.Zero,
            To = -parentHeight * 3 / 4, 
            From = 0,
            Duration = TimeSpan.FromMilliseconds(1000),
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut },
            FillBehavior = FillBehavior.Stop
        };
        Storyboard.SetTarget(innerTransformAnimation, EditorUnitsInnerControl);
        Storyboard.SetTargetProperty(innerTransformAnimation, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.Y)"));
        storyBoard.Children.Add(innerTransformAnimation);
        
        var outerTransformAnimation = new DoubleAnimation
        {
            BeginTime = TimeSpan.Zero,
            To = parentHeight * 3 / 4,
            From = 0,
            Duration = TimeSpan.FromMilliseconds(1000),
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut },
            FillBehavior = FillBehavior.Stop
        };
        Storyboard.SetTarget(outerTransformAnimation, EditorUnitsOuterControl);
        Storyboard.SetTargetProperty(outerTransformAnimation, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.Y)"));
        storyBoard.Children.Add(outerTransformAnimation);
        
        var menuTransformAnimation = new DoubleAnimation
        {
            BeginTime = TimeSpan.Zero,
            To = 0,
            From = -parentHeight * 3 / 4,
            Duration = TimeSpan.FromMilliseconds(1000),
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut },
            FillBehavior = FillBehavior.Stop
        };
        Storyboard.SetTarget(menuTransformAnimation, MenuControl);
        Storyboard.SetTargetProperty(menuTransformAnimation, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.Y)"));
        storyBoard.Children.Add(menuTransformAnimation);

        var teamBoardOpacityAnimation = new DoubleAnimation
        {
            BeginTime = TimeSpan.Zero,
            To = 0.85,
            From = 1,
            Duration = TimeSpan.FromMilliseconds(1000),
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut },
            FillBehavior = FillBehavior.Stop
        };
        Storyboard.SetTarget(teamBoardOpacityAnimation, TeamBoardControl);
        Storyboard.SetTargetProperty(teamBoardOpacityAnimation, new PropertyPath("(UIElement.Opacity)"));
        storyBoard.Children.Add(teamBoardOpacityAnimation);

        var menuSideTransformAnimation = new DoubleAnimation
        {
            BeginTime = TimeSpan.Zero,
            To = 0,
            From = -ParentMenuSideControl.ActualHeight,
            Duration = TimeSpan.FromMilliseconds(1000),
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut },
            FillBehavior = FillBehavior.Stop
        };
        Storyboard.SetTarget(menuSideTransformAnimation, MenuSideControl);
        Storyboard.SetTargetProperty(menuSideTransformAnimation, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.Y)"));
        storyBoard.Children.Add(menuSideTransformAnimation);

        var editorSideTransformAnimation = new DoubleAnimation
        {
            BeginTime = TimeSpan.Zero,
            To = ParentEditorSideControl.ActualHeight,
            From = 0,
            Duration = TimeSpan.FromMilliseconds(1000),
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut },
            FillBehavior = FillBehavior.Stop
        };
        Storyboard.SetTarget(editorSideTransformAnimation, EditorSideControl);
        Storyboard.SetTargetProperty(editorSideTransformAnimation, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.Y)"));
        storyBoard.Children.Add(editorSideTransformAnimation);
        
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
            TeamBoardControl.Visibility = Visibility.Visible;
            MenuControl.Visibility = Visibility.Visible;
            MenuSideControl.Visibility = Visibility.Visible;

            EditorUnitsInnerTransform.Y = -parentHeight * 3 / 4;
            EditorUnitsOuterTransform.Y = parentHeight * 3 / 4;
            EditorSideTransform.Y = ParentEditorSideControl.ActualHeight;
            
            MenuTransform.Y = 0;
            TeamBoardControl.Opacity = 0.95;
            MenuSideTransform.Y = 0;
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
                oldTransform!.Y = 0;
                oldControl.Visibility = Visibility.Visible;
                
                var oldTransformAnimation = new DoubleAnimation
                {
                    BeginTime = TimeSpan.Zero,
                    From = 0,
                    To = -parentHeight, 
                    Duration = TimeSpan.FromMilliseconds(1000),
                    EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut },
                    FillBehavior = FillBehavior.Stop
                };
                Storyboard.SetTarget(oldTransformAnimation, oldControl);
                Storyboard.SetTargetProperty(oldTransformAnimation, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.Y)"));
                storyBoard.Children.Add(oldTransformAnimation);
            }

            if (newControl is not null)
            {
                newTransform!.Y = parentHeight;
                newControl.Visibility = Visibility.Visible;
                
                var newTransformAnimation = new DoubleAnimation
                {
                    BeginTime = TimeSpan.Zero,
                    From = parentHeight,
                    To = 0, 
                    Duration = TimeSpan.FromMilliseconds(1000),
                    EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut },
                    FillBehavior = FillBehavior.Stop
                };
                Storyboard.SetTarget(newTransformAnimation, newControl);
                Storyboard.SetTargetProperty(newTransformAnimation, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.Y)"));
                storyBoard.Children.Add(newTransformAnimation);
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
            SelectedMenuTab.Leaderboard => (LeaderboardSideControl, LeaderboardSideTransform),
            _ => throw new ArgumentOutOfRangeException(nameof(selectedMenuTab), selectedMenuTab, null)
        };
    }
}
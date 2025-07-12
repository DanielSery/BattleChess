using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace BattleChess3.UI.MainWindow;

public partial class MainWindow
{
    private Storyboard? _storyBoard;

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
            oldViewModel.SelectedTabChanged -= OldViewModelOnSelectedTabChanged;
    
        if (newDataContext is MainWindowViewModel newViewModel)
        {
            TeamBoardControl.Visibility = newViewModel.SelectedTab is SelectedMainWindowTab.Editor or SelectedMainWindowTab.Menu ? Visibility.Visible : Visibility.Hidden;
            MenuSideControl.Visibility = newViewModel.SelectedTab == SelectedMainWindowTab.Menu ? Visibility.Visible : Visibility.Hidden;
            MenuControl.Visibility = newViewModel.SelectedTab == SelectedMainWindowTab.Menu ? Visibility.Visible : Visibility.Hidden;
            
            GameBoardOuterControl.Visibility = newViewModel.SelectedTab == SelectedMainWindowTab.Game ? Visibility.Visible : Visibility.Collapsed;
            GameSideControl.Visibility = newViewModel.SelectedTab == SelectedMainWindowTab.Game ? Visibility.Visible : Visibility.Collapsed;
            
            EditorUnitsOuterControl.Visibility = newViewModel.SelectedTab == SelectedMainWindowTab.Editor ? Visibility.Visible : Visibility.Collapsed;
            EditorSideControl.Visibility = newViewModel.SelectedTab == SelectedMainWindowTab.Editor ? Visibility.Visible : Visibility.Collapsed;
            
            newViewModel.SelectedTabChanged += OldViewModelOnSelectedTabChanged;
        }
    

        void OldViewModelOnSelectedTabChanged(object? sender, (SelectedMainWindowTab oldTab, SelectedMainWindowTab newTab) e)
        {
            _storyBoard?.Stop();
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
        GameSideTransform.X = 0;
        
        MenuTransform.Y = 0;
        TeamBoardControl.Opacity = 1;
        MenuSideTransform.X = -ParentMenuSideControl.ActualWidth;
        
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
            Duration = TimeSpan.FromMilliseconds(600),
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
            Duration = TimeSpan.FromMilliseconds(600),
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
            Duration = TimeSpan.FromMilliseconds(600),
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
            Duration = TimeSpan.FromMilliseconds(600),
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
            Duration = TimeSpan.FromMilliseconds(600),
            FillBehavior = FillBehavior.Stop
        };
        Storyboard.SetTarget(outerControlOpacityAnimation, GameBoardOuterControl);
        Storyboard.SetTargetProperty(outerControlOpacityAnimation, new PropertyPath("(UIElement.Opacity)"));
        storyBoard.Children.Add(outerControlOpacityAnimation);

        var gameSideTransformAnimation = new DoubleAnimation
        {
            BeginTime = TimeSpan.Zero,
            From = 0,
            To = ParentGameSideControl.ActualWidth,
            Duration = TimeSpan.FromMilliseconds(600),
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut },
            FillBehavior = FillBehavior.Stop
        };
        Storyboard.SetTarget(gameSideTransformAnimation, GameSideControl);
        Storyboard.SetTargetProperty(gameSideTransformAnimation, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.X)"));
        storyBoard.Children.Add(gameSideTransformAnimation);

        var menuSideTransformAnimation = new DoubleAnimation
        {
            BeginTime = TimeSpan.Zero,
            From = -ParentMenuSideControl.ActualWidth,
            To = 0,
            Duration = TimeSpan.FromMilliseconds(600),
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut },
            FillBehavior = FillBehavior.Stop
        };
        Storyboard.SetTarget(menuSideTransformAnimation, MenuSideControl);
        Storyboard.SetTargetProperty(menuSideTransformAnimation, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.X)"));
        storyBoard.Children.Add(menuSideTransformAnimation);
        
        storyBoard.Completed += StoryBoardOnCompleted;
        storyBoard.Freeze();
        storyBoard.Begin();
        _storyBoard = storyBoard;
        
        void StoryBoardOnCompleted(object? sender, EventArgs e)
        {
            TeamBoardControl.Visibility = Visibility.Visible;
            MenuControl.Visibility = Visibility.Visible;
            MenuSideControl.Visibility = Visibility.Visible;
            GameBoardOuterControl.Visibility = Visibility.Collapsed;
            GameSideControl.Visibility = Visibility.Collapsed;
            
            GameBoardOuterControl.Opacity = 0.85;
            GameBoardInnerTransform.Y = parentHeight / 4;
            GameBoardMiddleTransform.Y = -parentHeight;
            GameBoardOuterTransform.Y = parentHeight * 3 / 4;
            GameSideTransform.X = ParentGameSideControl.ActualWidth;
            
            MenuSideTransform.X = 0;
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
        GameSideTransform.X = ParentGameSideControl.ActualWidth;
        
        MenuTransform.Y = 0;
        TeamBoardControl.Opacity = 0.85;
        MenuSideTransform.X = 0;
        
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
            Duration = TimeSpan.FromMilliseconds(600),
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
            Duration = TimeSpan.FromMilliseconds(600),
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
            Duration = TimeSpan.FromMilliseconds(600),
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
            Duration = TimeSpan.FromMilliseconds(600),
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
            Duration = TimeSpan.FromMilliseconds(600),
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
            To = -ParentMenuSideControl.ActualWidth,
            Duration = TimeSpan.FromMilliseconds(600),
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut },
            FillBehavior = FillBehavior.Stop
        };
        Storyboard.SetTarget(menuSideTransformAnimation, MenuSideControl);
        Storyboard.SetTargetProperty(menuSideTransformAnimation, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.X)"));
        storyBoard.Children.Add(menuSideTransformAnimation);

        var gameSideTransformAnimation = new DoubleAnimation
        {
            BeginTime = TimeSpan.Zero,
            From = ParentGameSideControl.ActualWidth,
            To = 0,
            Duration = TimeSpan.FromMilliseconds(600),
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut },
            FillBehavior = FillBehavior.Stop
        };
        Storyboard.SetTarget(gameSideTransformAnimation, GameSideControl);
        Storyboard.SetTargetProperty(gameSideTransformAnimation, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.X)"));
        storyBoard.Children.Add(gameSideTransformAnimation);
        
        storyBoard.Completed += StoryBoardOnCompleted;
        storyBoard.Freeze();
        storyBoard.Begin();
        _storyBoard = storyBoard;
        
        void StoryBoardOnCompleted(object? sender, EventArgs e)
        {
            GameBoardOuterControl.Visibility = Visibility.Visible;
            GameSideControl.Visibility = Visibility.Visible;
            MenuControl.Visibility = Visibility.Collapsed;
            TeamBoardControl.Visibility = Visibility.Collapsed;
            MenuSideControl.Visibility = Visibility.Collapsed;
            
            GameBoardOuterControl.Opacity = 1;
            GameBoardInnerTransform.Y = 0;
            GameBoardMiddleTransform.Y = 0;
            GameBoardOuterTransform.Y = 0;
            GameSideTransform.X = 0;
            
            MenuTransform.Y = -parentHeight * 3 / 4;
            TeamBoardControl.Opacity = 0.85;
            MenuSideTransform.X = -ParentMenuSideControl.ActualWidth;
        }
    }
    
    private void MenuToEditorTabAnimation()
    {
        var parentHeight = ParentGameBoardBorder.ActualHeight;
        EditorUnitsInnerTransform.Y = -parentHeight * 3 / 4;
        EditorUnitsOuterTransform.Y = parentHeight * 3 / 4;
        EditorSideTransform.X = ParentEditorSideControl.ActualWidth;
        
        MenuTransform.Y = 0;
        TeamBoardControl.Opacity = 0.85;
        MenuSideTransform.X = 0;
        
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
            Duration = TimeSpan.FromMilliseconds(600),
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
            Duration = TimeSpan.FromMilliseconds(600),
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
            Duration = TimeSpan.FromMilliseconds(600),
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
            Duration = TimeSpan.FromMilliseconds(600),
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
            To = -ParentMenuSideControl.ActualWidth,
            Duration = TimeSpan.FromMilliseconds(600),
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut },
            FillBehavior = FillBehavior.Stop
        };
        Storyboard.SetTarget(menuSideTransformAnimation, MenuSideControl);
        Storyboard.SetTargetProperty(menuSideTransformAnimation, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.X)"));
        storyBoard.Children.Add(menuSideTransformAnimation);

        var editorSideTransformAnimation = new DoubleAnimation
        {
            BeginTime = TimeSpan.Zero,
            From = ParentEditorSideControl.ActualWidth,
            To = 0,
            Duration = TimeSpan.FromMilliseconds(600),
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut },
            FillBehavior = FillBehavior.Stop
        };
        Storyboard.SetTarget(editorSideTransformAnimation, EditorSideControl);
        Storyboard.SetTargetProperty(editorSideTransformAnimation, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.X)"));
        storyBoard.Children.Add(editorSideTransformAnimation);
        
        storyBoard.Completed += StoryBoardOnCompleted;
        storyBoard.Freeze();
        storyBoard.Begin();
        _storyBoard = storyBoard;
        
        void StoryBoardOnCompleted(object? sender, EventArgs e)
        {
            EditorUnitsOuterControl.Visibility = Visibility.Visible;
            EditorSideControl.Visibility = Visibility.Visible;
            TeamBoardControl.Visibility = Visibility.Visible;
            MenuControl.Visibility = Visibility.Collapsed;
            MenuSideControl.Visibility = Visibility.Collapsed;

            EditorUnitsInnerTransform.Y = 0;
            EditorUnitsOuterTransform.Y = 0;
            EditorSideTransform.X = 0;
            
            MenuTransform.Y = -parentHeight * 3 / 4;
            TeamBoardControl.Opacity = 1;
            MenuSideTransform.X = -ParentMenuSideControl.ActualWidth;
        }
    }
    
    private void EditorToMenuTabAnimation()
    {
        var parentHeight = ParentGameBoardBorder.ActualHeight;
        EditorUnitsInnerTransform.Y = 0;
        EditorUnitsOuterTransform.Y = 0;
        EditorSideTransform.X = 0;
        
        MenuTransform.Y = -parentHeight * 3 / 4;
        TeamBoardControl.Opacity = 1;
        MenuSideTransform.X = -ParentMenuSideControl.ActualWidth;
        
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
            Duration = TimeSpan.FromMilliseconds(600),
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
            Duration = TimeSpan.FromMilliseconds(600),
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
            Duration = TimeSpan.FromMilliseconds(600),
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
            Duration = TimeSpan.FromMilliseconds(600),
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
            From = -ParentMenuSideControl.ActualWidth,
            Duration = TimeSpan.FromMilliseconds(600),
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut },
            FillBehavior = FillBehavior.Stop
        };
        Storyboard.SetTarget(menuSideTransformAnimation, MenuSideControl);
        Storyboard.SetTargetProperty(menuSideTransformAnimation, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.X)"));
        storyBoard.Children.Add(menuSideTransformAnimation);

        var editorSideTransformAnimation = new DoubleAnimation
        {
            BeginTime = TimeSpan.Zero,
            To = ParentEditorSideControl.ActualWidth,
            From = 0,
            Duration = TimeSpan.FromMilliseconds(600),
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut },
            FillBehavior = FillBehavior.Stop
        };
        Storyboard.SetTarget(editorSideTransformAnimation, EditorSideControl);
        Storyboard.SetTargetProperty(editorSideTransformAnimation, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.X)"));
        storyBoard.Children.Add(editorSideTransformAnimation);
        
        storyBoard.Completed += StoryBoardOnCompleted;
        storyBoard.Freeze();
        storyBoard.Begin();
        _storyBoard = storyBoard;
        
        void StoryBoardOnCompleted(object? sender, EventArgs e)
        {
            EditorUnitsOuterControl.Visibility = Visibility.Collapsed;
            EditorSideControl.Visibility = Visibility.Collapsed;
            TeamBoardControl.Visibility = Visibility.Visible;
            MenuControl.Visibility = Visibility.Visible;
            MenuSideControl.Visibility = Visibility.Visible;

            EditorUnitsInnerTransform.Y = -parentHeight * 3 / 4;
            EditorUnitsOuterTransform.Y = parentHeight * 3 / 4;
            EditorSideTransform.X = ParentEditorSideControl.ActualWidth;
            
            MenuTransform.Y = 0;
            TeamBoardControl.Opacity = 0.95;
            MenuSideTransform.X = 0;
        }
    }
}
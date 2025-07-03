using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using BattleChess3.Game.Figures;
using BattleChess3.UI.ViewModel;
using Nicenis.Windows;

namespace BattleChess3.UI.Views;

/// <summary>
///     Interaction logic for EditorControl.xaml
/// </summary>
public partial class EditorControl
{
    public EditorControl()
    {
        InitializeComponent();
    }

    private void FigureButton_GotFocus(object sender, RoutedEventArgs e)
    {
        var button = (Button)sender;
        var stackPanel = FindAncestor<StackPanel>(button);
        var boardViewModel = (EditorViewModel)stackPanel.DataContext;
        boardViewModel.GotFocusCommand.Execute(button.CommandParameter);
    }

    private void FigureButton_LostFocus(object sender, RoutedEventArgs e)
    {
        var button = (Button)sender;
        var stackPanel = FindAncestor<StackPanel>(button);
        var boardViewModel = (EditorViewModel)stackPanel.DataContext;
        boardViewModel.LostFocusCommand.Execute(button.CommandParameter);
    }

    private void FigureButton_MouseEnter(object sender, MouseEventArgs e)
    {
        var button = (Button)sender;
        var stackPanel = FindAncestor<StackPanel>(button);
        var boardViewModel = (EditorViewModel)stackPanel.DataContext;
        boardViewModel.MouseEnterCommand.Execute(button.CommandParameter);
    }

    private void FigureButton_MouseLeave(object sender, MouseEventArgs e)
    {
        var button = (Button)sender;
        var stackPanel = FindAncestor<StackPanel>(button);
        var boardViewModel = (EditorViewModel)stackPanel.DataContext;
        boardViewModel.MouseExitCommand.Execute(button.CommandParameter);
    }

    private void ChessImage_Drop(object sender, DragEventArgs e)
    {
        if (!e.Data.GetDataPresent("BattleChess3.Game.Figures.FigureIdentifier")) 
            return;
        
        var figureIdentifier = (FigureIdentifier)e.Data.GetData("BattleChess3.Game.Figures.FigureIdentifier");
        var tileButton = (Button)sender;
        var tileViewModel = (TileViewModel)tileButton.DataContext;

        var itemsControl = FindAncestor<ItemsControl>((DependencyObject)e.OriginalSource);
        var boardView = (BoardViewModel)itemsControl.DataContext;

        boardView.CreateFigure(tileViewModel, figureIdentifier);
    }

    private void ChessImage_DragEnter(object sender, DragSourceDraggingEventArgs e)
    {
        var image = (Image)sender;
        var imagePair = (KeyValuePair<int, Uri>)image.DataContext;
        var itemsControl = FindAncestor<ItemsControl>((DependencyObject)e.OriginalSource);

        var figureType = (IFigureType)itemsControl.DataContext;
        
        e.Data = new FigureIdentifier(imagePair.Key, figureType, false);
    }

    private static T? FindAncestor<T>(DependencyObject parent)
        where T : DependencyObject
    {
        var current = parent;
        do
        {
            if (current is T dependencyObject)
            {
                return dependencyObject;
            }
            
            current = VisualTreeHelper.GetParent(current);
        } while (current != null);

        return default;
    }
}
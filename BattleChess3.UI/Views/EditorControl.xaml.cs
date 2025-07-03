using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using BattleChess3.Game.Figures;
using BattleChess3.UI.ViewModel;

namespace BattleChess3.UI.Views;

/// <summary>
///     Interaction logic for EditorControl.xaml
/// </summary>
public partial class EditorControl
{
    private Point _startPoint;

    public EditorControl()
    {
        InitializeComponent();
    }

    private void FigureButton_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        // Store the mouse position
        _startPoint = e.GetPosition(null);
    }

    private void FigureButton_PreviewMouseMove(object sender, MouseEventArgs e)
    {
        // Get the current mouse position
        var mousePos = e.GetPosition(null);
        var diff = _startPoint - mousePos;

        if (e.LeftButton == MouseButtonState.Pressed &&
            (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
             Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance))
        {
            // Get the dragged ListViewItem
            var button = (Button)sender;
            var itemsControl = FindAncestor<ItemsControl>((DependencyObject)e.OriginalSource);

            //// Find the data behind the ListViewItem
            var figureType = (IFigureType)itemsControl.DataContext;
            var imagePair = (KeyValuePair<int, Uri>)button.DataContext;
            var dataObject = new DataObject("figureData", new FigureIdentifier(imagePair.Key, figureType));

            //// Initialize the drag & drop operation
            DragDrop.DoDragDrop(button, dataObject, DragDropEffects.Move);
        }
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

    private void ChessButton_MouseEnter(object sender, MouseEventArgs e)
    {
        var button = (Button)sender;
        var itemsControl = FindAncestor<ItemsControl>(button);
        var boardViewModel = (BoardViewModel)itemsControl.DataContext;
        boardViewModel.MouseEnterCommand.Execute(button.CommandParameter);
    }

    private void ChessButton_MouseLeave(object sender, MouseEventArgs e)
    {
        var button = (Button)sender;
        var itemsControl = FindAncestor<ItemsControl>(button);
        var boardViewModel = (BoardViewModel)itemsControl.DataContext;
        boardViewModel.MouseExitCommand.Execute(button.CommandParameter);
    }

    private void ChessImage_Drop(object sender, DragEventArgs e)
    {
        if (!e.Data.GetDataPresent("figureData")) 
            return;
        
        var figureBlueprint = (FigureIdentifier)e.Data.GetData("figureData");
        var image = (Image)sender;
        var tileViewModel = (TileViewModel)image.DataContext;

        var itemsControl = FindAncestor<ItemsControl>((DependencyObject)e.OriginalSource);
        var boardView = (BoardViewModel)itemsControl.DataContext;

        boardView.CreateFigure(tileViewModel, figureBlueprint);
    }

    private void ChessImage_DragEnter(object sender, DragEventArgs e)
    {
        if (!e.Data.GetDataPresent("figureData") ||
            sender == e.Source)
        {
            e.Effects = DragDropEffects.Copy;
        }
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
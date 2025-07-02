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

    private void Image_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        // Store the mouse position
        _startPoint = e.GetPosition(null);
    }

    private void Image_PreviewMouseMove(object sender, MouseEventArgs e)
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

    private void Button_GotFocus(object sender, RoutedEventArgs e)
    {
        var button = (Button)sender;
        var stackPanel = FindAncestor<StackPanel>(button);
        var boardViewModel = (FiguresViewModel)stackPanel.DataContext;
        boardViewModel.GotFocusCommand.Execute(button.CommandParameter);
    }

    private void Button_LostFocus(object sender, RoutedEventArgs e)
    {
        var button = (Button)sender;
        var stackPanel = FindAncestor<StackPanel>(button);
        var boardViewModel = (FiguresViewModel)stackPanel.DataContext;
        boardViewModel.LostFocusCommand.Execute(button.CommandParameter);
    }

    private void Button_MouseEnter(object sender, MouseEventArgs e)
    {
        var button = (Button)sender;
        var stackPanel = FindAncestor<StackPanel>(button);
        var boardViewModel = (FiguresViewModel)stackPanel.DataContext;
        boardViewModel.MouseEnterCommand.Execute(button.CommandParameter);
    }

    private void Button_MouseLeave(object sender, MouseEventArgs e)
    {
        var button = (Button)sender;
        var stackPanel = FindAncestor<StackPanel>(button);
        var boardViewModel = (FiguresViewModel)stackPanel.DataContext;
        boardViewModel.MouseExitCommand.Execute(button.CommandParameter);
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
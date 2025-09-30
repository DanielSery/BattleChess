using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using CrownsGuard.Core.Figures;
using CrownsGuard.Game.Players;
using CrownsGuard.UI.Shared;
using Nicenis.Windows;

namespace CrownsGuard.UI.Editor;

public partial class EditorUnitsControl
{
    public EditorUnitsControl()
    {
        InitializeComponent();
    }

    private void FigureButton_GotFocus(object sender, RoutedEventArgs e)
    {
        var button = (Button)sender;
        var stackPanel = FindAncestor<Grid>(button);
        var editorViewModel = (EditorUnitsViewModel)stackPanel!.DataContext;
        editorViewModel.FigureGotFocusCommand.Execute(button.CommandParameter);
    }

    private void FigureButton_LostFocus(object sender, RoutedEventArgs e)
    {
        var button = (Button)sender;
        var stackPanel = FindAncestor<Grid>(button);
        var editorViewModel = (EditorUnitsViewModel)stackPanel!.DataContext;
        editorViewModel.FigureLostFocusCommand.Execute(button.CommandParameter);
    }

    private void FigureButton_MouseEnter(object sender, MouseEventArgs e)
    {
        var button = (Button)sender;
        var stackPanel = FindAncestor<Grid>(button);
        var editorViewModel = (EditorUnitsViewModel)stackPanel!.DataContext;
        editorViewModel.FigureMouseEnterCommand.Execute(button.CommandParameter);
    }

    private void FigureButton_MouseLeave(object sender, MouseEventArgs e)
    {
        var button = (Button)sender;
        var stackPanel = FindAncestor<Grid>(button);
        var editorViewModel = (EditorUnitsViewModel)stackPanel!.DataContext;
        editorViewModel.FigureMouseExitCommand.Execute(button.CommandParameter);
    }

    private void Figures_Drop(object sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent(typeof((TeamBoardViewModel, TileInfoViewModel)).FullName))
        {
            var (teamBoard, sourceTile) =  ((TeamBoardViewModel, TileInfoViewModel))e.Data.GetData(typeof((TeamBoardViewModel, TileInfoViewModel)).FullName);
            teamBoard.CreateFigure(sourceTile, Figure.Empty);
        }
    }

    private void ChessImage_DragEnter(object sender, DragSourceDraggingEventArgs e)
    {
        var button = (Button)sender;
        var figureType = (FigureTypeViewModel)button.DataContext;

        if (figureType.PlayerColor == PlayerColor.White)
            e.Data = figureType.Figure | Figure.IsWhite;
        else if (figureType.PlayerColor == PlayerColor.Black)
            e.Data = figureType.Figure | Figure.IsBlack;
        else e.Data = figureType.Figure;
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

        return null;
    }
}
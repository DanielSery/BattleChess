using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using CrownsGuard.Core.Figures;
using CrownsGuard.UI.Shared;
using Nicenis.Windows;

namespace CrownsGuard.UI.Editor;

public partial class TeamBoardControl
{
    public TeamBoardControl()
    {
        InitializeComponent();
    }

    private void ChessImage_Drop(object sender, DragEventArgs e)
    {
        Focus();
        if (e.Data.GetDataPresent(typeof(Figure).FullName))
        {
            var figureIdentifier = (Figure)e.Data.GetData(typeof(Figure).FullName);
            var tileButton = (Button)sender;
            var targetTile = (TileInfoViewModel)tileButton.DataContext;

            var itemsControl = FindAncestor<ItemsControl>((DependencyObject)sender);
            var teamBoard = (TeamBoardViewModel)itemsControl.DataContext;

            teamBoard.CreateFigure(targetTile, figureIdentifier);
        }

        else if (e.Data.GetDataPresent(typeof((TeamBoardViewModel, TileInfoViewModel)).FullName))
        {
            var (teamBoard, sourceTile) = ((TeamBoardViewModel, TileInfoViewModel))e.Data.GetData(typeof((TeamBoardViewModel, TileInfoViewModel)).FullName);
            var sourceFigureIdentifier = new Figure(sourceTile.Figure.Owner.PlayerColor,
                sourceTile.Figure.IsKing, sourceTile.Figure.TypeInfo.FigureId);

            var tileButton = (Button)sender;
            var targetTile = (TileInfoViewModel)tileButton.DataContext;
            var targetFigureIdentifier = new Figure(targetTile.Figure.Owner.PlayerColor,
                targetTile.Figure.IsKing, targetTile.Figure.TypeInfo.FigureId);

            teamBoard.CreateFigure(sourceTile, targetFigureIdentifier);
            teamBoard.CreateFigure(targetTile, sourceFigureIdentifier);
        }
    }

    private void ChessButton_DragEnter(object sender, DragSourceDraggingEventArgs e)
    {
        var button = (Button)sender;
        var tileViewModel = (TileInfoViewModel)button.DataContext;

        var itemsControl = FindAncestor<ItemsControl>((DependencyObject)sender);
        var teamBoard = (TeamBoardViewModel)itemsControl.DataContext;

        e.Data = (teamBoard, tileViewModel);
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

    private void TileButton_MouseEnter(object sender, MouseEventArgs e)
    {
        var button = (Button)sender;
        var stackPanel = FindAncestor<TeamBoardControl>(button);
        var editorViewModel = (EditorViewModel)stackPanel.DataContext;
        editorViewModel.EditorUnits.TileMouseEnterCommand.Execute(button.CommandParameter);
    }

    private void TileButton_MouseLeave(object sender, MouseEventArgs e)
    {
        var button = (Button)sender;
        var stackPanel = FindAncestor<TeamBoardControl>(button);
        var editorViewModel = (EditorViewModel)stackPanel.DataContext;
        editorViewModel.EditorUnits.TileMouseExitCommand.Execute(button.CommandParameter);
    }
}
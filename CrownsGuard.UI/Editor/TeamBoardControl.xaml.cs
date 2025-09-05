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
        if (e.Data.GetDataPresent("CrownsGuard.Game.Figures.FigureBlueprint"))
        {
            var figureIdentifier = (FigureBlueprint)e.Data.GetData("CrownsGuard.Game.Figures.FigureBlueprint");
            var tileButton = (Button)sender;
            var targetTile = (TileViewModel)tileButton.DataContext;

            var itemsControl = FindAncestor<ItemsControl>((DependencyObject)sender);
            var teamBoard = (TeamBoardViewModel)itemsControl.DataContext;

            teamBoard.CreateFigure(targetTile, figureIdentifier);
        }
        else if (e.Data.GetDataPresent(
                     "System.ValueTuple`2[[CrownsGuard.UI.Editor.TeamBoardViewModel, CrownsGuard, Version=4.0.2.0, Culture=neutral, PublicKeyToken=null],[CrownsGuard.UI.Shared.TileViewModel, CrownsGuard, Version=4.0.2.0, Culture=neutral, PublicKeyToken=null]]"))
        {
            var (teamBoard, sourceTile) = ((TeamBoardViewModel, TileViewModel))e.Data.GetData(
                "System.ValueTuple`2[[CrownsGuard.UI.Editor.TeamBoardViewModel, CrownsGuard, Version=4.0.2.0, Culture=neutral, PublicKeyToken=null],[CrownsGuard.UI.Shared.TileViewModel, CrownsGuard, Version=4.0.2.0, Culture=neutral, PublicKeyToken=null]]");
            var sourceFigureIdentifier = new FigureBlueprint(sourceTile.Figure.Owner.Player,
                sourceTile.Figure.Type.FigureId, sourceTile.Figure.IsKing);

            var tileButton = (Button)sender;
            var targetTile = (TileViewModel)tileButton.DataContext;
            var targetFigureIdentifier = new FigureBlueprint(targetTile.Figure.Owner.Player,
                targetTile.Figure.Type.FigureId, targetTile.Figure.IsKing);

            teamBoard.CreateFigure(sourceTile, targetFigureIdentifier);
            teamBoard.CreateFigure(targetTile, sourceFigureIdentifier);
        }
    }

    private void ChessButton_DragEnter(object sender, DragSourceDraggingEventArgs e)
    {
        var button = (Button)sender;
        var tileViewModel = (TileViewModel)button.DataContext;

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
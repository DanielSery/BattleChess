using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using CrownsGuard.Core.Figures;
using CrownsGuard.FigureDefinitions;
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
        var editorViewModel = (EditorUnitsViewModel)stackPanel.DataContext;
        editorViewModel.FigureGotFocusCommand.Execute(button.CommandParameter);
    }

    private void FigureButton_LostFocus(object sender, RoutedEventArgs e)
    {
        var button = (Button)sender;
        var stackPanel = FindAncestor<Grid>(button);
        var editorViewModel = (EditorUnitsViewModel)stackPanel.DataContext;
        editorViewModel.FigureLostFocusCommand.Execute(button.CommandParameter);
    }

    private void FigureButton_MouseEnter(object sender, MouseEventArgs e)
    {
        var button = (Button)sender;
        var stackPanel = FindAncestor<Grid>(button);
        var editorViewModel = (EditorUnitsViewModel)stackPanel.DataContext;
        editorViewModel.FigureMouseEnterCommand.Execute(button.CommandParameter);
    }

    private void FigureButton_MouseLeave(object sender, MouseEventArgs e)
    {
        var button = (Button)sender;
        var stackPanel = FindAncestor<Grid>(button);
        var editorViewModel = (EditorUnitsViewModel)stackPanel.DataContext;
        editorViewModel.FigureMouseExitCommand.Execute(button.CommandParameter);
    }

    private void Figures_Drop(object sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent("System.ValueTuple`2[[CrownsGuard.UI.Editor.TeamBoardViewModel, CrownsGuard, Version=4.0.2.0, Culture=neutral, PublicKeyToken=null],[CrownsGuard.UI.Shared.TileViewModel, CrownsGuard, Version=4.0.2.0, Culture=neutral, PublicKeyToken=null]]"))
        {
            var (teamBoard, sourceTile) =  ((TeamBoardViewModel, TileViewModel))e.Data.GetData("System.ValueTuple`2[[CrownsGuard.UI.Editor.TeamBoardViewModel, CrownsGuard, Version=4.0.2.0, Culture=neutral, PublicKeyToken=null],[CrownsGuard.UI.Shared.TileViewModel, CrownsGuard, Version=4.0.2.0, Culture=neutral, PublicKeyToken=null]]");

            teamBoard.CreateFigure(sourceTile, new FigureBlueprint(0, CrossFireFigureIds.EmptyId, false));
        }
    }

    private void ChessImage_DragEnter(object sender, DragSourceDraggingEventArgs e)
    {
        var button = (Button)sender;
        var figureType = (FigureTypeViewModel)button.DataContext;
        e.Data = new FigureBlueprint(figureType.Player, figureType.FigureId, false);
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
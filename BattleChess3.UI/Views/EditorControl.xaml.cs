using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using BattleChess3.CrossFireFigures;
using BattleChess3.Game.Figures;
using BattleChess3.Game.Players;
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
        var stackPanel = FindAncestor<Grid>(button);
        var editorViewModel = (EditorViewModel)stackPanel.DataContext;
        editorViewModel.FigureGotFocusCommand.Execute(button.CommandParameter);
    }

    private void FigureButton_LostFocus(object sender, RoutedEventArgs e)
    {
        var button = (Button)sender;
        var stackPanel = FindAncestor<Grid>(button);
        var editorViewModel = (EditorViewModel)stackPanel.DataContext;
        editorViewModel.FigureLostFocusCommand.Execute(button.CommandParameter);
    }

    private void FigureButton_MouseEnter(object sender, MouseEventArgs e)
    {
        var button = (Button)sender;
        var stackPanel = FindAncestor<Grid>(button);
        var editorViewModel = (EditorViewModel)stackPanel.DataContext;
        editorViewModel.FigureMouseEnterCommand.Execute(button.CommandParameter);
    }

    private void FigureButton_MouseLeave(object sender, MouseEventArgs e)
    {
        var button = (Button)sender;
        var stackPanel = FindAncestor<Grid>(button);
        var editorViewModel = (EditorViewModel)stackPanel.DataContext;
        editorViewModel.FigureMouseExitCommand.Execute(button.CommandParameter);
    }

    private void ChessImage_Drop(object sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent("BattleChess3.Game.Figures.FigureIdentifier"))
        {
            var figureIdentifier = (FigureIdentifier)e.Data.GetData("BattleChess3.Game.Figures.FigureIdentifier");
            var tileButton = (Button)sender;
            var targetTile = (TileViewModel)tileButton.DataContext;

            var itemsControl = FindAncestor<ItemsControl>((DependencyObject)sender);
            var editorViewModel = (EditorViewModel)itemsControl.DataContext;

            editorViewModel.CreateFigure(targetTile, figureIdentifier);
        }
        else if (e.Data.GetDataPresent("System.ValueTuple`2[[BattleChess3.UI.ViewModel.EditorViewModel, BattleChess3, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null],[BattleChess3.UI.ViewModel.TileViewModel, BattleChess3, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]]"))
        {
            var (editorViewModel, sourceTile) =  ((EditorViewModel, TileViewModel))e.Data.GetData("System.ValueTuple`2[[BattleChess3.UI.ViewModel.EditorViewModel, BattleChess3, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null],[BattleChess3.UI.ViewModel.TileViewModel, BattleChess3, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]]");
            var sourceFigureIdentifier = new FigureIdentifier(sourceTile.Figure.Owner.Id, sourceTile.Figure.Type, sourceTile.Figure.IsKing);
            
            var tileButton = (Button)sender;
            var targetTile = (TileViewModel)tileButton.DataContext;
            var targetFigureIdentifier = new FigureIdentifier(targetTile.Figure.Owner.Id, targetTile.Figure.Type, targetTile.Figure.IsKing);
            
            editorViewModel.CreateFigure(sourceTile, targetFigureIdentifier);
            editorViewModel.CreateFigure(targetTile, sourceFigureIdentifier);
        }
    }

    private void Figures_Drop(object sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent("System.ValueTuple`2[[BattleChess3.UI.ViewModel.EditorViewModel, BattleChess3, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null],[BattleChess3.UI.ViewModel.TileViewModel, BattleChess3, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]]"))
        {
            var (editorViewModel, sourceTile) =  ((EditorViewModel, TileViewModel))e.Data.GetData("System.ValueTuple`2[[BattleChess3.UI.ViewModel.EditorViewModel, BattleChess3, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null],[BattleChess3.UI.ViewModel.TileViewModel, BattleChess3, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]]");

            editorViewModel.CreateFigure(sourceTile, new FigureIdentifier(0, CrossFireFigureGroup.Empty, false));
        }
    }

    private void ChessButton_DragEnter(object sender, DragSourceDraggingEventArgs e)
    {
        var button = (Button)sender;
        var tileViewModel = (TileViewModel)button.DataContext;
        
        var itemsControl = FindAncestor<ItemsControl>((DependencyObject)sender);
        var editorViewModel = (EditorViewModel)itemsControl.DataContext;
        
        e.Data = (editorViewModel, tileViewModel);
    }

    private void ChessImage_DragEnter(object sender, DragSourceDraggingEventArgs e)
    {
        var button = (Button)sender;
        var figureType = (FigureTypeViewModel)button.DataContext;
        e.Data = new FigureIdentifier(figureType.PlayerId, figureType.UniqueFigureId, false);
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
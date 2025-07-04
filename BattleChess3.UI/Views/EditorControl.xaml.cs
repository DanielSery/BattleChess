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
        if (e.Data.GetDataPresent("BattleChess3.Game.Figures.FigureIdentifier"))
        {
            var figureIdentifier = (FigureIdentifier)e.Data.GetData("BattleChess3.Game.Figures.FigureIdentifier");
            var tileButton = (Button)sender;
            var targetTile = (TileViewModel)tileButton.DataContext;

            var itemsControl = FindAncestor<ItemsControl>((DependencyObject)sender);
            var boardViewModel = (BoardViewModel)itemsControl.DataContext;

            boardViewModel.CreateFigure(targetTile, figureIdentifier);
        }
        else if (e.Data.GetDataPresent("System.ValueTuple`2[[BattleChess3.UI.ViewModel.BoardViewModel, BattleChess3, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null],[BattleChess3.UI.ViewModel.TileViewModel, BattleChess3, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]]"))
        {
            var (boardViewModel, sourceTile) =  ((BoardViewModel, TileViewModel))e.Data.GetData("System.ValueTuple`2[[BattleChess3.UI.ViewModel.BoardViewModel, BattleChess3, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null],[BattleChess3.UI.ViewModel.TileViewModel, BattleChess3, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]]");
            var sourceFigureIdentifier = new FigureIdentifier(sourceTile.Figure.Owner.Id, sourceTile.Figure.Type, sourceTile.Figure.IsKing);
            
            var tileButton = (Button)sender;
            var targetTile = (TileViewModel)tileButton.DataContext;
            var targetFigureIdentifier = new FigureIdentifier(targetTile.Figure.Owner.Id, targetTile.Figure.Type, targetTile.Figure.IsKing);
            
            boardViewModel.CreateFigure(sourceTile, targetFigureIdentifier);
            boardViewModel.CreateFigure(targetTile, sourceFigureIdentifier);
        }
    }

    private void Figures_Drop(object sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent("System.ValueTuple`2[[BattleChess3.UI.ViewModel.BoardViewModel, BattleChess3, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null],[BattleChess3.UI.ViewModel.TileViewModel, BattleChess3, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]]"))
        {
            var (boardViewModel, sourceTile) =  ((BoardViewModel, TileViewModel))e.Data.GetData("System.ValueTuple`2[[BattleChess3.UI.ViewModel.BoardViewModel, BattleChess3, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null],[BattleChess3.UI.ViewModel.TileViewModel, BattleChess3, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]]");

            boardViewModel.CreateFigure(sourceTile, new FigureIdentifier(0, CrossFireFigureGroup.Empty, false));
        }
    }

    private void ChessButton_DragEnter(object sender, DragSourceDraggingEventArgs e)
    {
        var button = (Button)sender;
        var tileViewModel = (TileViewModel)button.DataContext;
        
        var itemsControl = FindAncestor<ItemsControl>((DependencyObject)sender);
        var boardViewModel = (BoardViewModel)itemsControl.DataContext;
        
        e.Data = (boardViewModel, tileViewModel);
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
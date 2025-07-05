using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
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

        Loaded += MainWindow_Loaded;
        DataContextChanged += MainWindow_DataContextChanged;
    }

    public EditorViewModel? ViewModel { get; private set; }

    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        if (DataContext is EditorViewModel viewModel)
        {
            ViewModel = viewModel;
            ViewModel.RequestSavePreview += ViewModel_RequestSavePreview;
        }
    }

    private void MainWindow_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (ViewModel is not null)
        {
            ViewModel.RequestSavePreview -= ViewModel_RequestSavePreview;
        }

        if (DataContext is EditorViewModel viewModel)
        {
            ViewModel = viewModel;
            ViewModel.RequestSavePreview += ViewModel_RequestSavePreview;
        }
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

    private void ViewModel_RequestSavePreview(object? sender, string identifier)
    {
        SaveBoardPreview($"Resources\\Maps\\{identifier}.png");
    }

    public void SaveBoardPreview(string fileName)
    {
        var dpi = VisualTreeHelper.GetDpi(ThisBoard);
        var bmp = new RenderTargetBitmap(
            (int)ThisBoard.ActualWidth,
            (int)ThisBoard.ActualHeight,
            dpi.PixelsPerInchX / dpi.DpiScaleX,
            dpi.PixelsPerInchY / dpi.DpiScaleY,
            PixelFormats.Pbgra32);

        bmp.Render(ThisBoard);

        var encoder = new PngBitmapEncoder();
        var frame = BitmapFrame.Create(bmp);
        encoder.Frames.Add(frame);

        using var stream = File.Create(fileName);
        encoder.Save(stream);
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
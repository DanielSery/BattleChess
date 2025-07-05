using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using BattleChess3.Game.Figures;
using BattleChess3.UI.ViewModel;
using Nicenis.Windows;

namespace BattleChess3.UI.Views;

public partial class TeamBoardControl
{
    public TeamBoardControl()
    {
        InitializeComponent();

        Loaded += TeamBoardControl_Loaded;
        DataContextChanged += TeamBoardControl_DataContextChanged;
    }

    public TeamBoardViewModel? ViewModel { get; private set; }

    private void TeamBoardControl_Loaded(object sender, RoutedEventArgs e)
    {
        if (DataContext is TeamBoardViewModel viewModel)
        {
            ViewModel = viewModel;
            ViewModel.RequestSavePreview += ViewModel_RequestSavePreview;
        }
    }

    private void TeamBoardControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (ViewModel is not null)
        {
            ViewModel.RequestSavePreview -= ViewModel_RequestSavePreview;
        }

        if (DataContext is TeamBoardViewModel viewModel)
        {
            ViewModel = viewModel;
            ViewModel.RequestSavePreview += ViewModel_RequestSavePreview;
        }
    }

    private void ChessImage_Drop(object sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent("BattleChess3.Game.Figures.FigureIdentifier"))
        {
            var figureIdentifier = (FigureIdentifier)e.Data.GetData("BattleChess3.Game.Figures.FigureIdentifier");
            var tileButton = (Button)sender;
            var targetTile = (TileViewModel)tileButton.DataContext;

            var itemsControl = FindAncestor<ItemsControl>((DependencyObject)sender);
            var teamBoard = (TeamBoardViewModel)itemsControl.DataContext;

            teamBoard.CreateFigure(targetTile, figureIdentifier);
        }
        else if (e.Data.GetDataPresent("System.ValueTuple`2[[BattleChess3.UI.ViewModel.TeamBoardViewModel, BattleChess3, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null],[BattleChess3.UI.ViewModel.TileViewModel, BattleChess3, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]]"))
        {
            var (teamBoard, sourceTile) =  ((TeamBoardViewModel, TileViewModel))e.Data.GetData("System.ValueTuple`2[[BattleChess3.UI.ViewModel.TeamBoardViewModel, BattleChess3, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null],[BattleChess3.UI.ViewModel.TileViewModel, BattleChess3, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]]");
            var sourceFigureIdentifier = new FigureIdentifier(sourceTile.Figure.Owner.Id, sourceTile.Figure.Type, sourceTile.Figure.IsKing);
            
            var tileButton = (Button)sender;
            var targetTile = (TileViewModel)tileButton.DataContext;
            var targetFigureIdentifier = new FigureIdentifier(targetTile.Figure.Owner.Id, targetTile.Figure.Type, targetTile.Figure.IsKing);
            
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
}
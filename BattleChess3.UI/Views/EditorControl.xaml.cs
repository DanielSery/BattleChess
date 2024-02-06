﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using BattleChess3.Core.Model.Figures;

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
            var image = (Image)sender;
            var itemsControl = FindAnchestor<ItemsControl>((DependencyObject)e.OriginalSource);

            //// Find the data behind the ListViewItem
            var figureType = (IFigureType)itemsControl.DataContext;
            var imagePair = (KeyValuePair<int, Uri>)image.DataContext;
            var dataObject = new DataObject("figureData", new FigureBlueprint(imagePair.Key, figureType));

            //// Initialize the drag & drop operation
            DragDrop.DoDragDrop(image, dataObject, DragDropEffects.Move);
        }
    }

    private static T FindAnchestor<T>(DependencyObject current)
        where T : DependencyObject
    {
        do
        {
            if (current is T) return (T)current;
            current = VisualTreeHelper.GetParent(current);
        } while (current != null);

        return default;
    }
}
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace BattleChess3.UI.Game;

public partial class GameBoardControl
{
    public GameBoardControl()
    {
        InitializeComponent();
    }

    private void Button_MouseEnter(object sender, MouseEventArgs e)
    {
        var button = (BoardTileControl)sender;
        var itemsControl = FindAncestor<ItemsControl>(button);
        var boardViewModel = (BoardViewModel)itemsControl.DataContext;
        boardViewModel.MouseEnterCommand.Execute(button.Button.CommandParameter);
    }

    private void Button_MouseLeave(object sender, MouseEventArgs e)
    {
        var button = (BoardTileControl)sender;
        var itemsControl = FindAncestor<ItemsControl>(button);
        var boardViewModel = (BoardViewModel)itemsControl.DataContext;
        boardViewModel.MouseExitCommand.Execute(button.Button.CommandParameter);
    }

    private static T FindAncestor<T>(DependencyObject parent)
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

        return default!;
    }
}
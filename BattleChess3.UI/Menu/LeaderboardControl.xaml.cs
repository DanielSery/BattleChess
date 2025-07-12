using System.Windows;
using System.Windows.Controls;

namespace BattleChess3.UI.Menu;

public partial class LeaderboardControl : UserControl
{
    public LeaderboardControl()
    {
        InitializeComponent();
        IsVisibleChanged += OnIsVisibleChanged;
    }

    private void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue.Equals(true))
        {
            Focus();
        }
    }
}
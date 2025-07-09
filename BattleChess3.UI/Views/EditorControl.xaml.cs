using System.Windows;

namespace BattleChess3.UI.Views;

/// <summary>
///     Interaction logic for EditorControl.xaml
/// </summary>
public partial class EditorControl
{
    public EditorControl()
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
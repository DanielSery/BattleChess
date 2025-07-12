using System.Windows;

namespace BattleChess3.UI.Editor;

/// <summary>
///     Interaction logic for EditorControl.xaml
/// </summary>
public partial class EditorRightControl
{
    public EditorRightControl()
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
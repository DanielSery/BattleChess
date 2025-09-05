using System.Windows;

namespace CrownsGuard.UI.Editor;

/// <summary>
///     Interaction logic for EditorControl.xaml
/// </summary>
public partial class EditorSideControl
{
    public EditorSideControl()
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
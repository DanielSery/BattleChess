using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace BattleChess3.UI.Shared;

public partial class MainWindow
{
    public MainWindow()
    {
        InitializeComponent();
        TextElement.FontFamilyProperty.OverrideMetadata(
            typeof(TextElement),
            new FrameworkPropertyMetadata(
                new FontFamily("Britannic Bold")));

        TextBlock.FontFamilyProperty.OverrideMetadata(
            typeof(TextBlock),
            new FrameworkPropertyMetadata(
                new FontFamily("Britannic Bold")));
    }

    private void DragButton_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ButtonState == MouseButtonState.Pressed)
        {
            DragMove();
        }   
    }

    private void MinimizeButton_Click(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }

    private void MaximizeButton_Click(object sender, RoutedEventArgs e)
    {
        if (WindowState == WindowState.Normal)
        {
            WindowState = WindowState.Maximized;
            MaximizeButtonTextBlock.Text = "🗗"; // Change to restore icon
        }
        else if (WindowState == WindowState.Maximized)
        {
            WindowState = WindowState.Normal;
            MaximizeButtonTextBlock.Text = "🗖"; // Change to maximize icon
        }
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void Control_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (WindowState == WindowState.Normal)
        {
            WindowState = WindowState.Maximized;
            MaximizeButtonTextBlock.Text = "🗗"; // Change to restore icon
        }
        else if (WindowState == WindowState.Maximized)
        {
            WindowState = WindowState.Normal;
            MaximizeButtonTextBlock.Text = "🗖"; // Change to maximize icon
        }
    }
}
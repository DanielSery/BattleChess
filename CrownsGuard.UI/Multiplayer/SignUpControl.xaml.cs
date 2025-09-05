using System.Windows;
using System.Windows.Controls;

namespace CrownsGuard.UI.Multiplayer;

public partial class SignUpControl : UserControl
{
    public SignUpControl()
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

    private void PasswordBox1_OnPasswordChanged(object sender, RoutedEventArgs e)
    {
        if (DataContext != null)
        {
            ((SignUpViewModel)DataContext).SecurePassword1 = ((PasswordBox)sender).SecurePassword;
        }
    }

    private void PasswordBox2_OnPasswordChanged(object sender, RoutedEventArgs e)
    {
        if (DataContext != null)
        {
            ((SignUpViewModel)DataContext).SecurePassword2 = ((PasswordBox)sender).SecurePassword;
        }
    }
}
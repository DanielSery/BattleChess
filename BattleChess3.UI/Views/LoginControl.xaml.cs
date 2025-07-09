using System.Windows;
using System.Windows.Controls;
using BattleChess3.UI.ViewModel;

namespace BattleChess3.UI.Views;

public partial class LoginControl : UserControl
{
    public LoginControl()
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

    private void PasswordBox_OnPasswordChanged(object sender, RoutedEventArgs e)
    {
        if (DataContext != null)
        {
            ((LoginViewModel)DataContext).SecurePassword = ((PasswordBox)sender).SecurePassword;
        }
    }
}
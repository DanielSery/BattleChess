using System.Windows;
using System.Windows.Controls;
using BattleChess3.UI.ViewModel;

namespace BattleChess3.UI.Views;

public partial class SignUpControl : UserControl
{
    public SignUpControl()
    {
        InitializeComponent();
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
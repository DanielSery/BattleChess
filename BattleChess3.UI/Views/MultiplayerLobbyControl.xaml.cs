using System.Windows;
using System.Windows.Controls;
using BattleChess3.UI.ViewModel;

namespace BattleChess3.UI.Views;

public partial class MultiplayerLobbyControl : UserControl
{
    public MultiplayerLobbyControl()
    {
        InitializeComponent();
    }

    private void PasswordBox_OnPasswordChanged(object sender, RoutedEventArgs e)
    {
        if (DataContext != null)
        {
            ((MultiplayerViewModel)DataContext).SecurePassword = ((PasswordBox)sender).SecurePassword;
        }
    }
}
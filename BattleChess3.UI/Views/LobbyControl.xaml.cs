using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using BattleChess3.Multiplayer.Tables;
using BattleChess3.UI.ViewModel;

namespace BattleChess3.UI.Views;

public partial class MultiplayerLobbyControl : UserControl
{
    public MultiplayerLobbyControl()
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
            ((MultiplayerViewModel)DataContext).SecurePassword = ((PasswordBox)sender).SecurePassword;
        }
    }

    private void Control_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (MyDataGrid.SelectedItem is not PublicLobbyData selectedItem) 
            return;
        
        if (DataContext is MultiplayerViewModel vm && vm.JoinLobbyCommand.CanExecute(selectedItem))
        {
            vm.JoinLobbyCommand.Execute(selectedItem);
        }
    }
}
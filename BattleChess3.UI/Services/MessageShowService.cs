using System.Windows;

namespace BattleChess3.UI.Services;

public class MessageShowService : IMessageShowService
{
    public void ShowMessage(string message)
    {
        Application.Current.Dispatcher.Invoke(() => MessageBox.Show(message));
    }
}
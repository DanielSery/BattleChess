using System.Windows;

namespace BattleChess3.UI.Services;

public class MessageShowService : IMessageShowService
{
    private bool _showingMessages;
    
    public void SetShowingMessages(bool showingMessages)
    {
        _showingMessages = showingMessages;
    }

    public void ShowMessage(string? message)
    {
        if (_showingMessages)
            Application.Current.Dispatcher.Invoke(() => MessageBox.Show(message));
    }
}
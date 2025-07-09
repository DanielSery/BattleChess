namespace BattleChess3.UI.Services;

public interface INotificationService
{
    event EventHandler<ShownMessage> OnShownMessage; 
    
    void SetShowMessages(ShownMessage.MessageType messageTypes);

    void ShowMessage(ShownMessage.MessageType messageType, string? message);
}
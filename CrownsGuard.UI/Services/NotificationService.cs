namespace CrownsGuard.UI.Services;

public class NotificationService : INotificationService
{
    private ShownMessage.MessageType _shownMessageTypes = ShownMessage.MessageType.All;
    
    public event EventHandler<ShownMessage>? OnShownMessage; 
    
    public void SetShowMessages(ShownMessage.MessageType messageTypes)
    {
        _shownMessageTypes = messageTypes;
    }

    public void ShowMessage(ShownMessage.MessageType messageType, string? message)
    {
        if (_shownMessageTypes.HasFlag(messageType))
        {
            OnShownMessage?.Invoke(this, new ShownMessage(messageType, message));
        }
    }
}
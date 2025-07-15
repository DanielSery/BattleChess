namespace BattleChess3.UI.Services;

public class ShownMessage
{
    public ShownMessage(MessageType messageType, string? content)
    {
        Type = messageType;
        Content = content;
    }
    
    public string? Content { get; }
    public MessageType Type { get; }
    
    [Flags]
    public enum MessageType
    {
        None = 0,
        Error = 1,
        Warning = 2,
        Info = 4,
        Success = 8,
        All = Error | Warning | Info | Success
    }
}
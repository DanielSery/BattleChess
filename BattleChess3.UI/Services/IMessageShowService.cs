namespace BattleChess3.UI.Services;

public interface IMessageShowService
{
    void SetShowingMessages(bool showingMessages);
    
    void ShowMessage(string? message);
}
using CommunityToolkit.Mvvm.Input;

namespace BattleChess3.UI.MainWindow;

public interface ILoadingService
{
    bool IsLoading { get; }
    
    string Message { get; set; }
    
    LoadingService.LoadingOperation? CurrentOperation { get; }
    
    RelayCommand CancelCommand { get; }
    
    event EventHandler<bool>? LoadingChanged;
    
    LoadingService.LoadingOperation StartLoadingOperation(string message);
}
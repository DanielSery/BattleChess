using CommunityToolkit.Mvvm.Input;

namespace BattleChess3.UI.Services;

public interface ILoadingService
{
    bool IsLoading { get; }
    
    LoadingService.LoadingOperation? CurrentOperation { get; }
    
    RelayCommand CancelCommand { get; }
    
    event EventHandler<bool>? LoadingChanged;
    
    LoadingService.LoadingOperation StartLoadingOperation();
}
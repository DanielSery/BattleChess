using CommunityToolkit.Mvvm.Input;

namespace CrownsGuard.UI.Services;

public interface ILoadingService
{
    bool IsLoading { get; }
    
    string Message { get; set; }
    
    LoadingService.LoadingOperation? CurrentOperation { get; }
    
    RelayCommand CancelCommand { get; }
    
    event EventHandler<bool>? LoadingChanged;
    
    LoadingService.LoadingOperation StartLoadingOperation(string message);
}
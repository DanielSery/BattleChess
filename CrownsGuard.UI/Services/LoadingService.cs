using CommunityToolkit.Mvvm.Input;
using Nicenis.Windows.ViewModels;

namespace CrownsGuard.UI.Services;

public class LoadingService : ViewModelBase, ILoadingService
{
    private readonly INotificationService _notificationService;

    public LoadingService(INotificationService notificationService)
    {
        _notificationService = notificationService;
        CancelCommand = new RelayCommand(CancelOperation, CanCancelOperation);
    }

    public bool IsLoading => CurrentOperation is not null;

    private string _message = string.Empty;
    public string Message
    {
        get => _message;
        set => SetProperty(ref _message, value);
    }
    
    public LoadingOperation? CurrentOperation { get; private set; }
    
    public RelayCommand CancelCommand { get; private set; }

    public event EventHandler<bool>? LoadingChanged;

    /// <inheritdoc />
    public LoadingOperation StartLoadingOperation(string message)
    {
        Message = message;
        CurrentOperation = new LoadingOperation(this);
        LoadingChanged?.Invoke(this, true);
        return CurrentOperation;
    }

    private bool CanCancelOperation()
    {
        return CurrentOperation is not null;
    }

    private void CancelOperation()
    {
        _notificationService.SetShowMessages(ShownMessage.MessageType.Error);
        CurrentOperation?.CancelOperation();
    }
    
    public sealed class LoadingOperation : IDisposable
    {
        private readonly LoadingService _service;
        private readonly CancellationTokenSource _cts;
        
        public CancellationToken CancellationToken => _cts.Token;

        public string Message
        {
            get => _service.Message;
            set => _service.Message = value;
        }
        
        public LoadingOperation(LoadingService service)
        {
            _service = service;
            _cts = new CancellationTokenSource();
        }

        public void CancelOperation()
        {
            _cts.Cancel();
            _service.CurrentOperation = null;
            _service.CancelCommand.NotifyCanExecuteChanged();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _service.CurrentOperation = null;
            _service.CancelCommand.NotifyCanExecuteChanged();
            _service.LoadingChanged?.Invoke(this, false);
            _service._notificationService.SetShowMessages(ShownMessage.MessageType.All);
        }
    }
}
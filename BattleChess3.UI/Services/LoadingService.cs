using CommunityToolkit.Mvvm.Input;
using Nicenis.Windows.ViewModels;

namespace BattleChess3.UI.Services;

public class LoadingService : ViewModelBase, ILoadingService
{
    private readonly IMessageShowService _messageShowService;

    public LoadingService(IMessageShowService messageShowService)
    {
        _messageShowService = messageShowService;
        CancelCommand = new RelayCommand(CancelOperation, CanCancelOperation);
    }

    public bool IsLoading => CurrentOperation is not null;
    public LoadingOperation? CurrentOperation { get; private set; }
    
    public RelayCommand CancelCommand { get; private set; }
    
    
    public event EventHandler<bool>? LoadingChanged;

    /// <inheritdoc />
    public LoadingOperation StartLoadingOperation()
    {
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
        _messageShowService.SetShowingMessages(false);
        CurrentOperation?.CancelOperation();
    }
    
    public sealed class LoadingOperation : IDisposable
    {
        private readonly LoadingService _service;
        private readonly CancellationTokenSource _cts;
        
        public CancellationToken CancellationToken => _cts.Token;
        
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
            _service._messageShowService.SetShowingMessages(true);
        }
    }
}
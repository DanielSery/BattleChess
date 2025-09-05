using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using CrownsGuard.Multiplayer;
using CrownsGuard.Multiplayer.Players;
using CrownsGuard.Multiplayer.Utilities;
using CrownsGuard.UI.Services;
using CommunityToolkit.Mvvm.Input;
using Nicenis.Windows.ViewModels;

namespace CrownsGuard.UI.Multiplayer;

public class LoginViewModel : ViewModelBase
{
    private readonly IMultiplayerPlayerService _multiplayerPlayerService;
    private readonly INotificationService _notificationService;
    private readonly ILoadingService _loadingService;
    private readonly ISoundService _soundService;
    
    public LoginViewModel(
        IMultiplayerPlayerService multiplayerPlayerService,
        INotificationService notificationService,
        ILoadingService loadingService,
        ISoundService soundService)
    {
        _multiplayerPlayerService = multiplayerPlayerService;
        _notificationService = notificationService;
        _loadingService = loadingService;
        _soundService = soundService;
        
        LoginCommand = new AsyncRelayCommand(LogIn);
        RequestEndCommand = new RelayCommand(CallRequestEnd);
    }

    private bool _isLoggedIn;
    public bool IsLoggedIn
    {
        get => _isLoggedIn;
        set => SetProperty(ref _isLoggedIn, value);
    }

    private string _name =  string.Empty;
    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    public SecureString SecurePassword { get; set; } = new SecureString();
    
    public AsyncRelayCommand LoginCommand { get; }
    public RelayCommand RequestEndCommand { get; }

    public event EventHandler? RequestEndLogin;
    
    private async Task LogIn()
    {
        using var loading = _loadingService.StartLoadingOperation("Logging in");
        var saltResult = await _multiplayerPlayerService.GetUserSaltAsync(Name, loading.CancellationToken);
        if (saltResult.IsFailed)
        {
            _notificationService.ShowMessage(ShownMessage.MessageType.Warning, "Invalid username or password");
            _soundService.PlaySoundEffect(SoundEffectType.Error);
            return;
        }
        
        var hash = HashingHelper.GetHash(SecurePassword, saltResult.Value);
        var result = await _multiplayerPlayerService.TryLoginAsync(Name, hash, loading.CancellationToken);
        if (result.IsFailed)
        {
            _notificationService.ShowMessage(ShownMessage.MessageType.Warning, result.Errors[0].Message);
            _soundService.PlaySoundEffect(SoundEffectType.Error);
        }
        else
        {
            _notificationService.ShowMessage(ShownMessage.MessageType.Success, $"Logged in as {Name}");
            RequestEndLogin?.Invoke(this, EventArgs.Empty);
            IsLoggedIn = true;
        }
    }

    private void CallRequestEnd()
    {
        RequestEndLogin?.Invoke(this, EventArgs.Empty);
    }
}
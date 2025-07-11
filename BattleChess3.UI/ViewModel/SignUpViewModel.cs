using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using BattleChess3.Multiplayer;
using BattleChess3.UI.Services;
using CommunityToolkit.Mvvm.Input;
using Nicenis.Windows.ViewModels;

namespace BattleChess3.UI.ViewModel;

public class SignUpViewModel : ViewModelBase
{
    private readonly LoginViewModel _loginViewModel;
    private readonly IMultiplayerPlayerService _multiplayerPlayerService;
    private readonly INotificationService _notificationService;
    private readonly ILoadingService _loadingService;
    
    public SignUpViewModel(
        LoginViewModel loginViewModel,
        IMultiplayerPlayerService multiplayerPlayerService,
        INotificationService notificationService,
        ILoadingService loadingService)
    {
        _loginViewModel = loginViewModel;
        _multiplayerPlayerService = multiplayerPlayerService;
        _notificationService = notificationService;
        _loadingService = loadingService;
        
        SignUpCommand = new AsyncRelayCommand(SignUp);
        RequestEndCommand = new RelayCommand(CallRequestEnd);
    }


    private string _name =  string.Empty;
    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    public SecureString SecurePassword1 { get; set; } = new SecureString();
    public SecureString SecurePassword2 { get; set; } = new SecureString();
    
    public AsyncRelayCommand SignUpCommand { get; }
    public RelayCommand RequestEndCommand { get; }

    public event EventHandler? RequestEndSignUp;

    private async Task SignUp()
    {
        if (Name.Length < 5)
        {
            _notificationService.ShowMessage(ShownMessage.MessageType.Warning, "Name is too short.");
            return;
        }

        if (SecurePassword1.Length < 6)
        {
            _notificationService.ShowMessage(ShownMessage.MessageType.Warning, "Password is too short.");
            return;
        }
        
        using var loading = _loadingService.StartLoadingOperation("Signing up");
        var salt = RandomNumberGenerator.GetBytes(16); // Generate 16-byte salt
        var stringSalt = Convert.ToBase64String(salt);
        
        var hash = GetHash(SecurePassword1, stringSalt);
        var hash2 = GetHash(SecurePassword2, stringSalt);

        if (hash != hash2)
        {
            _notificationService.ShowMessage(ShownMessage.MessageType.Warning, "Passwords do not match.");
            return;
        }

        var result = await _multiplayerPlayerService.TrySignUpAsync(Name, hash, stringSalt, loading.CancellationToken);
        if (result.IsFailed)
        {
            _notificationService.ShowMessage(ShownMessage.MessageType.Error, result.Errors[0].Message);
            return;
        }
        else
        {
            RequestEndSignUp?.Invoke(this, EventArgs.Empty);
            _notificationService.ShowMessage(ShownMessage.MessageType.Success, $"Created user {Name}");
        }
        
        _loginViewModel.Name = Name;
        _loginViewModel.SecurePassword = SecurePassword1;
        await _loginViewModel.LoginCommand.ExecuteAsync(null);
    }

    private static string GetHash(SecureString secureString, string saltString)
    {
        ArgumentNullException.ThrowIfNull(secureString);

        var unmanagedString = IntPtr.Zero;
        try
        {
            var salt = Convert.FromBase64String(saltString);
            unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(secureString);
            
            var pbkdf2 = new Rfc2898DeriveBytes(Marshal.PtrToStringUni(unmanagedString)!, salt, 100000, HashAlgorithmName.SHA256);
            var hash = pbkdf2.GetBytes(32); // 256-bit hash

            return Convert.ToBase64String(hash);
        }
        finally
        {
            Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString); // Clear memory
        }
    }

    private void CallRequestEnd()
    {
        RequestEndSignUp?.Invoke(this, EventArgs.Empty);
    }
}
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using BattleChess3.Multiplayer;
using BattleChess3.UI.Services;
using CommunityToolkit.Mvvm.Input;
using Nicenis.Windows.ViewModels;

namespace BattleChess3.UI.ViewModel;

public class LoginViewModel : ViewModelBase
{
    private readonly IMultiplayerLoginService _multiplayerLoginService;
    private readonly IMessageShowService _messageShowService;
    
    public LoginViewModel(
        IMultiplayerLoginService multiplayerLoginService,
        IMessageShowService messageShowService)
    {
        _multiplayerLoginService = multiplayerLoginService;
        _messageShowService = messageShowService;
        
        LoginCommand = new AsyncRelayCommand(LogIn);
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

    public event EventHandler? RequestEndLogin;
    
    private async Task LogIn()
    {
        var saltResult = await _multiplayerLoginService.GetUserSaltAsync(Name);
        if (saltResult.IsFailed)
        {
            _messageShowService.ShowMessage("Invalid username or password");
            return;
        }
        
        var hash = GetHash(SecurePassword, saltResult.Value);
        var result = await _multiplayerLoginService.TryLoginAsync(Name, hash);
        if (result.IsFailed)
        {
            _messageShowService.ShowMessage(result.Errors.First().Message);
        }
        else
        {
            RequestEndLogin?.Invoke(this, EventArgs.Empty);
            IsLoggedIn = true;
        }
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
}
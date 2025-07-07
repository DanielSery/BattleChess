using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using BattleChess3.Multiplayer;
using CommunityToolkit.Mvvm.Input;
using Nicenis.Windows.ViewModels;

namespace BattleChess3.UI.ViewModel;

public class LoginViewModel : ViewModelBase
{
    private readonly IMultiplayerService _multiplayerService;
    
    public LoginViewModel(IMultiplayerService multiplayerService)
    {
        _multiplayerService = multiplayerService;
        
        LoginCommand = new RelayCommand(LogIn);
    }

    private string _name =  string.Empty;
    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    public SecureString SecurePassword { get; set; } = new SecureString();
    public RelayCommand LoginCommand { get; }

    private void LogIn()
    {
        var saltResult = _multiplayerService.GetUserSalt(Name).Result;
        if (saltResult.IsFailed)
            throw new Exception("Incorrect name or password");
        
        var hash = GetHash(SecurePassword, saltResult.Value);
        var result = _multiplayerService.TryLogin(Name, hash).Result;
        if (result.IsFailed)
            throw new Exception(result.ToString());
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
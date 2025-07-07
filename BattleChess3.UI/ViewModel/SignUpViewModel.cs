using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using BattleChess3.Multiplayer;
using CommunityToolkit.Mvvm.Input;
using Nicenis.Windows.ViewModels;

namespace BattleChess3.UI.ViewModel;

public class SignUpViewModel : ViewModelBase
{
    private readonly IMultiplayerService _multiplayerService;
    
    public SignUpViewModel(IMultiplayerService multiplayerService)
    {
        _multiplayerService = multiplayerService;
        
        SignUpCommand = new AsyncRelayCommand(SignUp);
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

    private async Task SignUp()
    {
        if (Name.Length < 5)
            throw new Exception("Name is too short.");
        
        if (SecurePassword1.Length < 8)
            throw new Exception("Password must be at least 8 characters");
        
        var salt = RandomNumberGenerator.GetBytes(16); // Generate 16-byte salt
        var stringSalt = Convert.ToBase64String(salt);
        
        var hash = GetHash(SecurePassword1, salt);
        var hash2 = GetHash(SecurePassword2, salt);
        
        if (hash != hash2)
            throw new Exception("Passwords do not match");

        var result = await _multiplayerService.TrySignUp(Name, hash, stringSalt);
        if (result.IsFailed)
            throw new Exception(result.ToString());
    }

    private static string GetHash(SecureString secureString, byte[] salt)
    {
        ArgumentNullException.ThrowIfNull(secureString);

        var unmanagedString = IntPtr.Zero;
        try
        {
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
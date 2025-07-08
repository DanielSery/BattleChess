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
    private readonly IMultiplayerLoginService _multiplayerLoginService;
    private readonly IMessageShowService _messageShowService;
    
    public SignUpViewModel(
        IMultiplayerLoginService multiplayerLoginService,
        IMessageShowService messageShowService)
    {
        _multiplayerLoginService = multiplayerLoginService;
        _messageShowService = messageShowService;
        
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

    public event EventHandler? RequestEndSignUp;

    private async Task SignUp()
    {
        if (Name.Length < 5)
        {
            _messageShowService.ShowMessage("Name is too short.");
            return;
        }

        if (SecurePassword1.Length < 6)
        {
            _messageShowService.ShowMessage("Password is too short.");
            return;
        }
        
        var salt = RandomNumberGenerator.GetBytes(16); // Generate 16-byte salt
        var stringSalt = Convert.ToBase64String(salt);
        
        var hash = GetHash(SecurePassword1, salt);
        var hash2 = GetHash(SecurePassword2, salt);

        if (hash != hash2)
        {
            _messageShowService.ShowMessage("Passwords do not match.");
            return;
        }

        var result = await _multiplayerLoginService.TrySignUpAsync(Name, hash, stringSalt);
        if (result.IsFailed)
        {
            _messageShowService.ShowMessage(result.Errors.First().Message);
        }
        else
        {
            RequestEndSignUp?.Invoke(this, EventArgs.Empty);
        }
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
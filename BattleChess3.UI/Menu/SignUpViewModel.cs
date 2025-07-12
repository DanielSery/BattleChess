using System.Net;
using System.Net.Mail;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using BattleChess3.Multiplayer;
using BattleChess3.UI.MainWindow;
using CommunityToolkit.Mvvm.Input;
using Nicenis.Windows.ViewModels;

namespace BattleChess3.UI.Menu;

public class SignUpViewModel : ViewModelBase
{
    private readonly LoginViewModel _loginViewModel;
    private readonly IMultiplayerPlayerService _multiplayerPlayerService;
    private readonly INotificationService _notificationService;
    private readonly ILoadingService _loadingService;
    private string _privateVerificationCode = string.Empty;
    
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
        VerifyEmailCommand = new AsyncRelayCommand(VerifyEmail);
    }

    private string _name =  string.Empty;
    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }
    
    private bool _verificationInProgress;
    public bool VerificationInProgress
    {
        get => _verificationInProgress;
        set => SetProperty(ref _verificationInProgress, value);
    }

    private string _emailVerificationCode = string.Empty;
    public string EmailVerificationCode
    {
        get => _emailVerificationCode;
        set => SetProperty(ref _emailVerificationCode, value);
    }
    
    private string _email = string.Empty;
    public string Email
    {
        get => _email;
        set
        {
            SetProperty(ref _email, value);
            VerificationInProgress = false;
        }
    }

    public SecureString SecurePassword1 { get; set; } = new SecureString();
    public SecureString SecurePassword2 { get; set; } = new SecureString();
    
    public AsyncRelayCommand SignUpCommand { get; }
    public AsyncRelayCommand VerifyEmailCommand { get; }
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

        if (!string.Equals(EmailVerificationCode.Trim(), _privateVerificationCode, StringComparison.Ordinal))
        {
            _notificationService.ShowMessage(ShownMessage.MessageType.Warning, "Invalid verification code.");
            return;
        }
        
        using var loading = _loadingService.StartLoadingOperation("Signing up");
        var passwordSalt = Convert.ToBase64String(RandomNumberGenerator.GetBytes(16));
        var password1Hash = GetHash(SecurePassword1, passwordSalt);
        var password2Hash = GetHash(SecurePassword2, passwordSalt);
        if (password1Hash != password2Hash)
        {
            _notificationService.ShowMessage(ShownMessage.MessageType.Warning, "Passwords do not match.");
            return;
        }

        var emailHash = GetHash(Email, Secrets.EmailSalt);
        var result = await _multiplayerPlayerService.TrySignUpAsync(Name, password1Hash, passwordSalt, emailHash, loading.CancellationToken);
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

    private async Task VerifyEmail()
    {
        var name = string.IsNullOrEmpty(Name) ? "New user" : Name;
        
        var fromAddress = new MailAddress("thebattlechess3@gmail.com", "BattleChess 3");
        MailAddress toAddress;
        try
        {
            toAddress = new MailAddress(Email, name);
        }
        catch (FormatException e)
        {
            Console.WriteLine(e);
            _notificationService.ShowMessage(ShownMessage.MessageType.Warning, "Invalid email address format");
            return;
        }
        catch (ArgumentException e)
        {
            Console.WriteLine(e);
            _notificationService.ShowMessage(ShownMessage.MessageType.Warning, "Invalid email address");
            return;
        }
        
        using var loading = _loadingService.StartLoadingOperation("Checking existing users");
        var emailHash = GetHash(Email, Secrets.EmailSalt);
        var result = await _multiplayerPlayerService.TryVerifyEmailAsync(emailHash, loading.CancellationToken);
        if (result.IsFailed)
        {
            _notificationService.ShowMessage(ShownMessage.MessageType.Warning, result.Errors[0].Message);
            return;
        }
        
        const string subject = "BattleChess 3 verification code";
        var body = $"Hello,\n\nYour verification code is: {_privateVerificationCode}\nPaste it into the Battle Chess 3 verification field.\n\nHave a nice day!\nBattleChess 3";

        var smtp = new SmtpClient
        {
            Host = "smtp.gmail.com",
            Port = 587,
            EnableSsl = true,
            DeliveryMethod = SmtpDeliveryMethod.Network,
            Credentials = new NetworkCredential(fromAddress.Address, Secrets.GmailString),
            Timeout = 20000
        };

        loading.Message = "Sending validation email";
        using var message = new MailMessage(fromAddress, toAddress);
        message.Subject = subject;
        message.Body = body;
        try
        {
            await smtp.SendMailAsync(message, loading.CancellationToken);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            _notificationService.ShowMessage(ShownMessage.MessageType.Error, "Failed to send verification email");
            return;
        }
        
        _notificationService.ShowMessage(ShownMessage.MessageType.Success, "Verification email sent");
        VerificationInProgress = true;
    }

    private static string GetHash(string str, string saltString)
    {
        var salt = Convert.FromBase64String(saltString);
        var pbkdf2 = new Rfc2898DeriveBytes(str, salt, 100000, HashAlgorithmName.SHA256);
        var hash = pbkdf2.GetBytes(32); // 256-bit hash
        return Convert.ToBase64String(hash);
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

    public void OnActivation()
    {
        _privateVerificationCode = GenerateVerificationCode();
    }

    public void OnDeactivation()
    {
    }
    
    
    // Excludes: 0, O, l, 1, I
    private const string ReadableChars = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz23456789";
    private static string GenerateVerificationCode(int length = 8)
    {
        var result = new char[length];
        using var rng = RandomNumberGenerator.Create();
        var buffer = new byte[sizeof(uint)];

        for (var i = 0; i < length; i++)
        {
            rng.GetBytes(buffer);
            var num = BitConverter.ToUInt32(buffer, 0);
            result[i] = ReadableChars[(int)(num % (uint)ReadableChars.Length)];
        }

        return new string(result);
    }

    private void CallRequestEnd()
    {
        RequestEndSignUp?.Invoke(this, EventArgs.Empty);
    }
}
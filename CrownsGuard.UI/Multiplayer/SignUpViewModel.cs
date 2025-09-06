using System.Net;
using System.Net.Mail;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using CrownsGuard.Multiplayer;
using CrownsGuard.Multiplayer.Mail;
using CrownsGuard.Multiplayer.Players;
using CrownsGuard.Multiplayer.Utilities;
using CrownsGuard.UI.Editor;
using CrownsGuard.UI.Services;
using CommunityToolkit.Mvvm.Input;
using Nicenis.Windows.ViewModels;

namespace CrownsGuard.UI.Multiplayer;

public class SignUpViewModel : ViewModelBase
{
    private readonly ISoundService _soundService;
    
    private readonly LoginViewModel _loginViewModel;
    private readonly IMultiplayerPlayerService _multiplayerPlayerService;
    private readonly INotificationService _notificationService;
    private readonly ILoadingService _loadingService;
    private readonly TeamBoardViewModel _teamBoardViewModel;
    private readonly IEmailClient _emailClient;
    private string _privateVerificationCode = string.Empty;
    
    public SignUpViewModel(
        LoginViewModel loginViewModel,
        IMultiplayerPlayerService multiplayerPlayerService,
        INotificationService notificationService,
        ILoadingService loadingService,
        TeamBoardViewModel teamBoardViewModel,
        ISoundService soundService,
        IEmailClient emailClient)
    {
        _loginViewModel = loginViewModel;
        _multiplayerPlayerService = multiplayerPlayerService;
        _notificationService = notificationService;
        _loadingService = loadingService;
        _teamBoardViewModel = teamBoardViewModel;
        _soundService = soundService;
        _emailClient = emailClient;
        
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
            _soundService.PlaySoundEffect(SoundEffectType.Error);
            return;
        }
        
        if (SecurePassword1.Length < 6)
        {
            _notificationService.ShowMessage(ShownMessage.MessageType.Warning, "Password is too short.");
            _soundService.PlaySoundEffect(SoundEffectType.Error);
            return;
        }

        if (!string.Equals(EmailVerificationCode.Trim(), _privateVerificationCode, StringComparison.Ordinal))
        {
            _notificationService.ShowMessage(ShownMessage.MessageType.Warning, "Invalid verification code.");
            _soundService.PlaySoundEffect(SoundEffectType.Error);
            return;
        }
        
        using var loading = _loadingService.StartLoadingOperation("Signing up");
        var passwordSalt = HashingHelper.GetSalt();
        var password1Hash = HashingHelper.GetHash(SecurePassword1, passwordSalt);
        var password2Hash = HashingHelper.GetHash(SecurePassword2, passwordSalt);
        if (password1Hash != password2Hash)
        {
            _notificationService.ShowMessage(ShownMessage.MessageType.Warning, "Passwords do not match.");
            _soundService.PlaySoundEffect(SoundEffectType.Error);
            return;
        }
        
        var myMap = _teamBoardViewModel.GetMapBlueprint();
        var emailHash = HashingHelper.GetEmailHash(Email);
        var result = await _multiplayerPlayerService.TrySignUpAsync(Name, password1Hash, passwordSalt, emailHash, myMap, loading.CancellationToken);
        if (result.IsFailed)
        {
            _notificationService.ShowMessage(ShownMessage.MessageType.Error, result.Errors[0].Message);
            _soundService.PlaySoundEffect(SoundEffectType.Error);
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

        MailAddress toAddress;
        try
        {
            toAddress = new MailAddress(Email, name);
        }
        catch (FormatException e)
        {
            Console.WriteLine(e);
            _notificationService.ShowMessage(ShownMessage.MessageType.Warning, "Invalid email address format");
            _soundService.PlaySoundEffect(SoundEffectType.Error);
            return;
        }
        catch (ArgumentException e)
        {
            Console.WriteLine(e);
            _notificationService.ShowMessage(ShownMessage.MessageType.Warning, "Invalid email address");
            _soundService.PlaySoundEffect(SoundEffectType.Error);
            return;
        }
        
        using var loading = _loadingService.StartLoadingOperation("Checking existing users");
        var emailHash = HashingHelper.GetEmailHash(Email);
        var result = await _multiplayerPlayerService.TryVerifyEmailAsync(emailHash, loading.CancellationToken);
        if (result.IsFailed)
        {
            _notificationService.ShowMessage(ShownMessage.MessageType.Warning, result.Errors[0].Message);
            _soundService.PlaySoundEffect(SoundEffectType.Error);
            return;
        }
        
        loading.Message = "Sending validation email";
        try
        {
            await _emailClient.SendVerificationEmail(toAddress, _name, _privateVerificationCode, loading.CancellationToken);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            _notificationService.ShowMessage(ShownMessage.MessageType.Error, "Failed to send verification email");
            _soundService.PlaySoundEffect(SoundEffectType.Error);
            return;
        }
        
        _notificationService.ShowMessage(ShownMessage.MessageType.Success, "Verification email sent");
        VerificationInProgress = true;
        _soundService.PlaySoundEffect(SoundEffectType.Button);
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
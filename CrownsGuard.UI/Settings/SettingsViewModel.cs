using System.IO;
using System.Windows;
using CrownsGuard.UI.Services;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;
using Nicenis.Windows.ViewModels;

namespace CrownsGuard.UI.Settings;

public class SettingsViewModel : ViewModelBase
{
    private readonly ISoundService _soundService;

    public event EventHandler? RequestEnd;

    public SettingsViewModel(
        ISoundService soundService)
    {
        _soundService = soundService;
        
        ConfirmCommand = new RelayCommand(Confirm);
        CancelCommand = new RelayCommand(Cancel);
        ToggleFullscreenCommand = new RelayCommand(ToggleFullscreen);

        LoadSavedSettings();
        _soundService.Initialize();
    }


    private double _volume = 0.6;
    public double Volume
    {
        get => _volume;
        set
        {
            SetProperty(ref _volume, value);
            _soundService.SetVolume(value);
        }
    }

    private double _soundsVolume = 0.6;
    public double SoundsVolume
    {
        get => _soundsVolume;
        set
        {
            SetProperty(ref _soundsVolume, value);
            _soundService.SetSoundsVolume(value);
        }
    }

    private double _musicVolume = 0.6;
    public double MusicVolume
    {
        get => _musicVolume;
        set
        {
            SetProperty(ref _musicVolume, value);
            _soundService.SetMusicVolume(value);
        }
    }

    private WindowState _oldWindowState;
    private WindowState _windowState;
    public WindowState WindowState
    {
        get => _windowState;
        set => SetProperty(ref _windowState, value);
    }

    private bool _isFullScreen;
    public bool IsFullScreen
    {
        get => _isFullScreen;
        set => SetProperty(ref _isFullScreen, value);
    }

    public RelayCommand ConfirmCommand { get; }
    public RelayCommand CancelCommand { get; }
    public RelayCommand ToggleFullscreenCommand { get; }
    
    private void ToggleFullscreen()
    {
        SetFullscreen(!IsFullScreen);
    }

    private void SetFullscreen(bool isFullscreen)
    {
        if (!isFullscreen)
        {
            IsFullScreen = false;
            WindowState = _oldWindowState;
        }
        else
        {
            _oldWindowState = WindowState;
            IsFullScreen = true;
            WindowState = WindowState.Maximized;
        }
    }

    private void LoadSavedSettings()
    {
        if (!Directory.Exists("Resources"))
        {
            Directory.CreateDirectory("Resources");
        }

        var settings = new SavedSettings();
        if (File.Exists("Resources/Settings.json"))
        {
            settings = JsonConvert.DeserializeObject<SavedSettings>(File.ReadAllText("Resources/Settings.json"))
                ?? new SavedSettings();
        }
        
        Volume = settings.Volume;
        SoundsVolume = settings.SoundsVolume;
        MusicVolume = settings.MusicVolume;
        SetFullscreen(settings.IsFullScreen);
        WindowState = settings.WindowState;
    }

    private void Confirm()
    {
        var savedSettings = new SavedSettings
        {
            Volume = Volume,
            SoundsVolume = SoundsVolume,
            MusicVolume = MusicVolume,
            IsFullScreen = IsFullScreen,
            WindowState = WindowState
        };
        
        File.WriteAllText("Resources/Settings.json", JsonConvert.SerializeObject(savedSettings));
        RequestEnd?.Invoke(this, EventArgs.Empty);
    }

    private void Cancel()
    {
        LoadSavedSettings();
        
        RequestEnd?.Invoke(this, EventArgs.Empty);
    }
}
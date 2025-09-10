using System.Windows;
using System.Windows.Media;

namespace CrownsGuard.UI.Services;

public class SoundService : ISoundService
{
    private readonly Random _random = new Random();
    private readonly MediaPlayer _musicPlayer = new MediaPlayer();

    private bool _backgroundMusicPlaying;
    private MediaPlayer? _soundEffectPlayer;
    private Task _backgroundMusicTask = Task.CompletedTask;
    private CancellationTokenSource _backgroundMusicCancellationTokenSource = new CancellationTokenSource();

    private double _musicVolume;
    private double _soundsVolume;
    private double _totalVolume;

    public void Initialize()
    {
        StartInitialBackgroundMusic();
    }

    private double GetActualMusicVolume()
    {
        return _musicVolume * _totalVolume * 0.42;
    }

    private double GetSoundsVolume()
    {
        return _soundsVolume * _totalVolume * 0.8;
    }

    private void StartInitialBackgroundMusic()
    { 
        DispatchBackgroundMusicAction(() =>
        {
            const string path = "./Resources/Sounds/Background1.mp3";
            _musicPlayer.Open(new Uri(path, UriKind.RelativeOrAbsolute));
            _musicPlayer.Play();
            _musicPlayer.Volume = GetActualMusicVolume();
            _musicPlayer.MediaEnded += (_, _) => StartNextBackgroundSong();
            _backgroundMusicPlaying = true;
            return Task.CompletedTask;
        });
    }

    public void ContinueBackgroundMusic()
    {
        DispatchBackgroundMusicAction(async () =>
        {
            if (_backgroundMusicPlaying)
            {
                var maxVolume = GetActualMusicVolume();
                for (var i = 0; i <= 20; i++)
                {
                    if (_musicPlayer.Volume >= maxVolume * i / 20)
                        continue;
                    
                    await Task.Delay(30, _backgroundMusicCancellationTokenSource.Token);
                    _musicPlayer.Volume = maxVolume * i / 20;
                }
            }
            else
            {
                StartNextBackgroundSong();
            }
        });
    }

    public void PauseBackgroundMusic()
    {
        DispatchBackgroundMusicAction(async () =>
        {
            var maxVolume = GetActualMusicVolume();
            for (var i = 0; i <= 20; i++)
            {
                if (_musicPlayer.Volume <= maxVolume * (20 - i) / 20)
                    continue;
                
                await Task.Delay(30, _backgroundMusicCancellationTokenSource.Token);
                _musicPlayer.Volume = maxVolume * (20 - i) / 20;
            }

            await Task.Delay(1000, _backgroundMusicCancellationTokenSource.Token);
            _backgroundMusicPlaying = false;
            _musicPlayer.Stop();
        });
    }

    private Task DispatchBackgroundMusicAction(Func<Task> action)
    {
        var tcs = new TaskCompletionSource();
        Application.Current.Dispatcher.Invoke(async () =>
        {
            await CancelBackgroundMusicAction();
            _backgroundMusicCancellationTokenSource = new CancellationTokenSource();
            _backgroundMusicTask = tcs.Task;
            
            try
            {
                await action();
                tcs.TrySetResult();
            }
            catch
            {
                Console.WriteLine("Cancelled background music action");
                tcs.TrySetResult();
            }
        });
        
        return tcs.Task;
    }

    private void StartNextBackgroundSong()
    {
        var path = $"./Resources/Sounds/Background{_random.Next(2, 6)}.mp3";
        _musicPlayer.Volume = GetActualMusicVolume();
        Console.WriteLine($"Starting background music for {path}");
        _musicPlayer.Open(new Uri(path, UriKind.RelativeOrAbsolute));
        _musicPlayer.Play();
        _backgroundMusicPlaying = true;
    }

    private async Task CancelBackgroundMusicAction()
    {
        if (!_backgroundMusicCancellationTokenSource.IsCancellationRequested)
            await _backgroundMusicCancellationTokenSource.CancelAsync();
        
        try
        {
            await _backgroundMusicTask;        
        }
        catch (Exception)
        {
            Console.WriteLine("Background music action cancelled");
        }
    }

    public void PlaySoundEffect(SoundEffectType effectType)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            _soundEffectPlayer?.Stop();
            _soundEffectPlayer = new MediaPlayer();
            var path = effectType switch
            {
                SoundEffectType.ChessFigure => $"./Resources/Sounds/Chess{_random.Next(1, 7)}.wav",
                SoundEffectType.MenuAnimation => "./Resources/Sounds/Rolling2.wav",
                SoundEffectType.SmallMenuAnimation => "./Resources/Sounds/Rolling1.wav",
                SoundEffectType.Button => "./Resources/Sounds/Button1.wav",
                SoundEffectType.Error => "./Resources/Sounds/Error.wav",
                _ => throw new ArgumentOutOfRangeException(nameof(effectType))
            };

            _soundEffectPlayer.Open(new Uri(path, UriKind.RelativeOrAbsolute));
            _soundEffectPlayer.Play();
            _soundEffectPlayer.Volume = GetSoundsVolume();
            _soundEffectPlayer.MediaEnded += OnSoundEffectPlayerOnMediaEnded;
        });

        void OnSoundEffectPlayerOnMediaEnded(object? o, EventArgs eventArgs)
        {
            _soundEffectPlayer.MediaEnded -= OnSoundEffectPlayerOnMediaEnded;
            _soundEffectPlayer.Close();
        }
    }

    /// <inheritdoc />
    public void SetVolume(double value)
    {
        _totalVolume = value;
        Application.Current.Dispatcher.Invoke(() =>
        {
            _musicPlayer.Volume = GetActualMusicVolume();
        });
    }

    /// <inheritdoc />
    public void SetSoundsVolume(double value)
    {
        _soundsVolume = value;
    }

    /// <inheritdoc />
    public void SetMusicVolume(double value)
    {
        _musicVolume = value;
        Application.Current.Dispatcher.Invoke(() =>
        {
            _musicPlayer.Volume = GetActualMusicVolume();
        });
    }
}
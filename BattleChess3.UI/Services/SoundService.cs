using System.Windows.Media;

namespace BattleChess3.UI.Services;

public class SoundService : ISoundService
{
    private readonly Random _random = new Random();
    private MediaPlayer? _soundEffectPlayer;

    public void PlaySoundEffect(SoundEffectType effectType)
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
        _soundEffectPlayer.MediaEnded += (s, e) => _soundEffectPlayer.Close();
    }
}
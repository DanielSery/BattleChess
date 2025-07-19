namespace BattleChess3.UI.Services;

public interface ISoundService
{
    void Initialize();
    void ContinueBackgroundMusic();
    void PauseBackgroundMusic();
    void PlaySoundEffect(SoundEffectType effectType);
    void SetVolume(double value);
    void SetSoundsVolume(double value);
    void SetMusicVolume(double value);
}
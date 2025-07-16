namespace BattleChess3.UI.Services;

public interface ISoundService
{
    void ContinueBackgroundMusic();
    void PauseBackgroundMusic();
    void PlaySoundEffect(SoundEffectType effectType);
}
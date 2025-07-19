using System.Windows;

namespace BattleChess3.UI.Settings;

// JSON serializable
public class SavedSettings
{
    public double Volume { get; set; } = 0.6;

    public double SoundsVolume { get; set; } = 0.6;

    public double MusicVolume { get; set; } = 0.6;

    public WindowState WindowState { get; set; } = WindowState.Normal;

    public bool IsFullScreen { get; set; } = false;
}
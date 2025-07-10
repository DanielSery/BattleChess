using System.Runtime.InteropServices;
using BattleChess3.Multiplayer;

namespace BattleChess3.UI;

/// <summary>
///     Interaction logic for App.xaml
/// </summary>
public partial class App
{
    [DllImport("kernel32.dll")]
    static extern bool AllocConsole();
    
    public App()
    {
        // new DatabaseClearer().ClearDatabase();
        DependenciesBuilder.Initialize();
        AllocConsole();
    }
}
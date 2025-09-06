using System.Diagnostics;
using System.Runtime.ExceptionServices;
using CrownsGuard.UI.Shared;

namespace CrownsGuard.UI;

/// <summary>
///     Interaction logic for App.xaml
/// </summary>
public partial class App
{
    // [DllImport("kernel32.dll")]
    // static extern bool AllocConsole();
    
    public App()
    {
        // new DatabaseClearer().ClearDatabase();
        DependenciesBuilder.Initialize();
        AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
        AppDomain.CurrentDomain.FirstChanceException += CurrentDomainOnFirstChanceException;
        // AllocConsole();
    }

    private void CurrentDomainOnFirstChanceException(object? sender, FirstChanceExceptionEventArgs e)
    {
        Debugger.Break();
    }

    private void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        Debugger.Break();
    }
}
using CrownsGuard.UI.MainWindow;
using CommonServiceLocator;

namespace CrownsGuard.UI.Shared;

public static class ViewModelLocator
{
    public static MainWindowViewModel MainWindowViewModel => ServiceLocator.Current.GetInstance<MainWindowViewModel>();
}
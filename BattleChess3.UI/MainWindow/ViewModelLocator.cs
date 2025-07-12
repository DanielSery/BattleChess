using CommonServiceLocator;

namespace BattleChess3.UI.MainWindow;

public static class ViewModelLocator
{
    public static MainWindowViewModel MainWindowViewModel => ServiceLocator.Current.GetInstance<MainWindowViewModel>();
}
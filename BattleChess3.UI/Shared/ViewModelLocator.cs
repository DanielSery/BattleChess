using CommonServiceLocator;

namespace BattleChess3.UI.Shared;

public static class ViewModelLocator
{
    public static MainWindowViewModel MainWindowViewModel => ServiceLocator.Current.GetInstance<MainWindowViewModel>();
}
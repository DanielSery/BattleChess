using CommonServiceLocator;

namespace BattleChess3.UI.ViewModel;

public static class ViewModelLocator
{
    public static MainWindowViewModel MainWindowViewModel => ServiceLocator.Current.GetInstance<MainWindowViewModel>();
}
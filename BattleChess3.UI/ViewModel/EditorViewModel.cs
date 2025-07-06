using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace BattleChess3.UI.ViewModel;

public sealed class EditorViewModel : ViewModelBase
{
    public EditorViewModel(
        EditorUnitsViewModel editorUnits,
        TeamBoardViewModel teamBoard)
    {
        EditorUnits = editorUnits;
        TeamBoard = teamBoard;

        SaveGameCommand = new RelayCommand(SaveGame);
        CancelCommand = new RelayCommand(Cancel);
    }

    public EditorUnitsViewModel EditorUnits { get; }
    public TeamBoardViewModel TeamBoard { get; }

    public RelayCommand SaveGameCommand { get; }
    public RelayCommand CancelCommand { get; }

    public event EventHandler? RequestSwitchToMainView;

    private void SaveGame()
    {
        TeamBoard.SaveMap();
        RequestSwitchToMainView?.Invoke(this, EventArgs.Empty);
    }

    private void Cancel()
    {
        TeamBoard.Discard();
        RequestSwitchToMainView?.Invoke(this, EventArgs.Empty);
    }
}
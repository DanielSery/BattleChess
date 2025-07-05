using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace BattleChess3.UI.ViewModel;

public sealed class EditorViewModel : ViewModelBase
{
    public EditorViewModel(
        MapsViewModel maps,
        EditorUnitsViewModel editorUnits,
        TeamBoardViewModel teamBoard)
    {
        Maps = maps;
        EditorUnits = editorUnits;
        TeamBoard = teamBoard;

        SaveGameCommand = new RelayCommand(SaveGame);
        CancelCommand = new RelayCommand(Cancel);
    }

    public MapsViewModel Maps { get; }
    public EditorUnitsViewModel EditorUnits { get; }
    public TeamBoardViewModel TeamBoard { get; set; }

    public RelayCommand SaveGameCommand { get; }
    public RelayCommand CancelCommand { get; }

    public event EventHandler? RequestSwitchToMainView;

    private void SaveGame()
    {
        var identifier = DateTime.Now.Ticks.ToString();
        TeamBoard.RequestSave(identifier);
        Maps.SaveSelectedMap(identifier, TeamBoard.Tiles);
    }

    private void Cancel()
    {
        RequestSwitchToMainView?.Invoke(this, EventArgs.Empty);
    }
}
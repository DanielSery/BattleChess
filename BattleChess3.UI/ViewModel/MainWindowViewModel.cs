﻿using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace BattleChess3.UI.ViewModel;

public sealed class MainWindowViewModel : ViewModelBase
{
    private bool _editorTabSelected;

    private bool _gameTabEnabled;

    private bool _gameTabSelected;

    private bool _manualTabSelected;
    private bool _menuTabSelected;

    private bool _optionsTabSelected;

    public MainWindowViewModel(
        MapsViewModel mapsViewModel,
        BoardViewModel boardViewModel,
        FiguresViewModel figuresViewModel,
        MultiplayerViewModel multiplayerViewModel)
    {
        MapsViewModel = mapsViewModel;
        BoardViewModel = boardViewModel;
        FiguresViewModel = figuresViewModel;
        MultiplayerViewModel = multiplayerViewModel;

        NewGameCommand = new RelayCommand(NewGame);
        SaveGameCommand = new RelayCommand(SaveGame);
        DeleteGameCommand = new RelayCommand(DeleteGame);
        SelectOptionsCommand = new RelayCommand(() => OptionsTabSelected = true);
        CloseApplicationCommand = new RelayCommand(CloseApplication);
    }

    public bool MenuTabSelected
    {
        get => _menuTabSelected;
        set => SetTabSelected(out _menuTabSelected);
    }

    public bool GameTabSelected
    {
        get => _gameTabSelected;
        set => SetTabSelected(out _gameTabSelected);
    }

    public bool GameTabEnabled
    {
        get => _gameTabEnabled;
        set => Set(ref _gameTabEnabled, value);
    }

    public bool OptionsTabSelected
    {
        get => _optionsTabSelected;
        set => SetTabSelected(out _optionsTabSelected);
    }

    public bool ManualTabSelected
    {
        get => _manualTabSelected;
        set => SetTabSelected(out _manualTabSelected);
    }

    public bool EditorTabSelected
    {
        get => _editorTabSelected;
        set => SetTabSelected(out _editorTabSelected);
    }

    public MapsViewModel MapsViewModel { get; }
    public BoardViewModel BoardViewModel { get; }
    public FiguresViewModel FiguresViewModel { get; }
    public MultiplayerViewModel MultiplayerViewModel { get; }

    public RelayCommand NewGameCommand { get; }
    public RelayCommand SaveGameCommand { get; }
    public RelayCommand DeleteGameCommand { get; }
    public RelayCommand SelectOptionsCommand { get; }
    public RelayCommand CloseApplicationCommand { get; }

    public event EventHandler<string>? RequestSavePreview;

    private void NewGame()
    {
        BoardViewModel.ManualLoadMap(MapsViewModel.SelectedMap);
        GameTabEnabled = true;
        GameTabSelected = true;
    }

    private void SaveGame()
    {
        if (!GameTabEnabled)
        {
            return;
        }

        var identifier = DateTime.Now.Ticks.ToString();
        RequestSavePreview?.Invoke(this, identifier);
        MapsViewModel.SaveSelectedMap(identifier, BoardViewModel.Tiles);
    }

    private void DeleteGame()
    {
        MapsViewModel.DeleteSelectedMap();
    }

    private static void CloseApplication()
    {
        Application.Current.Shutdown();
    }

    private void SetTabSelected(out bool selectedTab)
    {
        _menuTabSelected = false;
        _gameTabSelected = false;
        _optionsTabSelected = false;
        _manualTabSelected = false;
        _editorTabSelected = false;
        selectedTab = true;

        RaisePropertyChanged(nameof(MenuTabSelected));
        RaisePropertyChanged(nameof(GameTabSelected));
        RaisePropertyChanged(nameof(OptionsTabSelected));
        RaisePropertyChanged(nameof(ManualTabSelected));
        RaisePropertyChanged(nameof(EditorTabSelected));
    }
}
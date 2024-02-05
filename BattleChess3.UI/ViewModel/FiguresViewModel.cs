using BattleChess3.Core.Model.Figures;
using BattleChess3.UI.Services;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BattleChess3.UI.ViewModel;

public sealed class FiguresViewModel : ViewModelBase, IDisposable
{
    private readonly IFigureService _figureService;

    private IFigureGroup _selectedFigureGroup = EmptyFigureGroup.Instance;
    public IFigureGroup SelectedFigureGroup
    {
        get => _selectedFigureGroup;
        private set => Set(ref _selectedFigureGroup, value);
    }

    private IList<IFigureGroup> _figureGroups = Array.Empty<IFigureGroup>();
    public IList<IFigureGroup> FigureGroups
    {
        get => _figureGroups;
        private set
        {
            Set(ref _figureGroups, value);
            if (_figureGroups.All(x => x.ShownName != _selectedFigureGroup.ShownName))
            {
                SelectedFigureGroup = _figureGroups.FirstOrDefault()
                    ?? EmptyFigureGroup.Instance;
            }
        }
    }

    public RelayCommand<IFigureGroup> SelectFigureGroupCommand { get; }

    public FiguresViewModel(IFigureService figureService)
    {
        _figureService = figureService;
        FigureGroups = _figureService.GetFigureGroups();
        _figureService.FigureGroupsChanged += OnFigureGroupsChanged;

        SelectFigureGroupCommand = new RelayCommand<IFigureGroup>(group => SelectedFigureGroup = group);
    }

    private void OnFigureGroupsChanged(object? sender, IList<IFigureGroup> groups)
    {
        FigureGroups = groups;
    }

    public void Dispose()
    {
        _figureService.FigureGroupsChanged -= OnFigureGroupsChanged;
    }
}

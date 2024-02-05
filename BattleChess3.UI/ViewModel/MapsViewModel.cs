﻿using BattleChess3.Core.Model;
using BattleChess3.Core.Model.Figures;
using BattleChess3.UI.Services;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace BattleChess3.UI.ViewModel;

public sealed class MapsViewModel : ViewModelBase, IDisposable
{
    private readonly IMapService _mapService;
    private readonly IPlayerService _playerService;

    private MapBlueprint _selectedMap = MapBlueprint.None;
    public MapBlueprint SelectedMap
    {
        get => _selectedMap;
        set => Set(ref _selectedMap, value);
    }

    private IList<MapBlueprint> _maps = Array.Empty<MapBlueprint>();
    public IList<MapBlueprint> Maps
    {
        get => _maps;
        private set
        {
            if (value.All(x => x.MapPath != _selectedMap.MapPath))
            {
                SelectedMap = value.FirstOrDefault()
                    ?? MapBlueprint.None;
            }
            Set(ref _maps, value);
        }
    }

    public MapsViewModel(
        IMapService mapService,
        IPlayerService playerService)
    {
        _mapService = mapService;
        _playerService = playerService;

        Maps = new ObservableCollection<MapBlueprint>(_mapService.GetCurrentMaps());
        _mapService.MapsChanged += OnMapsChanged;
    }

    private void OnMapsChanged(object? sender, IList<MapBlueprint> maps)
    {
        Maps = new ObservableCollection<MapBlueprint>(maps);
    }

    internal void DeleteSelectedMap()
    {
        if (SelectedMap != MapBlueprint.None)
        {
            _mapService.Delete(SelectedMap);
        }
    }

    internal void SaveSelectedMap(string identifier, IEnumerable<ITile> board)
    {
        var map = new MapBlueprint
        {
            Figures = board.Select(x => new FigureBlueprint
            {
                PlayerId = x.Figure.Owner.Id,
                UnitName = x.Figure.UnitName
            }).ToArray(),
            MapPath = $"Resources/Maps/{identifier}.map",
            PreviewPath = $"./Resources/Maps/{identifier}.png",
            StartingPlayer = _playerService.CurrentPlayer.Id,
        };

        _mapService.Save(map);
    }

    public void Dispose()
    {
        _mapService.MapsChanged -= OnMapsChanged;
    }
}

﻿using System.Windows;
using BattleChess3.UI.Model;
using BattleChess3.UI.Services;
using GalaSoft.MvvmLight;

namespace BattleChess3.UI.ViewModel;

public sealed class ThemesViewModel : ViewModelBase, IDisposable
{
    private readonly IThemeService _themesService;

    private ThemeModel _selectedTheme = ThemeModel.None;
    public ThemeModel SelectedTheme
    {
        get => _selectedTheme;
        set
        {
            Set(ref _selectedTheme, value);
            foreach (var keyObject in value.ResourceDictionary.Keys)
            {
                if (keyObject is not { })
                    return;
                
                Application.Current.Resources[keyObject] = value.ResourceDictionary[keyObject];
            }
        }
    }

    private IList<ThemeModel> _themes = Array.Empty<ThemeModel>();
    public IList<ThemeModel> Themes
    {
        get => _themes;
        private set
        {
            Set(ref _themes, value);
            if (_themes.All(x => x.Name != _selectedTheme.Name))
            {
                SelectedTheme = _themes.FirstOrDefault(x => x.Name.Contains("Paper")) ??
                    _themes.FirstOrDefault() ??
                    ThemeModel.None;
            }
        }
    }

    public ThemesViewModel(
        IThemeService themesService)
    {
        _themesService = themesService;
        Themes = _themesService.GetCurrentThemes();
        _themesService.ThemesChanged += OnThemesChanged;
    }

    private void OnThemesChanged(object? sender, IList<ThemeModel> themes)
    {
        Themes = themes;
    }

    public void Dispose()
    {
        _themesService.ThemesChanged -= OnThemesChanged;
    }
}

﻿using BattleChess3.UI.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleChess3.UI.Services
{
    public class ThemesService : IThemesService
    {
        private ThemeModel[] _themes = Array.Empty<ThemeModel>();

        public event EventHandler<IList<ThemeModel>>? ThemesChanged;

        public ThemesService()
        {
            using var watcher = new FileSystemWatcher(".");

            watcher.NotifyFilter = NotifyFilters.Attributes
                                 | NotifyFilters.CreationTime
                                 | NotifyFilters.DirectoryName
                                 | NotifyFilters.FileName
                                 | NotifyFilters.LastAccess
                                 | NotifyFilters.LastWrite
                                 | NotifyFilters.Security
                                 | NotifyFilters.Size;

            watcher.Changed += OnChanged;
            watcher.Created += OnChanged;
            watcher.Deleted += OnChanged;
            watcher.Renamed += OnChanged;

            watcher.Filter = "*Theme.dll";
            watcher.IncludeSubdirectories = true;
            watcher.EnableRaisingEvents = true;

            Task.Run(() => ReloadThemes());
        }

        public IList<ThemeModel> GetCurrentThemes()
        {
            return _themes;
        }

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            ReloadThemes();
        }

        private void ReloadThemes()
        {
            _themes = Directory.GetFiles(".", "*Theme.dll")
                .Select(path => new ThemeModel(Path.GetFullPath(path)))
                .ToArray();
            ThemesChanged?.Invoke(this, _themes);
        }
    }
}

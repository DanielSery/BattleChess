using System.Reflection;

namespace BattleChess3.Game.Figures;

internal class FigureService : IFigureService
{
    private readonly FileSystemWatcher _watcher;

    private IFigureGroup[] _figureGroups = [];
    private Dictionary<int, IFigureType> _figuresDictionary = new();
    private readonly TaskCompletionSource<bool> _figuresLoaded = new TaskCompletionSource<bool>();

    public FigureService()
    {
        _watcher = new FileSystemWatcher(".");

        _watcher.NotifyFilter = NotifyFilters.Attributes
                                | NotifyFilters.CreationTime
                                | NotifyFilters.DirectoryName
                                | NotifyFilters.FileName
                                | NotifyFilters.LastAccess
                                | NotifyFilters.LastWrite
                                | NotifyFilters.Security
                                | NotifyFilters.Size;

        _watcher.Changed += OnChanged;
        _watcher.Created += OnChanged;
        _watcher.Deleted += OnChanged;
        _watcher.Renamed += OnChanged;

        _watcher.Filter = "*Figures.dll";
        _watcher.IncludeSubdirectories = true;
        _watcher.EnableRaisingEvents = true;

        Task.Run(ReloadFigures);
    }

    public event EventHandler<IList<IFigureGroup>>? FigureGroupsChanged;

    public IList<IFigureGroup> GetFigureGroups()
    {
        return _figureGroups;
    }

    public IFigureType GetFigureByUniqueUnitId(int uniqueUnitId)
    {
        _figuresLoaded.Task.Wait();
        return _figuresDictionary[uniqueUnitId];
    }

    private void OnChanged(object sender, FileSystemEventArgs e)
    {
        ReloadFigures();
    }

    private void ReloadFigures()
    {
        _figureGroups = Directory.GetFiles(".", "*Figures.dll")
            .Select(path => Assembly.LoadFile(Path.GetFullPath(path)))
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => type.GetInterfaces().Any(x => x == typeof(IFigureGroup)))
            .Select(type => (IFigureGroup)Activator.CreateInstance(type)!)
            .ToArray();

        _figuresDictionary = _figureGroups.SelectMany(group => group.FigureTypes)
            .ToDictionary(figure => figure.UniqueFigureId, figure => figure);
        FigureGroupsChanged?.Invoke(this, _figureGroups);
        _figuresLoaded.TrySetResult(true);
    }
}
using System;
using System.IO;
using BattleChess3.Core.Model.Figures;

namespace BattleChess3.Core.Model;

// JSON serializable
public class MapBlueprint
{
    public static readonly MapBlueprint None = new();
    public string MapPath { get; init; } = string.Empty;
    public string PreviewPath { get; init; } = string.Empty;
    public Uri? PreviewUri => string.IsNullOrEmpty(PreviewPath) ? default : new Uri(Path.GetFullPath(PreviewPath));
    public int StartingPlayer { get; init; }
    public FigureBlueprint[] Figures { get; init; } = Array.Empty<FigureBlueprint>();
}

using CrownsGuard.Maps.Figures;

namespace CrownsGuard.Maps.GameBoard;

public class NoneTileInfo : ITileInfo
{
    public static readonly ITileInfo Instance = new NoneTileInfo();

    private NoneTileInfo() { }
    
    public Position Position => Position.None;

    public IFigureWithInfo Figure
    {
        get => FigureWithInfo.None;
        set { }
    }
}
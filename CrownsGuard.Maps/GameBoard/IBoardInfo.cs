namespace CrownsGuard.Maps.GameBoard;

public interface IBoardInfo : IEnumerable<ITileInfo>
{
    ITileInfo this[Position position] { get; }
}
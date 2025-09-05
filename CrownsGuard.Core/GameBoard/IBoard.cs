namespace CrownsGuard.Core.GameBoard;

public interface IBoard : IEnumerable<ITile>
{
    ITile this[Position position] { get; }
    
    bool HasTileOnPosition(Position position);
    
    bool TryGetTile(Position position, out ITile tile);
}
namespace CrownsGuard.Core.GameBoard;

public interface IBoard : IEnumerable<ITile>
{
    ITile this[Position position] { get; }
}
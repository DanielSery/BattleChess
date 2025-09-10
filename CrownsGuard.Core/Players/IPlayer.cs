using CrownsGuard.Core.Figures;
using CrownsGuard.Core.Helpers;

namespace CrownsGuard.Core.Players;

public interface IPlayer
{
    PlayerColor PlayerColor { get; }
    
    ArrayPoolMemory<Figure> Board { get; }
    
    public void UpdateBoard(ArrayPoolMemory<Figure> board);
}
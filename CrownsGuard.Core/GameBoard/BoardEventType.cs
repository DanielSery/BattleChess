namespace CrownsGuard.Core.GameBoard;

public enum BoardEventType : byte
{
    Attacked,
    Died,
    Moved,
    
    CreatedFigure,
    ChangedOwner,
    ChangedFigure,
}
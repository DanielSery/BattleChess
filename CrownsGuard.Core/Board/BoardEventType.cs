namespace CrownsGuard.Core.Board;

public enum BoardEventType : byte
{
    Attacked,
    Died,
    Moved,
    
    CreatedFigure,
    ChangedOwner,
    ChangedFigure,
}
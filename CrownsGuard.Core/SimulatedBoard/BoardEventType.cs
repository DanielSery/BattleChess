namespace CrownsGuard.Core.SimulatedBoard;

public enum BoardEventType : byte
{
    Attacking,
    Attacked,
    
    Dying,
    Died,
    
    Moving,
    Moved,
    
    Creating,
    Created,
    
    ChangingOwner,
    ChangedOwner,

    ChangingFigure,
    CreatedFigure,
}
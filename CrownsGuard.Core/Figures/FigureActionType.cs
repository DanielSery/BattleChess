namespace CrownsGuard.Core.Figures;

public enum FigureActionType : byte
{
    None = 0,
    PossibleAttack,
    PossibleSpecial,
    LastNonExecutable = 2,
    
    Move,
    Attack,
    Special,
}
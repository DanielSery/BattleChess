namespace CrownsGuard.Core.Figures;

public enum FigureActionType : byte
{
    None = 0,
    Move,
    PossibleAttack,
    Attack,
    PossibleSpecial,
    Special,
}
namespace CrownsGuard.Core.Figures;

[Flags]
public enum FigureActionType : ushort
{
    None = 0,
    
    Move = 20 | IsExecutable | IsMove,

    PossiblePushFigure = 21,
    PushFigure = 22 | IsExecutable | IsSpecial,
    ChangeToQueen = 23 | IsExecutable | IsSpecial, // Due to 8x more evaluation
    
    MakeUnitKing = 30 | IsExecutable | IsSpecial,
    BuildWall = 31  | IsExecutable | IsSpecial,
    MinerMove = 32 | IsExecutable | IsMove,
    BreatheFire = 33 | IsExecutable | IsSpecial,
    AlchemistMove = 34 | IsExecutable | IsMove,
    SwapWithFigure = 35 | IsExecutable | IsSpecial,

    PossibleCastling = 50,
    Castling = 50 | IsExecutable | IsSpecial,
    
    PossibleMeeleeAttack = 100 | IsMovingAttack,
    MeeleeAttack = 100  | IsTargetDependantMovingAttack,

    SpartanMove = 200 | IsBlindMovingAttack,

    PossibleMeeleePierceAttack = 203,
    MeeleePierceAttack = 203 | IsTargetDependantMovingAttack,

    BattleAxeMove = 300 | IsBlindMovingAttack,
    WarhammerMove = 301 | IsBlindMovingAttack,
    
    PossibleRangedAttack = 400,
    RangedAttack = 400 | IsExecutable | IsTargetDependant | IsAttack,

    MageMove = 501 | IsBlindMovingAttack,
    WizzardMove = 502 | IsBlindMovingAttack,
    
    PossibleConvertUnit = 600,
    ConvertUnit = 600 | IsExecutable | IsTargetDependant | IsSpecial,
    
    PossibleCannonAttack = 601,
    CannonAttack = 601 | IsExecutable | IsTargetDependant | IsAttack,

    ActionValueMask   = 0b0000001111111111,
    IsMovingAttack    = 0b0000010000000000,
    IsMove            = 0b0000100000000000,
    IsSpecial         = 0b0001000000000000,
    IsAttack          = 0b0010000000000000,
    IsExecutable      = 0b0100000000000000,
    IsTargetDependant = 0b1000000000000000,
    
    IsTargetDependantMovingAttack = IsExecutable | IsTargetDependant | IsAttack | IsMovingAttack,
    IsBlindMovingAttack = IsExecutable | IsMove | IsMovingAttack,
    
    ActionTypeMask    = 0b1111110000000000,
}
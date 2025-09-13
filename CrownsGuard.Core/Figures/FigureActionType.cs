namespace CrownsGuard.Core.Figures;

[Flags]
public enum FigureActionType : ushort
{
    None = 0,
    
    Move = 50 | IsExecutable | IsMove,
    
    PossiblePushFigure = 11,
    PushFigure = 11 | IsExecutable | IsSpecial,
    ChangeToQueen = 12 | IsExecutable | IsSpecial, // Due to 8x more evaluation

    SwapWithFigure = 20 | IsExecutable | IsSpecial,
    
    MakeUnitKing = 30 | IsExecutable | IsSpecial,
    BuildWall = 31  | IsExecutable | IsSpecial,
    MinerMove = 32 | IsExecutable | IsMove,
    BreatheFire = 33 | IsExecutable | IsSpecial,
    AlchemistMove = 34 | IsExecutable | IsMove,
    
    PossibleMeeleeAttack = 100 | IsMovingAttack,
    MeeleeAttack = 100  | IsExecutable | IsTargetDependant | IsAttack | IsMovingAttack,

    SpartanMove = 200 | IsExecutable | IsMove | IsMovingAttack,

    PossibleCastling = 202,
    Castling = 202 | IsExecutable | IsSpecial,

    BattleAxeMove = 300 | IsExecutable | IsMove | IsMovingAttack,
    WarhammerMove = 301 | IsExecutable | IsMove | IsMovingAttack,
    
    PossibleRangedAttack = 400,
    RangedAttack = 400 | IsExecutable | IsTargetDependant | IsAttack,

    PossibleMeeleePierceAttack = 401,
    MeeleePierceAttack = 401 | IsExecutable | IsTargetDependant | IsAttack | IsMovingAttack,

    MageMove = 501 | IsExecutable | IsMove | IsMovingAttack,
    WizzardMove = 502 | IsExecutable | IsMove | IsMovingAttack,
    
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
    IsTargetDependantMovingAttack = IsTargetDependant | IsMovingAttack,
}
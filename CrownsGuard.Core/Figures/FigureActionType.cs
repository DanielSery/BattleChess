namespace CrownsGuard.Core.Figures;

[Flags]
public enum FigureActionType : ushort
{
    None = 0,
    
    Move = Constants.MoveValue | IsExecutable | IsMove,

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
    MeeleeAttack = 100  | IsExecutable | IsTargeted | IsAttack | IsMovingAttack,

    SpartanMove = 200 | IsExecutable | IsMove | IsMovingAttack,

    PossibleMeeleePierceAttack = 203,
    MeeleePierceAttack = 203 | IsExecutable | IsTargeted | IsAttack | IsMovingAttack,

    BattleAxeMove = 300 | IsExecutable | IsMove | IsMovingAttack,
    WarhammerMove = 301 | IsExecutable | IsMove | IsMovingAttack,
    
    PossibleRangedAttack = 400,
    RangedAttack = 400 | IsExecutable | IsTargeted | IsAttack,

    MageMove = 501 | IsExecutable | IsMove | IsMovingAttack,
    WizzardMove = 502 | IsExecutable | IsMove | IsMovingAttack,
    
    PossibleConvertUnit = 600,
    ConvertUnit = 600 | IsExecutable | IsTargeted | IsSpecial,
    
    PossibleCannonAttack = 601,
    CannonAttack = 601 | IsExecutable | IsTargeted | IsAttack,

    ActionValueMask   = 0b0000001111111111,
    
    IsMove            = 0b0000010000000000,
    IsSpecial         = 0b0000100000000000,
    IsAttack          = 0b0001000000000000,
    ActionTypeMask    = 0b0001110000000000,
    
    IsMovingAttack    = 0b0010000000000000,
    IsTargeted        = 0b0100000000000000,
    EvaluationMask    = 0b0110000000000000,
    
    IsExecutable      = 0b1000000000000000,
}
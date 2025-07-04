using BattleChess3.Game.Board;

namespace BattleChess3.Game.Figures;

public interface IFigureType : IEquatable<IFigureType>
{
    /// <summary>
    /// Gets figure id.
    /// </summary>
    int FigureId { get; }
    
    /// <summary>
    /// Gets figure set id.
    /// </summary>
    int SetId { get; }

    /// <summary>
    /// Gets unique figure id
    /// </summary>
    int UniqueFigureId => SetId * 256 + FigureId;
    
    /// <summary>
    ///     Name shown in menus and helps
    /// </summary>
    string DisplayName { get; }
    
    /// <summary>
    ///     Gets base description of unit 
    /// </summary>
    string BaseDescription { get; }

    /// <summary>
    ///     Gets description of unit movement
    /// </summary>
    string MovementDescription { get; }

    /// <summary>
    ///     Gets description of unit attack
    /// </summary>
    string AttackDescription { get; }

    /// <summary>
    ///     Gets description of unit special abilities
    /// </summary>
    string SpecialDescription { get; }
    
    /// <summary>
    ///     Images of player with id
    /// </summary>
    IDictionary<int, Uri> ImageUris { get; }

    /// <summary>
    ///     Default equality comparison is based on unique unit name.
    /// </summary>
    bool IEquatable<IFigureType>.Equals(IFigureType? other)
    {
        return UniqueFigureId == other?.UniqueFigureId;
    }

    /// <summary>
    ///     Gets possible action on tile.
    /// </summary>
    IEnumerable<FigureAction> GetPossibleActions(ITile unitTile, IBoard board);

    void OnAttacking(ITile unitTile, ITile targetTile, IBoard board)
    {
    }

    void OnAttacked(ITile unitTile, ITile targetTile, IBoard board)
    {
    }

    void OnBeingAttacked(ITile tile, ITile attackingTile, IBoard board)
    {
    }

    void OnKilled(ITile tile, ITile attackingTile, IBoard board)
    {
    }

    void OnDying(ITile tile, IBoard board)
    {
    }
    
    void OnDied(ITile unitTile, IBoard board)
    {
    }

    void OnMoving(ITile from, ITile to, IBoard board)
    {
    }

    void OnMoved(ITile from, ITile to, IBoard board)
    {
    }

    void OnCreated(IBoard board)
    {
    }
}
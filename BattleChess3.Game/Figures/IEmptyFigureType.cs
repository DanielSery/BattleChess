namespace BattleChess3.Game.Figures;

public interface IEmptyFigureType : IFigureType
{
    int IFigureType.SetId => 0;
    
    /// <summary>
    /// Value selected for historical reasons
    /// </summary>
    public const string EmptyUnitName = "DefaultFigureGroup.Empty";
    
    string IFigureType.UnitName => EmptyUnitName;
}
using BattleChess3.Game.Figures;

namespace BattleChess3.StarWarsFigures;

public class CodyBane : IStarWarsFigureType, IShooterFigureType
{
    int IFigureType.FigureId => 4;
}
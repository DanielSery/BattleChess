using BattleChess3.Game.Figures;

namespace BattleChess3.StarWarsFigures;

public class AnakinGrievus : IStarWarsFigureType, IJediFigureType
{
    int IFigureType.FigureId => 2;
}
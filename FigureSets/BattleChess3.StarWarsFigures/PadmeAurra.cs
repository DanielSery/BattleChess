using BattleChess3.Game.Figures;

namespace BattleChess3.StarWarsFigures;

public class PadmeAurra : IStarWarsFigureType, IShooterFigureType
{
    int IFigureType.FigureId => 6;
}
using CrownsGuard.Core.Figures;
using CrownsGuard.Game.Players;
using System.Runtime.CompilerServices;

namespace CrownsGuard.Maps.Utilities
{
    public static class FigureGetHelper
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Figure GetFigure(PlayerColor playerColor, bool isKing, Figure figureType)
        {
            if (isKing) figureType |= Figure.IsKing;
            if (playerColor == PlayerColor.White) figureType |= Figure.IsWhite;
            else if (playerColor == PlayerColor.Black) figureType |= Figure.IsBlack;
            return figureType;
        }
    }
}

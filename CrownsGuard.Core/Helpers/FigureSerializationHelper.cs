using CrownsGuard.Core.SimulatedBoard;

namespace CrownsGuard.Core.Helpers;

public static class FigureSerializationHelper
{
    public static Figure FromInt(int value)
    {
        var player = PlayerSerializationHelper.ToPlayer(value & 0xFF);
        var isKing = ((value >> 8) & 1) != 0;
        var figureType = (short)((value >> 9) & 0xFFFF);
        return new Figure(player, isKing, (FigureId)figureType);
    }

    public static int ToInt(this Figure figure)
    {
        int result = 0;
        result |= figure.Player.ToInt();
        result |= (figure.IsKing ? 1 : 0) << 8;
        result |= ((int)figure.FigureType & 0xFFFF) << 9; // assumes FigureId.Value is short
        return result;
    }
}
using CrownsGuard.Core;
using CrownsGuard.Core.GameBoard;

namespace CrownsGuard.FigureDefinitions.Utilities;

internal static class BoardHelper
{
    public static int GetWithOffset(this byte absoluteIndex, short relative)
    {
        var result = absoluteIndex + (sbyte)(relative & 255);
        if ((absoluteIndex >> Constants.BoardLengthShift) != (result >> Constants.BoardLengthShift))
        {
            return -1;
        }

        result += (relative >> 5) & ~7;
        if ((uint)result >= Constants.FullBoardTilesCount)
        {
            return -1;
        }

        return result;
    }
    
    public static int GetWithOffset(this int absoluteIndex, short relative)
    {
        var result = absoluteIndex + (sbyte)(relative & 255);
        if ((absoluteIndex >> Constants.BoardLengthShift) != (result >> Constants.BoardLengthShift))
        {
            return -1;
        }

        result += (relative >> 5) & ~7;
        if ((uint)result >= Constants.FullBoardTilesCount)
        {
            return -1;
        }

        return result;
    }
    
    public static int GetWithOffset(this byte absoluteIndex, Position relative)
    {
        var result = absoluteIndex + relative.X;
        if ((absoluteIndex >> Constants.BoardLengthShift) != (result >> Constants.BoardLengthShift))
        {
            return -1;
        }
        
        result = absoluteIndex + relative.Y << Constants.BoardLengthShift;
        if ((uint)result >= Constants.FullBoardTilesCount)
        {
            return -1;
        }

        return result;
    }
}
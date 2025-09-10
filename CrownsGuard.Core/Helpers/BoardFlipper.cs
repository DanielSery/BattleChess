using CrownsGuard.Core.Figures;

namespace CrownsGuard.Core.Helpers;

public static class BoardFlipper
{
    public static ArrayPoolMemory<Figure> GetFlippedBoard(ArrayPoolMemory<Figure> figures)
    {
        if (figures.Length != Constants.FullBoardTilesCount) throw new InvalidOperationException("Invalid number of figures");

        var result = ArrayPoolHelper.Rent<Figure>(Constants.FullBoardTilesCount);
        for (var i = 0; i < Constants.BoardLength; i++)
        {
            figures.Span.Slice(i * 8, Constants.BoardLength).CopyTo(result.Span.Slice((7 - i) * 8, Constants.BoardLength));
        }
        
        return result;
    }
}
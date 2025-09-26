using System.Runtime.CompilerServices;
using CrownsGuard.Core.Figures;

namespace CrownsGuard.Core.Helpers;

public static class FigureHelper
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsEmpty(this Figure checkedFigure)
    {
        return checkedFigure == Figure.Empty;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsWalkable(this Figure checkedFigure)
    {
        return (checkedFigure & Figure.FigureMask) <= Figure.LastWalkableFigure;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool CanAttack(this Figure yoursFigure, Figure checkedFigure)
    {
        return ((yoursFigure ^ checkedFigure) & Figure.PlayerMask) != Figure.Empty &&
               (checkedFigure & Figure.FigureMask) > Figure.LastNonAttackableFigure;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsAllyTo(this Figure yoursFigure, Figure checkedFigure)
    {
        return ((yoursFigure ^ checkedFigure) & Figure.PlayerMask) == Figure.Empty;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsEnemyTo(this Figure yoursFigure, Figure checkedFigure)
    {
        return ((yoursFigure ^ checkedFigure) & Figure.PlayerMask) == Figure.PlayerMask;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Figure GetFigureType(this Figure figure)
    {
        return figure & Figure.FigureMask;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Figure GetIsKing(this Figure figure)
    {
        return figure & Figure.IsKing;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsSameColor(this Figure figure, Figure otherFigure)
    {
        return (figure & Figure.PlayerMask) == (otherFigure & Figure.PlayerMask);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Figure GetFigureColor(this Figure figure)
    {
        return figure & Figure.PlayerMask;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsWhite(this Figure figure)
    {
        return (figure & Figure.IsWhite) == Figure.IsWhite;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsKing(this Figure figure)
    {
        return (figure & Figure.IsKing) == Figure.IsKing;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsBlack(this Figure figure)
    {
        return (figure & Figure.IsBlack) == Figure.IsBlack;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNeutralFigure(this Figure figure)
    {
        return figure <= Figure.LastNeutralFigure;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNeutral(this Figure figure)
    {
        return (figure & Figure.PlayerMask) == Figure.Empty;
    }
}
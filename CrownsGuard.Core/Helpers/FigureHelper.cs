// Copyright (c) Veeam Software Group GmbH

using System.Runtime.CompilerServices;
using CrownsGuard.Core.Figures;
using CrownsGuard.Core.Players;

namespace CrownsGuard.Core.Helpers;

public static class FigureHelper
{
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
        return figure.HasFlag(Figure.IsWhite);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsKing(this Figure figure)
    {
        return figure.HasFlag(Figure.IsKing);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsBlack(this Figure figure)
    {
        return figure.HasFlag(Figure.IsBlack);
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
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Figure GetFigure(PlayerColor playerColor, bool isKing, Figure figureType)
    {
        if (isKing) figureType |= Figure.IsKing;
        if (playerColor == PlayerColor.White) figureType |= Figure.IsWhite;
        else if (playerColor == PlayerColor.Black) figureType |= Figure.IsBlack;
        return figureType;
    }
}
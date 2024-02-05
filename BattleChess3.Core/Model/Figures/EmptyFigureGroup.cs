﻿using System;

namespace BattleChess3.Core.Model.Figures;

public class EmptyFigureGroup : IFigureGroup
{
    public static EmptyFigureGroup Instance { get; } = new();
    public string DisplayName => string.Empty;
    public IFigureType[] FigureTypes { get; } = Array.Empty<IFigureType>();
}
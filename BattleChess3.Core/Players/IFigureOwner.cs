// Copyright (c) Veeam Software Group GmbH

using BattleChess3.Core.Figures;

namespace BattleChess3.Core.Players;

public interface IFigureOwner
{
    Player Player { get; }
    List<IFigure> Figures { get; }
}
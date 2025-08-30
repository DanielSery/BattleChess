// Copyright (c) Veeam Software Group GmbH

using BattleChess3.Game.Figures;

namespace BattleChess3.Game.Players;

public interface IFigureOwner
{
    Player Player { get; }
    List<IFigure> Figures { get; }
}
// Copyright (c) Veeam Software Group GmbH

namespace BattleChess3.Game.Players;

public interface IPlayerInfo : IFigureOwner
{
    IPlayerTimer Timer { get; }
    
    string Name { get; }
}
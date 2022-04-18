﻿using System;
using System.Collections.Generic;
using BattleChess3.Core.Model;
using BattleChess3.Core.Model.Figures;
using BattleChess3.DefaultFigures.Utilities;
using BattleChess3.HobbitFigures.Localization;

namespace BattleChess3.HobbitFigures;

public class Soldier : IFigureType
{
    public static readonly Soldier Instance = new();
    public string ShownName => CurrentLocalization.Instance["Soldier_Name"];
    public string Description => CurrentLocalization.Instance["Soldier_Description"];
    public string UnitName => $"{nameof(HobbitFigureGroup)}.{nameof(Soldier)}";
    public FigureTypes UnitTypes => FigureTypes.Foot;
    public FigureTypes Bonus => FigureTypes.Nothing;
    public FigureTypes AntiBonus => FigureTypes.Nothing;
    public double FullHp => 100;
    public double Attack => 100;
    public double Defence => 0;
    public bool MovingAttack => true;
    public int Cost => 1;

    public Dictionary<int, Uri> ImageUris { get; } = new Dictionary<int, Uri>
    {
        {0, new Uri("pack://application:,,,/BattleChess3.HobbitFigures;component/Images/Soldier1.png", UriKind.Absolute)},
        {1, new Uri("pack://application:,,,/BattleChess3.HobbitFigures;component/Images/Soldier2.png", UriKind.Absolute)},
    };

    public void AttackAction(ITile from, ITile to, ITile[] board)
        => to.KillFigure(board);

    public void MoveAction(ITile from, ITile to, ITile[] board)
        => from.MoveToPosition(to.Position, board);

    private readonly Position[][] _firstMoveChain = 
    {
        new Position[] {(1, 0), (2, 0)},
    };
    private readonly Position[][] _moveChain = 
    {
        new Position[] {(1, 0)},
    };
    public Position[][] GetMoveChains(Position position) 
        => position.Y == 1 ? _firstMoveChain : _moveChain;


    private readonly Position[][] _attackChain = 
    {
        new Position[] {(1, 1)},
        new Position[] {(1, -1)},
    };
    public Position[][] GetAttackChains(Position position) => _attackChain;
}

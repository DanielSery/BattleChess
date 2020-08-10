﻿using System;
using BattleChess3.Core.Figures;
using BattleChess3.Core.Models;
using BattleChess3.LordOfTheRingsFigures.Localization;

namespace BattleChess3.LordOfTheRingsFigures
{
    public class GimliNazgul : IFigureType
    {
        public static readonly GimliNazgul Instance = new GimliNazgul();
        public string ShownName => CurrentLocalization.Instance["GimliNazgul_Name"];
        public string UnitName => $"{nameof(LordOfTheRingsFigureGroup)}.{nameof(GimliNazgul)}";
        public string GroupName => nameof(LordOfTheRingsFigureGroup);
        public FigureTypes UnitTypes => FigureTypes.Foot;
        public FigureTypes Bonus => FigureTypes.Nothing;
        public FigureTypes AntiBonus => FigureTypes.Nothing;
        public int Attack => 100;
        public int Defence => 0;
        public bool MovingAttack => true;
        public int Cost => 5;
        public string Description => CurrentLocalization.Instance["GimliNazgul_Description"];
        public Position[] AttackPattern => Array.Empty<Position>();
        public bool CanMove(Tile tile, Tile[] board) => false;
        public bool CanAttack(Tile tile, Tile[] board) => false;
    }
}
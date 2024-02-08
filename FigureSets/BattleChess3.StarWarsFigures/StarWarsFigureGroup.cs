﻿using BattleChess3.Core.Model.Figures;
using BattleChess3.StarWarsFigures.Localization;

namespace BattleChess3.StarWarsFigures;

public class StarWarsFigureGroup : IFigureGroup
{
    public string DisplayName => CurrentLocalization.Instance[$"{nameof(StarWarsFigureGroup)}_Name"];

    public static readonly IFigureType AhsokaVentress = new AhsokaVentress();
    public static readonly IFigureType AnakinGrievus = new AnakinGrievus();
    public static readonly IFigureType CodyBane = new CodyBane();
    public static readonly IFigureType MaceDooku = new MaceDooku();
    public static readonly IFigureType ObiwanPalpatine = new ObiwanPalpatine();
    public static readonly IFigureType PadmeAurra = new PadmeAurra();
    public static readonly IFigureType Shield = new Bomb();
    public static readonly IFigureType Shielded = new Special();
    public static readonly IFigureType Soldiers = new Soldiers();

    public IFigureType[] FigureTypes =>
        new[]
        {
            AhsokaVentress,
            AnakinGrievus,
            CodyBane,
            MaceDooku,
            ObiwanPalpatine,
            PadmeAurra,
            Shield,
            Shielded,
            Soldiers
        };
}
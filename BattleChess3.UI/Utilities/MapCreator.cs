using System;
using System.IO;
using BattleChess3.ChessFigures;
using BattleChess3.Core.Model;
using BattleChess3.Core.Model.Figures;
using BattleChess3.Core.Utilities;
using BattleChess3.CrossFireFigures;
using BattleChess3.DefaultFigures;
using BattleChess3.LordOfTheRingsFigures;
using Newtonsoft.Json;
using Knight = BattleChess3.ChessFigures.Knight;

namespace BattleChess3.UI.Utilities;

public static class MapCreator
{
    public static void CreateCrossfireMap()
    {
        CreateMap(new MapBlueprint
        {
            Figures = new FigureBlueprint[]
                {
                    (2, Builder.Instance), (2, CrossFireFigures.Knight.Instance), (2, Archer.Instance), (2, Bomber.Instance), (2, Spy.Instance), (2, Archer.Instance), (2, CrossFireFigures.Knight.Instance), (2, Builder.Instance),
                    (2, Ninja.Instance), (2, Ninja.Instance), (2, Ninja.Instance), (2, Ninja.Instance), (2, Ninja.Instance), (2, Ninja.Instance), (2, Ninja.Instance), (2, Ninja.Instance),
                    (2, Wall.Instance), (2, Wall.Instance), (0, Empty.Instance), (0, Empty.Instance), (0, Empty.Instance), (0, Empty.Instance), (2, Wall.Instance), (2, Wall.Instance),
                    (0, Empty.Instance), (0, Empty.Instance), (0, Empty.Instance), (0, Empty.Instance), (0, Empty.Instance), (0, Empty.Instance), (0, Empty.Instance), (0, Empty.Instance),
                    (0, Empty.Instance), (0, Empty.Instance), (0, Empty.Instance), (0, Empty.Instance), (0, Empty.Instance), (0, Empty.Instance), (0, Empty.Instance), (0, Empty.Instance),
                    (1, Wall.Instance), (1, Wall.Instance), (0, Empty.Instance), (0, Empty.Instance), (0, Empty.Instance), (0, Empty.Instance), (1, Wall.Instance), (1, Wall.Instance),
                    (1, Ninja.Instance), (1, Ninja.Instance), (1, Ninja.Instance), (1, Ninja.Instance), (1, Ninja.Instance), (1, Ninja.Instance), (1, Ninja.Instance), (1, Ninja.Instance),
                    (1, Builder.Instance), (1, CrossFireFigures.Knight.Instance), (1, Archer.Instance), (1, Bomber.Instance), (1, Spy.Instance), (1, Archer.Instance), (1, CrossFireFigures.Knight.Instance), (1, Builder.Instance),
                },
            MapPath = $"Resources/Maps/Ninja_{new Random().Next()}.map",
            PreviewPath = $"/Resources/Maps/Ninja_{new Random().Next()}.png",
            StartingPlayer = 1,
        });
    }

    public static void CreateLOTRMap()
    {
        CreateMap(new MapBlueprint
        {
            Figures = new FigureBlueprint[]
                {
                    (2, LegolasNazgul.Instance), (2, SamSaruman.Instance), (2, PipinTroll.Instance), (2, GandalfWitchKing.Instance), (2, AragornSauron.Instance), (2, MerryTroll.Instance), (2, FrodoGollum.Instance), (2, GimliNazgul.Instance),
                    (2, SoldierOrc.Instance), (2, SoldierOrc.Instance), (2, SoldierOrc.Instance), (2, SoldierOrc.Instance), (2, SoldierOrc.Instance), (2, SoldierOrc.Instance), (2, SoldierOrc.Instance), (2, SoldierOrc.Instance),
                    (0, Empty.Instance), (0, Empty.Instance), (0, Empty.Instance), (0, Empty.Instance), (0, Empty.Instance), (0, Empty.Instance), (0, Empty.Instance), (0, Empty.Instance),
                    (0, Empty.Instance), (0, Empty.Instance), (0, Empty.Instance), (0, Empty.Instance), (0, Empty.Instance), (0, Empty.Instance), (0, Empty.Instance), (0, Empty.Instance),
                    (0, Empty.Instance), (0, Empty.Instance), (0, Empty.Instance), (0, Empty.Instance), (0, Empty.Instance), (0, Empty.Instance), (0, Empty.Instance), (0, Empty.Instance),
                    (0, Empty.Instance), (0, Empty.Instance), (0, Empty.Instance), (0, Empty.Instance), (0, Empty.Instance), (0, Empty.Instance), (0, Empty.Instance), (0, Empty.Instance),
                    (1, SoldierOrc.Instance), (1, SoldierOrc.Instance), (1, SoldierOrc.Instance), (1, SoldierOrc.Instance), (1, SoldierOrc.Instance), (1, SoldierOrc.Instance), (1, SoldierOrc.Instance), (1, SoldierOrc.Instance),
                    (1, LegolasNazgul.Instance), (1, SamSaruman.Instance), (1, PipinTroll.Instance), (1, GandalfWitchKing.Instance), (1, AragornSauron.Instance), (1, MerryTroll.Instance), (1, FrodoGollum.Instance), (1, GimliNazgul.Instance),
                },
            MapPath = $"Resources/Maps/LOTR_{new Random().Next()}.map",
            PreviewPath = $"/Resources/Maps/LOTR_{new Random().Next()}.png",
            StartingPlayer = 1,
        });
    }

    public static void CreateChessMap()
    {
        CreateMap(new MapBlueprint
        {
            Figures = new FigureBlueprint[]
                {
                    (2, Rook.Instance), (2, Knight.Instance), (2, Bishop.Instance), (2, Queen.Instance), (2, King.Instance), (2, Bishop.Instance), (2, Knight.Instance), (2, Rook.Instance),
                    (2, Pawn.Instance), (2, Pawn.Instance), (2, Pawn.Instance), (2, Pawn.Instance), (2, Pawn.Instance), (2, Pawn.Instance), (2, Pawn.Instance), (2, Pawn.Instance),
                    (0, Empty.Instance), (0, Empty.Instance), (0, Empty.Instance), (0, Empty.Instance), (0, Empty.Instance), (0, Empty.Instance), (0, Empty.Instance), (0, Empty.Instance),
                    (0, Empty.Instance), (0, Empty.Instance), (0, Empty.Instance), (0, Empty.Instance), (0, Empty.Instance), (0, Empty.Instance), (0, Empty.Instance), (0, Empty.Instance),
                    (0, Empty.Instance), (0, Empty.Instance), (0, Empty.Instance), (0, Empty.Instance), (0, Empty.Instance), (0, Empty.Instance), (0, Empty.Instance), (0, Empty.Instance),
                    (0, Empty.Instance), (0, Empty.Instance), (0, Empty.Instance), (0, Empty.Instance), (0, Empty.Instance), (0, Empty.Instance), (0, Empty.Instance), (0, Empty.Instance),
                    (1, Pawn.Instance), (1, Pawn.Instance), (1, Pawn.Instance), (1, Pawn.Instance), (1, Pawn.Instance), (1, Pawn.Instance), (1, Pawn.Instance), (1, Pawn.Instance),
                    (1, Rook.Instance), (1, Knight.Instance), (1, Bishop.Instance), (1, Queen.Instance), (1, King.Instance), (1, Bishop.Instance), (1, Knight.Instance), (1, Rook.Instance),
                },
            MapPath = $"Resources/Maps/Chess_{new Random().Next()}.map",
            PreviewPath = $"/Resources/Maps/Chess_{new Random().Next()}.png",
            StartingPlayer = 1,
        });
    }

    public static void CreateChess2Map()
    {
        CreateMap(new MapBlueprint
        {
            Figures = new FigureBlueprint[]
                {
                    (2, Rook.Instance), (2, Knight.Instance), (2, Bishop.Instance), (2, Queen.Instance), (2, King.Instance), (2, Bishop.Instance), (2, Knight.Instance), (2, Rook.Instance),
                    (2, Spy.Instance), (2, Pawn.Instance), (2, Pawn.Instance), (2, Pawn.Instance), (2, Pawn.Instance), (2, Pawn.Instance), (2, Pawn.Instance), (2, Pawn.Instance),
                    (0, Empty.Instance), (0, Empty.Instance), (0, Empty.Instance), (0, Empty.Instance), (0, Empty.Instance), (0, Empty.Instance), (0, Empty.Instance), (0, Empty.Instance),
                    (0, Empty.Instance), (0, Empty.Instance), (0, Empty.Instance), (0, Empty.Instance), (0, Empty.Instance), (0, Empty.Instance), (0, Empty.Instance), (0, Empty.Instance),
                    (0, Empty.Instance), (0, Empty.Instance), (0, Empty.Instance), (0, Empty.Instance), (0, Empty.Instance), (0, Empty.Instance), (0, Empty.Instance), (0, Empty.Instance),
                    (0, Empty.Instance), (0, Empty.Instance), (0, Empty.Instance), (0, Empty.Instance), (0, Empty.Instance), (0, Empty.Instance), (0, Empty.Instance), (0, Empty.Instance),
                    (1, Spy.Instance), (1, Pawn.Instance), (1, Pawn.Instance), (1, Pawn.Instance), (1, Pawn.Instance), (1, Pawn.Instance), (1, Pawn.Instance), (1, Pawn.Instance),
                    (1, Rook.Instance), (1, Knight.Instance), (1, Bishop.Instance), (1, Queen.Instance), (1, King.Instance), (1, Bishop.Instance), (1, Knight.Instance), (1, Rook.Instance),
                },
            MapPath = $"Resources/Maps/Chess_{new Random().Next()}.map",
            PreviewPath = $"/Resources/Maps/Chess_{new Random().Next()}.png",
            StartingPlayer = 1,
        });
    }

    private static void CreateMap(MapBlueprint map)
    {
        Directory.CreateDirectory("Resources/Maps");
        var text = JsonConvert.SerializeObject(map);
        text = CompressionHelper.Compress(text);
        File.WriteAllText(map.MapPath, text);
    }
}

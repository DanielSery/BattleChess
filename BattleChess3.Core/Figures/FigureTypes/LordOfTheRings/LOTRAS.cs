﻿using BattleChess3.Core.Figures.AttackingTypes;
using System;
using System.IO;

namespace BattleChess3.Core.Figures.FigureTypes.LordOfTheRings
{
    public class LOTRAS : SimpleAttackFigure, IFigure
    {
        public string ShownName => "Aragorn/Sauron";
        public string UnitName => "LOTRAS";
        public FigureType UnitType => FigureType.Foot;
        public FigureType Bonus => FigureType.Nothing;
        public FigureType AntiBonus => FigureType.Nothing;
        public int Attack => 100;
        public int Defence => 0;
        public bool MovingWhileAttacking => true;
        public int Cost => 0;

        public string Description => "\nAragorn\n\nAragorn II, the son of Arathorn II and Gilraen, also known as Elessar and Strider, was the 16th Chieftain of the Dúnedain of the North; later crowned King Elessar Telcontar (March 1, 2931 - FO 120 or SR 1541), the 26th King of Arnor, and the 35th King of Gondor and first High King of Gondor and Arnor since the short reign of Isildur. He was a great warrior, and as Isildur's heir he bore the shards of Narsil, reforged and renamed Andúril, in the War of the Ring.\n" +
            "\nSauron\n\nSauron (or Þauron (Thauron); Quenya; IPA: [ˈsaʊron] or Vanyarin; IPA: [ˈθaʊron] - \"The Abhorred\"), the eponymous Lord of the Rings, was a fallen Maia, the creator of the One Ring, and the most trusted lieutenant of his master Melkor (Morgoth, the first Dark Lord). After Melkor's defeat in the First Age, Sauron became the second Dark Lord and strove to conquer Arda by creating the Rings of Power. In the Second Age, he was defeated in the War of the Last Alliance by the last line of defense: Elves and Men under kings Gil-galad and Elendil. ";

        public string PictureBlackPath => Directory.GetCurrentDirectory() + "\\Pictures\\LordOfTheRings\\Sauron.png";
        public string PictureWhitePath => Directory.GetCurrentDirectory() + "\\Pictures\\LordOfTheRings\\Aragorn.png";
        public string PictureNeutralPath => "";

        private readonly Position[] _avaibleMoves =
        {
            new Position(0, 1),
            new Position(0, -1),
            new Position(1, 0),
            new Position(-1, 0),
            new Position(1, 1),
            new Position(-1, -1),
            new Position(1, -1),
            new Position(-1, 1),
        };

        private readonly Position[] _avaibleAttacks =
        {
            new Position(0, 1),
            new Position(0, -1),
            new Position(1, 0),
            new Position(-1, 0),
            new Position(1, 1),
            new Position(-1, -1),
            new Position(1, -1),
            new Position(-1, 1),
        };

        public Position[] AttackPattern => new[]
        {
            new Position(0, 0),
        };

        public Func<BaseFigure, BaseFigure, Func<Position, BaseFigure>, bool> CanMove => (figure, moveToFigure, x) =>
                 CanMoveSimple(figure, moveToFigure, _avaibleMoves);

        public Func<BaseFigure, BaseFigure, Func<Position, BaseFigure>, bool> CanAttack => (figure, attackFigure, x) =>
                 CanAttackSimple(figure, attackFigure, _avaibleAttacks);
    }
}
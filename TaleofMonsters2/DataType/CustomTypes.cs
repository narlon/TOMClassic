﻿namespace TaleofMonsters.DataType
{
    internal enum CardTypes
    {
        Null = 0,
        Monster = 1,
        Weapon = 2,
        Spell = 3,
    }

    internal enum CardElements
    {
        None = 0,
        Water = 1,
        Wind = 2,
        Fire = 3,
        Earth = 4,
        Light = 5,
        Dark = 6,
    }

    internal enum CardTypeSub
    {
        Devil = 1,
        Machine = 2,
        Spirit = 3,
        Insect = 4,
        Dragon = 5,
        Bird = 6,
        Crawling = 7,
        Human = 8,
        Orc = 9,
        Undead = 10,
        Beast = 11,
        Fish = 12,
        Element = 13,
        Plant = 14,
        Goblin = 15,
        Totem=16,
        Hero = 35,

        Weapon = 100,
        Scroll = 101,
        Armor = 102,
        Ring = 103,

        Single = 200,
        Multi = 201,
        Aid = 202,
        Terrain = 203,
    }

    internal static class CardQualityTypes
    {
        public const byte Common = 0;
        public const byte Good = 1;
        public const byte Excel = 2;
        public const byte Epic = 3;
        public const byte Legend = 4;
        public const byte God = 5; //不可能出现，只是做一个bound
        public const byte Trash = 6; //不可能出现，只是做一个bound
    }

    internal static class EquipQualityTypes
    {
        public const int Common = 0;
        public const int Good = 1;
        public const int Excel = 2;
        public const int Epic = 3;
        public const int Legend = 4;
    }
    internal static class HItemTypes
    {
        public const int Common = 1;
        public const int Material = 2;
        public const int Task = 3;

        public const int Fight = 11;
        public const int Gift = 12;
        public const int Item = 13;
      //  public const int Ore = 14;
        public const int DropItem = 15;
        public const int RandomCard = 16;
        public const int Seed = 17;
    }

    internal static class HItemRandomGroups
    {
        public const int None = 0;
        public const int Gather = 1;
        public const int Fight = 2;
    }

    internal enum BuffEffectTypes
    {
        NoAction = 2,
        NoSkill = 5,
        NoMove = 6,

        Tile = 101,
        Shield = 102,
        Chaos = 103,
    }
    internal class AchieveTypes
    {
        public const int Common = 1;
        public const int Resource = 2;
        public const int Fight = 3;
        public const int Summon = 4;
        public const int Battle = 5;
    }

    internal enum CardProductMarkTypes
    {
        Null,
        New,
        Hot,
        Gold,
        Only,
        Sale
    }

    internal static class TournamentTypes
    {
        public const int League4 = 1;
        public const int Cup8 = 2;
        public const int Cup16 = 3;
    }
    internal static class SceneTypes
    {
        public const int Town = 1;
        public const int Common = 2;
        public const int Dungeon = 3;
    }

    internal static class SceneQuestTypes
    {
        public const int Common = 1;
        public const int MapSetting = 2;
        public const int Hidden = 3;
    }

    internal enum RegionTypes
    {
        None = 0,
        Row = 1,
        RowForward = 2, //本行前方
        Column = 3,
        Circle = 4,
        Cross = 5,
        MultiColumn = 6,
        Grid = 7,
        All = 9,
    };

    internal enum GameResourceType
    {
        Gold = 0,
        Lumber,
        Stone,
        Mercury,
        Carbuncle,
        Sulfur,
        Gem
    }

    internal enum CardPreviewType
    {
        Normal,
        Shop
    }
    internal enum MonsterCountTypes
    {
        Total = 1,

        TypeNull = 10,
        TypeWater = 11,
        TypeWind = 12,
        TypeFire = 13,
        TypeEarth = 14,
        TypeLight = 15,
        TypeDark = 16,

        RaceDevil = 21,
        RaceMachine = 22,
        RaceSpirit = 23,
        RaceInsect = 24,
        RaceDragon = 25,
        RaceBird = 26,
        RaceCrawling = 27,
        RaceHuman = 28,
        RaceOrc = 29,
        RaceUndead = 30,
        RaceBeast = 31,
        RaceFish = 32,
        RaceElement = 33,
        RacePlant = 34,
        RaceGoblin = 35,
        RaceTotem = 36,
        RaceHero = 55,
    }
    internal enum QuestionTypes
    {
        MonsterInfo = 1,
        InfoMonster = 2,
        WeaponInfo = 3,
        InfoWeapon = 4,
        SpellInfo = 5,
        InfoSpell = 6,
        SkillInfo = 7,
        InfoSkill = 8,
    }
    internal enum SkillSourceTypes
    {
        Monster,
        Weapon,
        Equip,
        Skill,//技能给技能
    }
    internal enum EquipAttrs
    {
        AtkRate,
        HpRate,
        Spd,
        Range,
        Def,
        Mag,
        Hit,
        Dhit,
        Crt,
        Luk
    }

    internal enum MemPlayerRecordTypes
    {
        TotalWin = 1,
        TotalDig = 2,
        TotalPick = 3,
        TotalFish = 4,
        TotalKill = 5,
        TotalSummon = 6,
        TotalWeapon = 7,
        TotalSpell = 8,
        TotalOnline = 9,
        ContinueWin = 10,
        TotalKillByType = 100, // size 9
        TotalKillByRace = 200, //16
        TotalKillByLevel = 300, //9

        TotalSummonByType = 400, //17
        TotalSummonByRace = 500, //16
        TotalSummonByLevel = 600, //9

        LastCardShopTime = 1000,
        LastMergeTime = 1001,
        LastBlessShopTime = 1002,
       // LastNpcPieceTime = 1003,
        LastQuestionTime = 1004,
        HeroExpPoint = 1100,//经验罐的exp

        SceneQuestRandPeopleId = 1101,
        DungeonQuestOffside = 1102,
    }

    internal enum MemPlayerFlagTypes
    {
        SelectJob=1,
        Qiaqia=2,
        Aolai=3,
        Qiongqi =4,
        AolaiFight=5,
        Kedi=6
    }
}
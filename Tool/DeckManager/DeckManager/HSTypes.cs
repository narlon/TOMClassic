﻿namespace DeckManager
{
    public class HSTypes
    {
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
            Totem = 16,
            NormalTower = 34,
            KingTower = 35,

            Weapon = 100,
            Scroll = 101,
            Armor = 102,
            Ring = 103,

            Single = 200,
            Multi = 201,
            Aid = 202,
            Terrain = 203,
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

        public static string I2QualityColor(int id)
        {
            string[] rt = { "White", "Green", "DodgerBlue", "Violet", "Orange", "Gray", "Gray", "", "", "Yellow" };
            return rt[id];
        }
        public static string I2Attr(int aid)
        {
            string[] rt = { "无", "水", "风", "火", "地", "光", "暗" };
            return rt[aid];
        }

        public static string I2CardTypeSub(int rid)
        {
            switch (rid)
            {
                case 1: return "恶魔";
                case 2: return "机械";
                case 3: return "精灵";
                case 4: return "昆虫";
                case 5: return "龙";
                case 6: return "鸟";
                case 7: return "爬行";
                case 8: return "人类";
                case 9: return "兽人";
                case 10: return "亡灵";
                case 11: return "野兽";
                case 12: return "鱼";
                case 13: return "元素";
                case 14: return "植物";
                case 15: return "地精";
                case 16: return "石像";
                case 34: return "建筑";
                case 35: return "城堡";

                case 100: return "武器";
                case 101: return "卷轴";
                case 102: return "防具";
                case 103: return "饰品";

                case 200: return "单体法术";
                case 201: return "群体法术";
                case 202: return "基本法术";
                case 203: return "地形变化";
            }

            return "";
        }
    }
}
﻿using System.Collections.Generic;
using System.IO;
using ConfigDatas;
using TaleofMonsters.Controler.Loader;
using System.Drawing;

namespace TaleofMonsters.DataType.Maps
{
    internal static class BattleMapBook
    {
        static Dictionary<string, BattleMap> mapType = new Dictionary<string, BattleMap>();

        public static BattleMap GetMap(string name)
        {
            if (!mapType.ContainsKey(name))
            {
                mapType.Add(name, GetMapFromFile(string.Format("{0}.map", name)));
            }
            return mapType[name];
        }

        private static BattleMap GetMapFromFile(string name)
        {
            StreamReader sr = new StreamReader(DataLoader.Read("Map", name));
            BattleMap map = new BattleMap();
            var datas = sr.ReadLine().Split('\t');
            map.XCount = int.Parse(datas[0]);
            map.YCount = int.Parse(datas[1]);
            map.Cells = new int[map.XCount, map.YCount];
            for (int i = 0; i < map.YCount; i++)
            {
                string line = sr.ReadLine();
                if (line != null)
                {
                    string[] mapinfos = line.Split('\t');
                    for (int j = 0; j < map.XCount; j++)
                    {
                        map.Cells[j, i] = int.Parse(mapinfos[j]);
                    }
                }
            }
            var unitCount = int.Parse(sr.ReadLine());//左边单位布置
            map.LeftUnits = new BattleMapUnitInfo[unitCount];
            for (int i = 0; i < unitCount; i++)
            {
                string[] unitinfos = sr.ReadLine().Split('\t');
                map.LeftUnits[i] = new BattleMapUnitInfo
                    {
                        X = int.Parse(unitinfos[0]),
                        Y = int.Parse(unitinfos[1]),
                        UnitId = int.Parse(unitinfos[2])
                    };
            }

            unitCount = int.Parse(sr.ReadLine());//右边单位布置
            map.RightUnits = new BattleMapUnitInfo[unitCount];
            for (int i = 0; i < unitCount; i++)
            {
                string[] unitinfos = sr.ReadLine().Split('\t');
                map.RightUnits[i] = new BattleMapUnitInfo
                {
                    X = int.Parse(unitinfos[0]),
                    Y = int.Parse(unitinfos[1]),
                    UnitId = int.Parse(unitinfos[2])
                };
            }
            sr.Close();
            return map;
        }

        public static Image GetMapImage(string name, int nowtile)
        {
            Image img = new Bitmap(100, 100);
            BattleMap mapInfo = GetMap(name);
            Graphics g = Graphics.FromImage(img);

            int cellSize = 100/mapInfo.XCount;
            int yOff = (100 - cellSize*mapInfo.YCount)/2;
            for (int i = 0; i < mapInfo.XCount; i++)
            {
                for (int j = 0; j < mapInfo.YCount; j++)
                {
                    var tile = mapInfo.Cells[i, j];
                    if (tile == 0)
                    {
                        tile = nowtile == 0 ? TileConfig.Indexer.DefaultTile : nowtile;
                    }
                    Brush sBrush = new SolidBrush(Color.FromName(ConfigData.GetTileConfig(tile).Color));
                    g.FillRectangle(sBrush, i * cellSize + 1, j * cellSize + yOff + 1, cellSize, cellSize);
                    g.DrawRectangle(Pens.White, i * cellSize + 1, j * cellSize + yOff + 1, cellSize, cellSize);
                    sBrush.Dispose();
                }
            }
            g.Dispose();
            return img;
        }
    }
}

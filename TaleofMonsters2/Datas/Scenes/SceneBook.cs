﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using ConfigDatas;
using NarlonLib.Math;
using TaleofMonsters.Core;
using TaleofMonsters.Core.Loader;
using TaleofMonsters.Datas.User;
using TaleofMonsters.Forms.CMain.Scenes;
using TaleofMonsters.Tools;

namespace TaleofMonsters.Datas.Scenes
{
    internal static class SceneBook
    {
        public static int GetEnemyGroupIdByName(string f)
        {
            foreach (var enemyData in ConfigData.SceneEnemyGroupDict.Values)
            {
                if (enemyData.Ename == f)
                    return enemyData.Id;
            }

            return 0;
        }

        public static int GetRandomEnemy(int mapId, bool isElite)
        {
            var groupId = GetEnemyGroupIdByName(ConfigData.GetSceneConfig(mapId).EnemyGroup);
            if (groupId > 0)
            {
                var enemyGroupConfig = ConfigData.GetSceneEnemyGroupConfig(groupId);
                if(!isElite)
                    return enemyGroupConfig.EnemyIds[MathTool.GetRandom(enemyGroupConfig.EnemyIds.Length)];
                return enemyGroupConfig.EliteIds[MathTool.GetRandom(enemyGroupConfig.EliteIds.Length)];
            }
            return 0;
        }

        public static SceneInfo LoadSceneFile(int id, int mapWidth, int mapHeight, Random r)
        {
            var filePath = ConfigData.GetSceneConfig(id).TilePath;
            StreamReader sr = new StreamReader(DataLoader.Read("Scene", string.Format("{0}.txt", filePath)));
            SceneInfo info = new SceneInfo(id);

            string line;
            while ((line = sr.ReadLine()) != null)
            {
                string[] datas = line.Split('=');
                string tp = datas[0].Trim();
                string parm = datas[1].Trim();
                switch (tp)
                {
                    case "startx": info.Xoff = int.Parse(parm) * mapWidth / 1422; break;
                    case "starty": info.Yoff = int.Parse(parm) * mapHeight / 855 + 50; break; //50为固定偏移
                    case "width": info.XCount = int.Parse(parm); break;
                    case "height": info.YCount = int.Parse(parm); break;
                    case "startpoint": info.StartPos = int.Parse(parm); break;
                    case "revivepoint": info.RevivePos = int.Parse(parm); break;
                    case "data": ReadBody(sr, mapWidth, mapHeight, r, info); break;
                }
            }

            sr.Close();

            return info;
        }

        private static void ReadBody(StreamReader sr, int mapWidth, int mapHeight, Random r, SceneInfo info)
        {
            int cellWidth = GameConstants.SceneTileStandardWidth * mapWidth / 1422;
            int cellHeight = GameConstants.SceneTileStandardHeight * mapHeight / 855;
            Dictionary<int, List<SceneInfo.SceneScriptPosData>> randomGroup = new Dictionary<int, List<SceneInfo.SceneScriptPosData>>();
            for (int i = 0; i < info.YCount; i++)
            {
                string[] datas = sr.ReadLine().Split('\t');
                for (int j = 0; j < info.XCount; j++)
                {
                    string numberStr = datas[j];
                    char cellTag = (char)0;
                    if (numberStr[0] >= 'a' && numberStr[0] <= 'z') //是个字母
                    {
                        cellTag = numberStr[0];
                        numberStr = numberStr.Substring(1);
                    }
                    int cellIndex = int.Parse(numberStr);
                    if (cellIndex == 0)
                    {
                        continue;
                    }

                    int lineOff = (int)(cellWidth * (info.YCount - i - 1) * GameConstants.SceneTileGradient);
                    SceneInfo.SceneScriptPosData so = new SceneInfo.SceneScriptPosData
                    {
                        Id = cellIndex,
                        X = info.Xoff + j * cellWidth + lineOff,
                        Y = info.Yoff + i * cellHeight,
                        Width = cellWidth,
                        Height = cellHeight
                    };
                    if (cellTag == 'r') //随机组
                    {
                        so.Id = (info.YCount - i) * 1000 + j + 1;
                        if (!randomGroup.ContainsKey(cellIndex))
                            randomGroup[cellIndex] = new List<SceneInfo.SceneScriptPosData>();
                        randomGroup[cellIndex].Add(so);
                    }
                    else if (cellTag == 'h') //隐藏组
                    {
                        so.HiddenIndex = cellIndex;
                        so.Id = (info.YCount - i) * 1000 + j + 1;
                        info.MapData.Add(so);
                        info.HiddenCellCount++;
                    }
                    else
                    {
                        info.MapData.Add(so);
                    }
                }
            }

            RandomSequence rs = new RandomSequence(randomGroup.Count, r);
            for (int i = 0; i < Math.Ceiling(randomGroup.Keys.Count * 0.5f); i++)
                foreach (var randPos in randomGroup[rs.NextNumber() + 1])
                    info.MapData.Add(randPos);

            string line;
            while ((line = sr.ReadLine()) != null)
            {
                string[] data = line.Split('\t');
                if (data.Length < 2)
                    continue;

                var posData = new SceneInfo.SceneScriptSpecialData();
                posData.Id = int.Parse(data[0]);
                posData.Type = (SceneCellTypes)Enum.Parse(typeof(SceneCellTypes), data[1]);
                if (data.Length > 2)
                    posData.Info = int.Parse(data[2]);
                info.SpecialData.Add(posData);
            }
        }

        public static Image GetScenePreview(Scene scene)
        {
            var config = ConfigData.GetSceneConfig(UserProfile.InfoBasic.MapId);
            ControlPlus.TipImage tipData = new ControlPlus.TipImage(PaintTool.GetTalkColor);
            tipData.AddTextNewLine(string.Format("{0}(Lv{1})", config.Name, config.Level), "LightBlue", 20);
            tipData.AddLine(2);
            tipData.AddTextNewLine(string.Format(" 温度:{0}", HSTypes.I2TemperatureName(config.Temperature)), "Red");
            tipData.AddTextNewLine(string.Format(" 湿度:{0}", HSTypes.I2HumitityName(config.Humitity)), "LightBlue");
            tipData.AddTextNewLine(string.Format(" 高度:{0}", HSTypes.I2AltitudeName(config.Altitude)), "LightGreen");
            tipData.AddLine(2);
            tipData.AddTextNewLine(string.Format("格子:{0}", scene.SceneInfo.Items.Count), "White");

            if (UserProfile.InfoDungeon.DungeonId <= 0) //普通场景
            {
                foreach (var questData in SceneQuestBook.GetQuestConfigData(UserProfile.InfoBasic.MapId))
                {
                    var questConfig = ConfigData.GetSceneQuestConfig(questData.Id);
                    if (questConfig.Type == (int)SceneQuestTypes.Rare || questConfig.TriggerRate > 0) //概率事件不显示
                        continue;
                    var happend = scene.GetDisableEventCount(questData.Id);
                    var evtLevel = questConfig.Level == 0 ? config.Level : questConfig.Level;
                    tipData.AddTextNewLine(string.Format(" {0}Lv{3}({1}/{2})", questConfig.Name,
                        happend, questData.Value, evtLevel), happend == questData.Value ? "DimGray" : HSTypes.I2QuestDangerColor(questConfig.Danger));
                }
            }
            else if(UserProfile.InfoDungeon.Items.Count > 0)
            {
                tipData.AddLine(1);
                foreach (var pickItem in UserProfile.InfoDungeon.Items)
                {
                    var itemConfig = ConfigData.GetDungeonItemConfig(pickItem.Type);
                    tipData.AddTextNewLine(string.Format(" {0}x", itemConfig.Name), "White");
                    tipData.AddText(string.Format("{0}", pickItem.Value), "Lime");
                }
            }

            return tipData.Image;
        }
    }

}

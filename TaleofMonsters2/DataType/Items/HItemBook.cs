﻿using System.Collections.Generic;
using System.Drawing;
using ConfigDatas;
using NarlonLib.Drawing;
using NarlonLib.Math;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.Controler.Resource;
using TaleofMonsters.Core;
using TaleofMonsters.DataType.Others;

namespace TaleofMonsters.DataType.Items
{
    internal static class HItemBook
    {
        private static Dictionary<int, Dictionary<int, List<int>>> rareMidDict;//随机组，稀有度

        public static int GetRandRareItemId(int group, int rare)
        {
            if (rareMidDict == null)
            {
                rareMidDict = new Dictionary<int, Dictionary<int, List<int>>>();
                rareMidDict[1] = new Dictionary<int, List<int>>();
                rareMidDict[2] = new Dictionary<int, List<int>>();//目前只有2个随机组
                foreach (var hItemConfig in ConfigData.HItemDict.Values)
                {
                    if (hItemConfig.RandomGroup > 0)
                    {
                        if (!rareMidDict[hItemConfig.RandomGroup].ContainsKey(hItemConfig.Rare))
                        {
                            rareMidDict[hItemConfig.RandomGroup].Add(hItemConfig.Rare, new List<int>());
                        }
                        rareMidDict[hItemConfig.RandomGroup][hItemConfig.Rare].Add(hItemConfig.Id);
                    }
                }
            }

            var rareList = rareMidDict[group][rare];
            return rareList[MathTool.GetRandom(rareList.Count)];
        }

        public static Image GetHItemImage(int id)
        {
            HItemConfig hItemConfig = ConfigData.GetHItemConfig(id);

            string fname = string.Format("Item/{0}", hItemConfig.Url);
            if (!ImageManager.HasImage(fname))
            {
                Image image = PicLoader.Read("Item", string.Format("{0}.JPG", hItemConfig.Url));
                ImageManager.AddImage(fname, image);
            }
            return ImageManager.GetImage(fname);
        }

        public static Image GetPreview(int id)
        {
            HItemConfig hItemConfig = ConfigData.GetHItemConfig(id);
            if (hItemConfig.Id <= 0)
            {
                return DrawTool.GetImageByString("unknown", 100);
            }

            ControlPlus.TipImage tipData = new ControlPlus.TipImage();
            tipData.AddTextNewLine(hItemConfig.Name, HSTypes.I2RareColor(hItemConfig.Rare), 20);
            if (hItemConfig.IsUsable)
            {
                if (hItemConfig.SubType == HItemTypes.Fight)
                {
                    tipData.AddTextNewLine("       战斗中双击使用", "Red");
                }
                else if (hItemConfig.SubType == HItemTypes.Seed)
                {
                    tipData.AddTextNewLine("       农场中双击使用", "Red");
                }
                else
                {
                    tipData.AddTextNewLine("       双击使用", "Green");
                }
            }
            else if (hItemConfig.SubType == HItemTypes.Task)
            {
                tipData.AddTextNewLine("       任务物品", "DarkBlue");
            }
            else if (hItemConfig.SubType == HItemTypes.Material)
            {
                tipData.AddTextNewLine(string.Format("       材料(稀有度:{0})", hItemConfig.Rare), "White");
            }
            else
            {
                tipData.AddTextNewLine("", "White");
            }
            tipData.AddTextNewLine(string.Format("       等级:{0}", hItemConfig.Level), "White");
            tipData.AddTextNewLine("", "White");
            tipData.AddTextLines(hItemConfig.Descript, "White",20,true);
            if (hItemConfig.SubType == HItemTypes.RandomCard)
            {
                var consumerConfig = ConfigData.GetItemConsumerConfig(hItemConfig.Id);
                int totalRate = 0;
                foreach (var rate in consumerConfig.RandomCardRate)
                    totalRate += rate;
                tipData.AddLine();
                tipData.AddTextNewLine("抽卡概率", "White");
                tipData.AddTextNewLine("", "White");
                tipData.AddImage(HSIcons.GetIconsByEName("gem5"));
                tipData.AddText(string.Format("{0:0}%  ", (float)consumerConfig.RandomCardRate[3]*100/ totalRate), "White");
                tipData.AddImage(HSIcons.GetIconsByEName("gem4"));
                tipData.AddText(string.Format("{0:0}%  ", (float)consumerConfig.RandomCardRate[2] * 100 / totalRate), "White");
                tipData.AddLine();
            }
            tipData.AddTextNewLine(string.Format("出售价格:{0}", GameResourceBook.InGoldSellItem(hItemConfig.Rare, hItemConfig.ValueFactor)), "Yellow");
            tipData.AddImage(HSIcons.GetIconsByEName("res1"));
            tipData.AddImageXY(GetHItemImage(id), 8, 8, 48, 48, 7, 24, 32, 32);

            return tipData.Image;
        }

    }
}

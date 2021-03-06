﻿using System;
using System.Collections.Generic;
using System.Drawing;
using ConfigDatas;
using NarlonLib.Log;
using NarlonLib.Math;
using TaleofMonsters.Core.Loader;
using TaleofMonsters.Tools;

namespace TaleofMonsters.Datas.Skills
{
    internal static class SkillBook
    {
        private static List<int> randomSkillIds;

        public static int GetRandSkillId()
        {
            if (randomSkillIds == null)
            {
                randomSkillIds = new List<int>();
                foreach (SkillConfig skillConfig in ConfigData.SkillDict.Values)
                {
                    if (skillConfig.IsRandom)
                        randomSkillIds.Add(skillConfig.Id);
                }
            }
            return randomSkillIds[MathTool.GetRandom(randomSkillIds.Count)];            
        }

        public static string GetAttrByString(int id, string info)
        {
            SkillConfig skillConfig = ConfigData.GetSkillConfig(id);

            switch (info)
            {
                case "type": return skillConfig.Type;
                case "des": return GetSkillDes(id, 1);
            }
            return "";
        }

        private static float GetSkillMark(int id, float rate)
        {
            SkillConfig skillConfig = ConfigData.GetSkillConfig(id);
            return skillConfig.Mark*rate;
        }

        public static int GetSkillQuality(int id, float rate)
        {
            var mark = GetSkillMark(id, rate/100);
            if (mark <= 2)
                return (int)QualityTypes.Common;
            if (mark <= 8)
                return (int)QualityTypes.Good;
            if (mark <= 16)
                return (int)QualityTypes.Excel;
            if (mark <= 30)
                return (int)QualityTypes.Epic;
            return (int)QualityTypes.Legend;
        }

        public static Image GetSkillImage(int id)
        {
            return GetSkillImage(id, 64, 64);
        }

        public static Image GetSkillImage(int id, int width, int height)
        {
            SkillConfig skillConfig = ConfigData.GetSkillConfig(id);

            string fname = string.Format("Skill/{0}{1}x{2}", id, width, height);
            if (!ImageManager.HasImage(fname))
            {
                Image image = PicLoader.Read("Skill", string.Format("{0}.JPG", skillConfig.Icon));
                if (image == null)
                {
                    NLog.Error("GetSkillImage {0} {1} not found", id, fname);
                    return null;
                }

#if DEBUG
                if (skillConfig.Remark.Contains("未完成"))
                {
                    Graphics g = Graphics.FromImage(image);
                    var icon = PicLoader.Read("System", "NotFinish2.PNG");
                    g.DrawImage(icon, 0, 0, 64, 64);
                    g.Save();
                    g.Dispose();
                }
#endif

                if (image.Width != width || image.Height != height)
                    image = image.GetThumbnailImage(width, height, null, new IntPtr(0));
                ImageManager.AddImage(fname, image);
            }
            return ImageManager.GetImage(fname);
        }

        public static Image GetSkillImageSpecial(string name)
        {
            var width = 64;
            var height = 64;
            string fname = string.Format("Skill/{0}{1}x{2}", name, width, height);
            if (!ImageManager.HasImage(fname))
            {
                Image image = PicLoader.Read("Skill", string.Format("{0}.JPG", name));
                if (image == null)
                {
                    NLog.Error("GetSkillImage {0} {1} not found", name, fname);
                    return null;
                }

                if (image.Width != width || image.Height != height)
                    image = image.GetThumbnailImage(width, height, null, new IntPtr(0));
                ImageManager.AddImage(fname, image);
            }
            return ImageManager.GetImage(fname);
        }

        public static string GetSkillDes(int skillId, int level)
        {
            var skillConfig = ConfigData.GetSkillConfig(skillId);
            if (skillConfig.RelatedBuffId > 0)
                return skillConfig.GetDescript(level) +
                       ConfigDatas.ConfigData.GetBuffConfig(skillConfig.RelatedBuffId).GetDescript(level);
            return skillConfig.GetDescript(level);
        }
    }
}

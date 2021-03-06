﻿using ConfigDatas;

namespace TaleofMonsters.Datas.Skills
{
    internal class Skill : ISkill
    {
        public SkillConfig SkillConfig { get; private set; }

        public int Id
        {
            get { return SkillConfig.Id; }
        }

        public int Level { get; set; }
        public IMonster Owner { get; private set; }

        public string Name
        {
            get { return SkillConfig.Name; }
        }

        public Skill(IMonster p, int id)
        {
            Owner = p;
            SkillConfig = ConfigData.GetSkillConfig(id);
            Level = 1;
        }

        public string Target
        {
            get { return SkillConfig.Target.Substring(1, 1); }
        }

        public string Shape
        {
            get { return SkillConfig.Target.Substring(2, 1); }
        }

        public int Range
        {
            get { return SkillConfig.Range; }
        }

        public string Descript
        {
            get
            {
                if (SkillConfig.RelatedBuffId > 0)
                    return SkillConfig.GetDescript(Level) + ConfigDatas.ConfigData.GetBuffConfig(SkillConfig.RelatedBuffId).GetDescript(Level);
                return SkillConfig.GetDescript(Level);
            }
        }

        public Skill UpgradeToLevel(int newLevel)
        {
            Level = newLevel;
            return this;
        }
    }
}

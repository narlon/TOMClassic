﻿using System.Collections.Generic;
using TaleofMonsters.Controler.Battle.Tool;
using TaleofMonsters.Datas.Effects;
using TaleofMonsters.Datas.Effects.Facts;

namespace TaleofMonsters.Controler.Battle.Data.MemMonster.Component
{
    /// <summary>
    /// 特殊技能造成的遮罩盒子
    /// </summary>
    internal class MonsterCoverBox
    {
        private LiveMonster self;
        private List<MonsterBindEffect> coverEffectList = new List<MonsterBindEffect>();//变身时需要重算

        public MonsterCoverBox(LiveMonster lm)
        {
            self = lm;
            CheckCover();
        }

        public void CheckCover()
        {
            string cover = self.Avatar.MonsterConfig.Cover;
            if (!string.IsNullOrEmpty(cover))
            {
                MonsterBindEffect ef = new MonsterBindEffect(EffectBook.GetEffect(cover), self, true);
                ef.Repeat = true;
                BattleManager.Instance.EffectQueue.Add(ef);
                coverEffectList.Add(ef);
            }

            self.SkillManager.CheckCover(coverEffectList);
        }

        public void RemoveAllCover()
        {
            foreach (var activeEffect in coverEffectList)
                activeEffect.IsFinished = RunState.Finished;
            coverEffectList.Clear();
        }
    }
}
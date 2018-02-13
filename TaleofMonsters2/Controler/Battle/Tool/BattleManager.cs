﻿using System.Drawing;
using TaleofMonsters.Controler.Battle.Data;
using TaleofMonsters.Controler.Battle.Data.MemMap;
using TaleofMonsters.Controler.Battle.Data.MemMonster;
using TaleofMonsters.Controler.Battle.DataTent;
using TaleofMonsters.Core;
using TaleofMonsters.DataType;

namespace TaleofMonsters.Controler.Battle.Tool
{
    internal class BattleManager
    {
        static BattleManager instance;
        public static BattleManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new BattleManager();
                }
                return instance;
            }
        }

        public EffectQueue EffectQueue { get; set; }
        public FlowWordQueue FlowWordQueue { get; set; }
        public MonsterQueue MonsterQueue { get; set; }
        public BattleStatisticData StatisticData { get; set; }//统计数据
        public BattleRuleData RuleData { get; set; }//规则数据
        public PlayerManager PlayerManager { get; set; }
        public MissileQueue MissileQueue { get; set; }
        public MemRowColumnMap MemMap { get; set; }

        public bool IsNight;
        public int RoundMark;//目前一个roundmark代表0.05s
        public float Round;//当前的回合数，超过固定值就可以抽牌

        public BattleManager()
        {
            Init();
        }

        public void Init()
        {
            EffectQueue = new EffectQueue();
            FlowWordQueue = new FlowWordQueue();
            MonsterQueue = new MonsterQueue();
            StatisticData = new BattleStatisticData();
            RuleData = new BattleRuleData();
            PlayerManager = new PlayerManager();
            MissileQueue = new MissileQueue();
            IsNight = false;
        }

        public void Next()
        {
            RoundMark++;

            FlowWordQueue.Next();
            EffectQueue.Next();
            MissileQueue.Next();

            if (RoundMark % 4 == 0) //200ms
            {
                float pastRound = (float)200 / GameConstants.RoundTime;

                MonsterQueue.NextAction(pastRound); //1回合

                PlayerManager.Update(false, pastRound, StatisticData.Round);

                Round += pastRound;
                if (Round >= 1)
                {
                    Round = 0;
                    PlayerManager.CheckRoundCard(); //1回合
                }
            }
            StatisticData.Round = RoundMark * 50 / GameConstants.RoundTime + 1;//50ms
            if (RoundMark % 10 == 0)
                AIStrategy.AIProc(PlayerManager.RightPlayer);
        }

        public void Draw(Graphics g, MagicRegion magicRegion, CardVisualRegion visualRegion, int mouseX, int mouseY, bool isMouseIn)
        {
            MemMap.Draw(g);//画地图
            visualRegion.Draw(g);
            if (magicRegion.Active && isMouseIn)
                magicRegion.Draw(g, RoundMark, mouseX, mouseY);
            for (int i = 0; i < MonsterQueue.Count; i++)
            {
                LiveMonster monster = MonsterQueue[i];
                Color color = Color.White;
                if (isMouseIn)
                    color = magicRegion.GetMonsterColor(monster, mouseX, mouseY);
                monster.DrawOnBattle(g, color);//画怪物
            }

            for (int i = 0; i < MissileQueue.Count; i++)
                MissileQueue[i].Draw(g);//画导弹

            for (int i = 0; i < EffectQueue.Count; i++)
                EffectQueue[i].Draw(g);//画特效
            for (int i = 0; i < FlowWordQueue.Count; i++)
                FlowWordQueue[i].Draw(g);//画飘字

        }
    }
}
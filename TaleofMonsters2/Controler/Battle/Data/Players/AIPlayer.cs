﻿using System;
using System.Collections.Generic;
using ConfigDatas;
using NarlonLib.Tools;
using TaleofMonsters.Controler.Battle.Data.MemCard;
using TaleofMonsters.Controler.Battle.Tool;
using TaleofMonsters.Core;
using TaleofMonsters.Datas.Decks;
using TaleofMonsters.Datas.Equips;

namespace TaleofMonsters.Controler.Battle.Data.Players
{
    internal class AIPlayer : Player
    {
        public AIPlayer(int id, string deck, bool isLeft, int rlevel)
            : base(isLeft)
        {
            PeopleId = id;

            Level = rlevel;
            PeopleConfig peopleConfig = ConfigData.GetPeopleConfig(id);
            Job = peopleConfig.Job;
            
            DeckCard[] cds = DeckBook.GetDeckByName(deck, Level);
            ArraysUtils.RandomShuffle(cds);
            if (peopleConfig.CardReduce > 0) //有的野怪卡会比较少
            {
                noCardOutPunish = true;
                cds = ArraysUtils.GetSubArray(cds, 0, GameConstants.DeckCardCount - peopleConfig.CardReduce);
            }
            OffCards = new CardOffBundle(cds);

            int[] energyRate = { 0, 0, 0 };
            CalculateEquipAndSkill(new List<Equip>(), energyRate); //todo
            EnergyGenerator.SetRateNpc(energyRate, peopleConfig);
            EnergyGenerator.Next(0);

            BattleManager.Instance.RuleData.CheckPlayerData(this);
        }
        
        public override void InitialCards()
        {
            base.InitialCards();
        }

        public override List<int> GetInitialMonster()
        {
            if (IsLeft)
                return new List<int>();

            var list = new List<int>();
            PeopleConfig peopleConfig = ConfigData.GetPeopleConfig(PeopleId);
            if (peopleConfig.RightMon != null && peopleConfig.RightMon.Length>0)
                list.AddRange(peopleConfig.RightMon);
            BattleManager.Instance.RuleData.CheckInitialMonster(this, list, peopleConfig.PetMon);//会修改player.InitialMonster
            return list;
        }

    }
}

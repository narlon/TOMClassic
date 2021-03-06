﻿using System.Drawing;
using ConfigDatas;
using TaleofMonsters.Controler.Battle.Data.MemCard;
using TaleofMonsters.Controler.Battle.Data.MemFlow;
using TaleofMonsters.Controler.Battle.Tool;
using TaleofMonsters.Core;
using TaleofMonsters.Datas;
using TaleofMonsters.Datas.CardPieces;
using TaleofMonsters.Datas.Decks;
using TaleofMonsters.Datas.User;

namespace TaleofMonsters.Controler.Battle.Data.Players
{
    internal class HumanPlayer : Player
    {
        public HumanPlayer(bool isLeft, DeckCard[] cardInitial)
            : base(isLeft)
        {
            PeopleId = 0;
            Level = UserProfile.InfoBasic.Level;
            Job = UserProfile.InfoBasic.Job;

            if (UserProfile.InfoDungeon.DungeonId > 0)
                Job = UserProfile.InfoDungeon.JobId;

            OffCards = new CardOffBundle(cardInitial);

            int[] energyRate = {0, 0, 0};
            CalculateEquipAndSkill(UserProfile.InfoCastle.GetValidEquipsList(), energyRate);
            EnergyGenerator.SetRate(energyRate, Job);
            EnergyGenerator.Next(0);
            
            BattleManager.Instance.RuleData.CheckPlayerData(this);
        }

        public override void AddResource(GameResourceType type, int number)
        {
            if (number > 0)
                UserProfile.InfoBag.AddResource(type, (uint)number);
            else if (number < 0)
                UserProfile.InfoBag.SubResource(type, (uint)(-number));
        }

        public override void InitialCards()
        {
            base.InitialCards();

#if DEBUG
            int[] cardToGive = new[] { 52000132 };
            foreach (var cardId in cardToGive)
                HandCards.AddCard(new ActiveCard(cardId, 1));
#endif
        }

        public override void OnKillMonster(int id, int dieLevel, int dieStar, Point position)
        {
            base.OnKillMonster(id, dieLevel, dieStar, position);

            if (IsLeft)
            {
                if (BattleManager.Instance.StatisticData.Items.Count < GameConstants.MaxDropItemGetOnBattle)
                {
                    int itemId = CardPieceBook.CheckPieceDrop(id, Luk);
                    if (itemId > 0)
                    {
                        BattleManager.Instance.StatisticData.AddItemGet(itemId);
                        BattleManager.Instance.FlowWordQueue.Add(new FlowItemInfo(itemId, position, 20, 50));
                    }
                    var monConfig = ConfigData.GetMonsterConfig(id);
                    UserProfile.Profile.OnKillMonster(monConfig.Star, monConfig.Type, monConfig.Type);
                }
            }
        }
    }
}

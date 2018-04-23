using System;
using System.Collections.Generic;
using NarlonLib.Math;
using NarlonLib.Tools;
using TaleofMonsters.Core.Config;
using TaleofMonsters.Datas.Decks;

namespace TaleofMonsters.Controler.Battle.Data.MemCard
{
    /// <summary>
    /// �ƿ����
    /// </summary>
    internal class CardOffBundle
    {
        private int index;
        private List<ActiveCard> waitList; //�ȴ���
        private List<ActiveCard> graveList; //�س�

        public int LeftCount
        {
            get { return Math.Max(0, waitList.Count - index); }
        }

        private CardOffBundle()
        {
            index = 0;
            waitList = new List<ActiveCard>();
            graveList = new List<ActiveCard>();
        }

        public CardOffBundle(DeckCard[] itsCards)
        {
            waitList = new List<ActiveCard>();
            for (int i = 0; i < itsCards.Length; i++)
                waitList.Add(new ActiveCard(itsCards[i]));
            ArraysUtils.RandomShuffle(waitList);
            graveList = new List<ActiveCard>();
            index = 0;
        }

        public CardOffBundle GetCopy()
        {
            CardOffBundle cloneDeck = new CardOffBundle();
            foreach (var checkCard in waitList)
                cloneDeck.waitList.Add(new ActiveCard(checkCard.CardId, checkCard.Level, 0));
            return cloneDeck;
        }

        public ActiveCard GetNextCard()
        {
            if (waitList.Count == 0)
                return ActiveCard.NoneCard;

            int rt = index;
            if (rt >= waitList.Count)
                return ActiveCard.NoneCard;
            index++;

            if (CardConfigManager.GetCardConfig(waitList[rt].CardId).Id == 0)
            {//�������ÿ����Ѿ����ڣ�����һ����
                NarlonLib.Log.NLog.Warn("GetNextCard card is outofdate id={0}", waitList[rt].CardId);
                return GetNextCard();
            }

            return waitList[rt];
        }

        public ActiveCard ReplaceCard(ActiveCard card)
        {
            if (LeftCount <= 0)
                return ActiveCard.NoneCard;

            var targetIndex = index + MathTool.GetRandom(LeftCount);
            var target = waitList[targetIndex];
            waitList[targetIndex] = card;
            return target;
        }

        public void AddGrave(ActiveCard card)
        {
            graveList.Add(card);
        }
    }
}
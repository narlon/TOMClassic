﻿using System;
using ConfigDatas;
using TaleofMonsters.Config;
using TaleofMonsters.Core;
using TaleofMonsters.DataType.Decks;

namespace TaleofMonsters.DataType.User.Db
{
    public class DbDeckData
    {
        [FieldIndex(Index = 1)]
        public int[] CardIds;
        [FieldIndex(Index = 2)]
        public string Name;
        [FieldIndex(Index = 3)]
        public int Mcount;
        [FieldIndex(Index = 4)]
        public int Wcount;

        public DbDeckData()
        {
            
        }

        public DbDeckData(int index)
        {
            CardIds = new int[GameConstants.DeckCardCount];
            for (int i = 0; i < GameConstants.DeckCardCount; i++)
                CardIds[i] = -1;
            Name = string.Format("卡组{0}", index);
        }

        public int Count
        {
            get { return Wcount+Mcount; }
        }

        public int GetCardAt(int index)
        {
            return CardIds[index];
        }

        public void SetCardAt(int index, int card)
        {
            if (index >= 0 && index < GameConstants.DeckCardCount)
            {
                CardIds[index] = card;
                Recalculate();
            }
        }

        public int AddCard(DeckCard card)
        {
            int firstBlank = -1;
            int count = 0;
            var cardConfig = CardConfigManager.GetCardConfig(card.BaseId);
            int newCardJob = cardConfig.JobId;
            for (int i = 0; i < GameConstants.DeckCardCount; i++)
            {
                var dcard = CardIds[i];

                if (dcard == -1 && firstBlank == -1)
                {
                    firstBlank = i;
                }
                else if (dcard != -1)
                {
                    var tCard = UserProfile.InfoCard.GetDeckCardById(dcard);
                    if (tCard.BaseId == card.BaseId)
                        count++;

                    int cardJob = CardConfigManager.GetCardConfig(tCard.BaseId).JobId;
                    if (newCardJob > 0 && cardJob > 0 && newCardJob != cardJob)
                        return ErrorConfig.Indexer.CardJobTwice;
                }
            }

            if (cardConfig.Quality == CardQualityTypes.Legend && count >= GameConstants.CardLimitLegend)
            {
                return ErrorConfig.Indexer.DeckCardTypeLimitLegend;
            }
            if (cardConfig.Quality != CardQualityTypes.Legend && count >= GameConstants.CardLimit)
            {
                return ErrorConfig.Indexer.DeckCardTypeLimit;
            }
            if (firstBlank==-1)
            {
                return ErrorConfig.Indexer.DeckIsFull;
            }
            SetCardAt(firstBlank, card.BaseId);

            return ErrorConfig.Indexer.OK;
        }

        public void RemoveCardById(int id)
        {
            for (int i = 0; i < GameConstants.DeckCardCount; i++)
            {
                if (CardIds[i] == id)
                {
                    CardIds[i] = -1;
                    break;
                }
            }
            Recalculate();
        }

        public bool HasCard(int id)
        {
            for (int i = 0; i < GameConstants.DeckCardCount; i++)
            {
                if (CardIds[i] == id)
                    return true;
            }
            return false;
        }

        public void Recalculate()
        {
            Mcount = 0;
            Wcount = 0;
            for (int i = 0; i < GameConstants.DeckCardCount; i++)
            {
                if (CardIds[i] != -1)
                {
                    var cardType = ConfigIdManager.GetCardType(CardIds[i]);
                    if (cardType == CardTypes.Monster)
                        Mcount = Mcount + 1;
                    else
                        Wcount = Wcount + 1;
                }
            }
        }
    }
}

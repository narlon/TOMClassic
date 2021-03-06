﻿using System.Collections.Generic;
using ConfigDatas;
using NarlonLib.Log;
using NarlonLib.Math;
using TaleofMonsters.Datas;

namespace TaleofMonsters.Core.Config
{
    internal struct CardConfigData
    {
        public int Id { get; set; }
        public CardTypeSub TypeSub { get; set; }
        public CardTypes Type { get; set; }
        public int Attr { get; set; }
        public int Cost { get; set; }
        public int Star { get; set; }
        public string Name { get; set; }
        public QualityTypes Quality { get; set; }
        public int JobId { get; set; }
        public bool IsSpecial { get; set; }
        public bool IsHeroCard { get; set; } //英雄技能
        public string Remark { get; set; }

        public override string ToString()
        {
            return Id+" " + Name;
        }
    }
    
    internal static class CardConfigManager
    {
        private class CardInfoList
        {
            private List<IntPair> dataList;
            private int[] qualityIndex;

            public CardInfoList()
            {
                dataList = new List<IntPair>();
            }

            public void Add(int cardId, QualityTypes quality)
            {
                IntPair data = new IntPair
                {
                    Type = cardId,
                    Value = (int) quality
                };
                dataList.Add(data);
            }

            public void EndInit()
            {
                dataList.Sort(new CompareByQuality());
                qualityIndex = new[] {-1, -1, -1, -1, -1};
                for (int i = 0; i < dataList.Count; i++)
                {
                    if (dataList[i].Value >= qualityIndex.Length)
                    {
                        NLog.Debug("EndInit invalide quality id={0}", dataList[i].Type);
                        continue;
                    }

                    if (qualityIndex[dataList[i].Value]==-1)
                    {
                        qualityIndex[dataList[i].Value] = i;
                    }
                }
            }

            public int GetRandom(int quality)
            {
                if (quality ==-1)
                    return dataList[MathTool.GetRandom(dataList.Count)].Type;
                if (quality < 4)
                    return dataList[MathTool.GetRandom(qualityIndex[quality], qualityIndex[quality + 1])].Type;
                return dataList[MathTool.GetRandom(qualityIndex[quality], dataList.Count)].Type;
            }

            public int GetRandomByStar(int star)
            {
                if (star <= 0)
                    return dataList[MathTool.GetRandom(dataList.Count)].Type;
                var itemList = new List<int>();
                foreach (var checkItem in dataList)
                {
                    if (CardConfigManager.GetCardConfig(checkItem.Type).Star == star)
                        itemList.Add(checkItem.Type);
                }
                return itemList[MathTool.GetRandom(0, itemList.Count)];
            }

            private class CompareByQuality : IComparer<IntPair>
            {
                #region IComparer<IntPair> 成员

                public int Compare(IntPair x, IntPair y)
                {
                    if (y.Value != x.Value)
                        return x.Value.CompareTo(y.Value);

                    return x.Type.CompareTo(y.Type);
                }

                #endregion
            }
        }

        private static Dictionary<int, CardConfigData> cardConfigDataDict;
        private static CardInfoList allCardData; //可以出职业卡
        private static Dictionary<int, CardInfoList> jobCardDict; //职业卡组列表
        private static Dictionary<int, CardInfoList> attrCardDict; //属性卡组列表，不出职业卡
        private static Dictionary<int, CardInfoList> raceCardDict; //种族卡组列表，不出职业卡
        private static Dictionary<int, CardInfoList> typeCardDict; //生物/武器/法术 1/2/3，不出职业卡

        public static int MonsterTotal { get; set; }
        public static int MonsterAvail { get; set; }
        public static int WeaponTotal { get; set; }
        public static int WeaponAvail { get; set; }
        public static int SpellTotal { get; set; }
        public static int SpellAvail { get; set; }

        static CardConfigManager()
        {
        }

        public static void Init()
        {
            cardConfigDataDict = new Dictionary<int, CardConfigData>();
            foreach (var monsterConfig in ConfigData.MonsterDict.Values)
            {
                CardConfigData card = new CardConfigData
                {
                    Id = monsterConfig.Id,
                    Type = CardTypes.Monster,
                    TypeSub = (CardTypeSub)monsterConfig.Type,
                    Attr = monsterConfig.Attr,
                    Cost = monsterConfig.Cost,
                    Star = monsterConfig.Star,
                    Name = monsterConfig.Name,
                    Quality = (QualityTypes)monsterConfig.Quality,
                    JobId = monsterConfig.JobId,
                    IsSpecial = monsterConfig.IsSpecial == 1,
                    IsHeroCard = monsterConfig.IsHeroCard == 1,
                    Remark = monsterConfig.Remark
                };
                cardConfigDataDict.Add(monsterConfig.Id, card);
                if (monsterConfig.IsSpecial == 0)
                {
                    MonsterTotal++;
                    if (monsterConfig.Remark != "未完成")
                        MonsterAvail++;
                }
            }
            foreach (var weaponConfig in ConfigData.WeaponDict.Values)
            {
                CardConfigData card = new CardConfigData
                {
                    Id = weaponConfig.Id,
                    Type = CardTypes.Weapon,
                    TypeSub = (CardTypeSub)weaponConfig.Type,
                    Attr = weaponConfig.Attr,
                    Cost = weaponConfig.Cost,
                    Star = weaponConfig.Star,
                    Name = weaponConfig.Name,
                    Quality = (QualityTypes)weaponConfig.Quality,
                    JobId = weaponConfig.JobId,
                    IsSpecial = weaponConfig.IsSpecial == 1,
                    IsHeroCard = weaponConfig.IsHeroCard == 1,
                    Remark = weaponConfig.Remark
                };
                cardConfigDataDict.Add(weaponConfig.Id, card);
                if (weaponConfig.IsSpecial == 0)
                {
                    WeaponTotal++;
                    if (weaponConfig.Remark != "未完成")
                        WeaponAvail++;
                }
            }
            foreach (var spellConfig in ConfigData.SpellDict.Values)
            {
                CardConfigData card = new CardConfigData
                {
                    Id = spellConfig.Id,
                    Type = CardTypes.Spell,
                    TypeSub = (CardTypeSub)spellConfig.Type,
                    Attr = spellConfig.Attr,
                    Cost = spellConfig.Cost,
                    Star = spellConfig.Star,
                    Name = spellConfig.Name,
                    Quality = (QualityTypes)spellConfig.Quality,
                    JobId = spellConfig.JobId,
                    IsSpecial = spellConfig.IsSpecial == 1,
                    IsHeroCard = spellConfig.IsHeroCard == 1,
                    Remark = spellConfig.Remark
                };
                cardConfigDataDict.Add(spellConfig.Id, card);
                if (spellConfig.IsSpecial == 0)
                {
                    SpellTotal++;
                    if (!spellConfig.Remark.Contains("未完成"))
                        SpellAvail++;
                }
            }

            InitJobCard();
            InitAttrCard();
            InitRaceCard();
            InitTypeCard();
            InitAllCard();
        }

        #region 各种缓存初始化
        private static void InitAllCard()
        {
            allCardData = new CardInfoList();
            foreach (var cardConfigData in cardConfigDataDict.Values)
            {
                if (!cardConfigData.IsSpecial)
                    allCardData.Add(cardConfigData.Id, cardConfigData.Quality);
            }
            allCardData.EndInit();
        }

        private static void InitAttrCard()
        {
            attrCardDict = new Dictionary<int, CardInfoList>();
            for (int i = 0; i < 10; i++)
                attrCardDict.Add(i, new CardInfoList());

            foreach (var cardConfigData in cardConfigDataDict.Values)
            {
                if (!cardConfigData.IsSpecial && cardConfigData.JobId == 0)
                    attrCardDict[cardConfigData.Attr].Add(cardConfigData.Id, cardConfigData.Quality);
            }

            for (int i = 0; i < 10; i++)
                attrCardDict[i].EndInit();
        }

        private static void InitJobCard()
        {
            jobCardDict = new Dictionary<int, CardInfoList>();
            jobCardDict[0] = new CardInfoList();
            foreach (var jobId in ConfigData.JobDict.Keys)
                jobCardDict[jobId] = new CardInfoList();

            foreach (var cardConfigData in cardConfigDataDict.Values)
            {
                if (!cardConfigData.IsSpecial)
                    jobCardDict[cardConfigData.JobId].Add(cardConfigData.Id, cardConfigData.Quality);
            }

            foreach (var jobId in jobCardDict.Keys)
                jobCardDict[jobId].EndInit();
        }

        private static void InitRaceCard()
        {
            raceCardDict = new Dictionary<int, CardInfoList>();
            for (int i = 0; i <= 16; i++)
                raceCardDict[i] = new CardInfoList();
            for (int i = 100; i <= 104; i++) //weapon
                raceCardDict[i] = new CardInfoList();
            for (int i = 200; i <= 203; i++)//spell
                raceCardDict[i] = new CardInfoList();

            foreach (var cardConfigData in cardConfigDataDict.Values)
            {
                if (!cardConfigData.IsSpecial && cardConfigData.JobId == 0)
                    raceCardDict[(int)cardConfigData.TypeSub].Add(cardConfigData.Id, cardConfigData.Quality);
            }

            for (int i = 0; i < 17; i++)
                raceCardDict[i].EndInit();
        }

        private static void InitTypeCard()
        {
            typeCardDict = new Dictionary<int, CardInfoList>();
            for (int i = 0; i < 4; i++)
                typeCardDict[i] = new CardInfoList();

            foreach (var cardConfigData in cardConfigDataDict.Values)
            {
                var cardType = (int)ConfigIdManager.GetCardType(cardConfigData.Id);
                if (!cardConfigData.IsSpecial && cardConfigData.JobId == 0)
                    typeCardDict[cardType].Add(cardConfigData.Id, cardConfigData.Quality);
            }

            for (int i = 0; i < 4; i++)
                typeCardDict[i].EndInit();
        }
        #endregion

        public static CardConfigData GetCardConfig(int id)
        {
            CardConfigData outData;
            if (cardConfigDataDict.TryGetValue(id, out outData))
                return outData;
            return new CardConfigData();
        }

        #region 获取随机卡牌的算法
        internal delegate int RandomCardSelectorDelegate(int raceId, int quality);

        internal static int GetRandomCard(int seed, int quality=-1)
        {
            return allCardData.GetRandom(quality);
        }

        internal static int GetRandomJobCard(int jobId, int quality=-1)
        {
            CardInfoList rtData;//job=0表示返回中立卡
            if (jobCardDict.TryGetValue(jobId, out rtData))
            {
                if(rtData == null)//special job
                    return 0;
                return rtData.GetRandom(quality);
            }
            return 0;
        }

        internal static int GetRandomAttrCard(int attrId, int quality = -1)
        {
            CardInfoList rtData;
            if (attrCardDict.TryGetValue(attrId, out rtData))
                return rtData.GetRandom(quality);
            return 0;
        }

        internal static int GetRandomAttrStarCard(int attrId, int star = -1)
        {
            CardInfoList rtData;
            if (attrCardDict.TryGetValue(attrId, out rtData))
                return rtData.GetRandomByStar(star);
            return 0;
        }

        internal static int GetRandomRaceCard(int raceId, int quality = -1)
        {
            CardInfoList rtData;
            if (raceCardDict.TryGetValue(raceId, out rtData))
                return rtData.GetRandom(quality);
            return 0;
        }

        internal static int GetRandomRaceStarCard(int raceId, int star = -1)
        {
            CardInfoList rtData;
            if (raceCardDict.TryGetValue(raceId, out rtData))
                return rtData.GetRandomByStar(star);
            return 0;
        }

        internal static int GetRandomTypeCard(int typeId, int quality = -1)
        {
            CardInfoList rtData;
            if (typeCardDict.TryGetValue(typeId, out rtData))
                return rtData.GetRandom(quality);
            return 0;
        }
        internal static int GetRandomTypeStarCard(int typeId, int star = -1)
        {
            CardInfoList rtData;
            if (typeCardDict.TryGetValue(typeId, out rtData))
                return rtData.GetRandomByStar(star);
            return 0;
        }

        #endregion

        private static int GetRateCard(int[] rate, RandomCardSelectorDelegate del, int funcInfo)
        {
            while (true)
            {
                int sum = 0;
                foreach (var r in rate)
                    sum += r;
                int roll = MathTool.GetRandom(sum);
                int quality = 0;
                sum = 0;
                for (int i = 0; i < rate.Length; i++)
                {
                    sum += rate[i];
                    if (roll < sum)
                    {
                        quality = i;
                        break;
                    }
                }

                var cardId = del(funcInfo, quality);
                if (cardId > 0)
                {
                    return cardId;
                }
                else
                {
                    NLog.Debug("GetRateCard cardId = 0 funcInfo " + funcInfo);
                    break;
                }
            }

            return 0;
        }

        public static int GetRateCardStr(string ruleStr, int[] dropRate)
        {
            var ruleDats = ruleStr.Split('.');
            var type = ruleDats[0];
            var ruleInfo = int.Parse(ruleDats[1]);
            if (dropRate == null)
            {
                if (ruleDats.Length >= 3)
                {
                    dropRate = new[] { 0, 0, 0, 0, 0 };
                    dropRate[int.Parse(ruleDats[2])] = 1000;
                }
                else
                {
                    dropRate = new[] {420, 310, 240, 20, 10};//默认卡包的概率
                }
            }

            if (type == "attr")
                return GetRateCard(dropRate, GetRandomAttrCard, ruleInfo);
            if (type == "type")
                return GetRateCard(dropRate, GetRandomTypeCard, ruleInfo);
            if (type == "race")
                return GetRateCard(dropRate, GetRandomRaceCard, ruleInfo);
            if (type == "job")
                return GetRateCard(dropRate, GetRandomJobCard, ruleInfo);
            return GetRateCard(dropRate, GetRandomCard, ruleInfo);
        }
    }
}

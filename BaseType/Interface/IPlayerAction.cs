﻿namespace ConfigDatas
{
    public interface IPlayerAction
    {
        void AddSpike(int id);
        void RemoveSpike(int id);
        void DeleteRandomCardFor(IPlayer p, int levelChange);
        void CopyRandomCardFor(IPlayer p, int levelChange);
        void GetNextNCard(IMonster mon, int n);
        void ConvertCard(int count, int cardId, int levelChange);
        void AddCard(IMonster mon, int cardId, int level);
        void AddCard(IMonster mon, int cardId, int level, int modify);
        void DeleteAllCard();
        void DeleteSelectCard();
        void RecostSelectCard();

        void CopyRandomNCard(int n, int spellid);
        void CardLevelUp(int n, int type);
        void AddRandomCard(IMonster mon, int type, int lv); //按类型给一张随机卡牌，怪物，武器
        void AddRandomCardJob(IMonster mon, int jobId, int lv); //按职业给一张随机卡牌
        void AddRandomCardRace(IMonster mon, int race, int lv); //按种族给一张随机卡牌

        void DiscoverCardType(IMonster mon, int type, int lv, string dtype);
        void DiscoverCardRace(IMonster mon, int race, int lv, string dtype);

        void AddMonster(int cardId, int level, System.Drawing.Point location);
        void ExchangeMonster(IMonster target, int lv);
        void AddResource(int type, int number);
        void AddTrap(int id, int spellId, int lv, double rate, int dam, double help);
        void RemoveRandomTrap();
        void AddSpellMissile(IMonster target, ISpell spell, System.Drawing.Point mouse, string effect);
        void AddSpellRowMissile(ISpell spell, int count, System.Drawing.Point mouse, string effect);

        void AddSpellEffect(double rate);
        void AddSpellVibrate(double rate);
    }
}
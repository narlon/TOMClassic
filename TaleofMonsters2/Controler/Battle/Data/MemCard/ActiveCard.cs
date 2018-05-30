using System;
using NarlonLib.Math;
using TaleofMonsters.Core;
using TaleofMonsters.Core.Config;
using TaleofMonsters.Datas;
using TaleofMonsters.Datas.Decks;

namespace TaleofMonsters.Controler.Battle.Data.MemCard
{
    internal class ActiveCard : IMemCardData
    {
        internal static ActiveCard NoneCard = new ActiveCard();

        public int CardId { get; set; }

        public int Mp { get; set; }
        public int Lp { get; set; }
        public int Pp { get; set; }

        public int CostModify { get; set; } //�������ĵ��������ܻᱻ�����޸�

        public byte Level { get; private set; }//���Ƶȼ������ܻᱻ�����޸�
        public ushort Exp { get { return 0; } }
        
        public ActiveCard()
        {
           
        }

        public ActiveCard(int baseid, byte level)
        {
            CardId = baseid;
            Level = level;

            SetCost();
        }

        public CardTypes CardType
        {
            get { return ConfigIdManager.GetCardType(CardId); }
        }

        public void ChangeLevel(int levelChange)
        {
            var level = (byte)MathTool.Clamp(Level + levelChange, 1, GameConstants.CardMaxLevel);
            Level = level;
        }

        private void SetCost()
        {
            var cardConfig = CardConfigManager.GetCardConfig(CardId);
            Mp = cardConfig.Type != CardTypes.Spell ? 0 : cardConfig.Cost;
            Lp = cardConfig.Type != CardTypes.Monster ? 0 : cardConfig.Cost;
            Pp = cardConfig.Type != CardTypes.Weapon ? 0 : cardConfig.Cost;
        }

        public ActiveCard GetCopy()
        {
            return new ActiveCard(CardId, Level);
        }
        
        public static bool operator ==(ActiveCard rec1, ActiveCard rec2)
        {
            return Equals(rec1, rec2);
        }

        public static bool operator !=(ActiveCard rec1, ActiveCard rec2)
        {
            return !Equals(rec1, rec2);
        }

        public override int GetHashCode()
        {
            return CardId + Level;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (GetType() != obj.GetType())
                return false;
            ActiveCard rec = (ActiveCard) obj;
            if (rec.CardId != CardId) 
                return false;
            if (rec.Level != Level) //todo �������������ԣ���ʱֻ�����
                return false;
            return true;
        }

        public override string ToString()
        {
            return string.Format("id={0}", CardId);
        }
    }   
}

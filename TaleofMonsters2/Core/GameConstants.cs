namespace TaleofMonsters.Core
{
    static class GameConstants
    {
        public const int CardMaxLevel = 99;
        public const int CardSlotMaxCount = 10;
        public const int DeckCardCount = 30;//一套牌有几张
        public const int CardLimit = 1;//同种卡牌拥有的上限

        public const int CardShopDura = 24*3600;
        public const int MergeWeaponDura = 6*3600;
        public const int ChangeCardDura = 24*3600;
        public const int NpcPieceDura = 6 * 3600;
        public const int QuestionCooldownDura = 15;

        public const float DrawManaTime = 1f;
        public const float DrawManaTimeFast = 0.5f;

        public const int PlayDeckCount = 9;
        public const int PlayFarmCount = 9;

        public const int NewDayAP = 100;

        public const int FightAPCost = 2;
        public const int MazeAPCost = 20;

        public const int BattleInitialCardCount = 3; //战斗开始时的卡牌数
        public const int RoundTime = 8000;//一个回合多少ms，一个回合给一张卡
        public const int BattleAttackRoundWait = 4; //影响战斗节奏以及一盘的时间
        public const int BattleActionLimit = 200;//200即可攻击
        public const float RoundRecoverAddon = 1.5f; //回合的回复倍率
        public const int RoundRecoverDoubleRound = 10; //第几个回合开始恢复加倍
        public const int RoundRecoverAllRound = 4; //每多少回合回复一次所有能量点
        public const int RoundAts = 30;//这个高了，怪的hp也要调高，spell的伤害和武器的耐久调整
        public const int LimitAts = 600; //超过这个值就会进行攻击，LimitAts/RoundAts/5=攻击间隔时间

    }
}

namespace TaleofMonsters.Core
{
    static class GameConstants
    {
        public const int ProfileNameLengthMin = 3;
        public const int ProfileNameLengthMax = 12;
        public const int RoleNameLengthMin = 2;
        public const int RoleNameLengthMax = 6;

        public const int CardMaxLevel = 19;
        public const int CardSlotMaxCount = 10;
        public const int DeckCardCount = 30;//һ�����м���
        public const int CardLimit = 1;//ͬ�ֿ���ӵ�е�����
        public const int DiamondToGold = 10; //��ʯ�ƽ��ֵ

        public const int CardShopDura = 24*3600;
        public const int MergeWeaponDura = 3600; //����ϵͳ����ʱ��
        public const int BlessShopDura = 3600; //blessshop����ʱ��
        public const int BlessLimit = 10; //���ף������
        public const int QuestionCooldownDura = 15;
        public const int EquipOffCount = 60;
        public const int EquipOnCount = 9;
        public const int ItemCdGroupCount = 10;
        public const int BagInitCount = 50; //��ʼ��������

        public const float DrawManaTime = 1f;
        public const float DrawManaTimeFast = 0.5f;

        public const int PlayDeckCount = 9;
        public const int PlayFarmCount = 9;

        public const int SceneMoveCost = 2;

        public const int BattleInitialCardCount = 3; //ս����ʼʱ�Ŀ�����
        public const int DiscoverCardCount = 3; //����ʱ�Ŀ�������
        public const int RoundTime = 8000;//һ���غ϶���ms��һ���غϸ�һ�ſ�
        public const float RoundRecoverAddon = 1.5f; //�غϵĻظ�����
        public const int RoundRecoverDoubleRound = 10; //�ڼ����غϿ�ʼ�ָ��ӱ�
        public const int RoundRecoverAllRound = 9999; //ÿ���ٻغϻظ�һ�����������㣬todo ��ʱȥ���������
        public const int RoundAts = 30;//������ˣ��ֵ�hpҲҪ���ߣ�spell���˺����������;õ���
        public const int LimitAts = 600; //�������ֵ�ͻ���й�����LimitAts/RoundAts/5=�������ʱ��
        public const int PrepareAts = 600; //׼��ʱ���ĵ�ʱ��
        public const int MaxTrapCount = 3; //���������

        public const float CardStrengthStar = 0.3f;
        public const float CardStrengthLevel = 0.12f;
        public const int MaxMeleeAtkRange = 15; //��ս��������
        public const float SideKickFactor = 0.6f; //֧Ԯ��ϵ��
        public const int DefaultHitRate = 85;//Ĭ�ϵ�����
        public const int CrtToRate = 6;//ÿ��crtʵ�ʵı�����
        public const float DefaultCrtDamage = 1.5f;//Ĭ�ϱ���ʱ���˺�����
        public const double MagToRate = 0.1;//ÿ��magʵ�ʵ��˺���
        public const double SpdToRate = 3;//ÿ�㹥��ʵ�ʵ�������
        public const int HitToRate = 5;//ÿ������ʵ�ʵ�������
        public const int LukToRoll = 100; //ÿ��������ɵĶ���roll�㣬���֮
        public const int MaxDropItemGetOnBattle = 10; //ÿ�ο��Ի��ս��Ʒ��������

        public const float SceneTileGradient = 0.33f;
        public const int SceneTileStandardWidth = 100;
        public const int SceneTileStandardHeight = 65;
        public const double SceneQuestHiddenIconRate = 0.2;

        public const int DungeonCardLimit = 40;
    }
}

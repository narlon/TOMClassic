﻿using ConfigDatas;
using TaleofMonsters.Controler.Battle.Data.Players;
using TaleofMonsters.Controler.Battle.Data.Players.AIs;
using TaleofMonsters.Datas;
using TaleofMonsters.Datas.Decks;

namespace TaleofMonsters.Controler.Battle.DataTent
{
    internal class PlayerManager
    {
        public Player LeftPlayer { get; set; }
        public Player RightPlayer { get; set; }

        public void Init(int left, DeckCard[] leftCards, int right, int rlevel)
        {
            PeopleConfig peopleConfig = ConfigData.GetPeopleConfig(right);
            if (left == 0)
            {
                switch (peopleConfig.Method)
                {
                    case "common": LeftPlayer = new HumanPlayer(true, leftCards); break;
                    case "actrand": LeftPlayer = new RandomPlayer(right,true); break;
                    default: LeftPlayer = new AIPlayer(right, peopleConfig.Method, true, rlevel); break;
                }
            }
            else //观看比赛
            {
                LeftPlayer = new AIPlayer(left, ConfigData.GetPeopleConfig(left).Emethod, true, rlevel);
                LeftPlayer.AIContext = new AIStrategyContext(LeftPlayer);
            }

            switch (peopleConfig.Emethod)
            {
                case "common": RightPlayer = new HumanPlayer(false, leftCards); RightPlayer.PeopleId = right; break;
                case "actrand": RightPlayer = new RandomPlayer(right, false); break;
                case "actmirror": RightPlayer = new MirrorPlayer(right, LeftPlayer.OffCards, false); break;
                default: RightPlayer = new AIPlayer(right, peopleConfig.Emethod, false, rlevel); break;
            }
            RightPlayer.AIContext = new AIStrategyContext(RightPlayer);
        }

        public void Clear()
        {
            LeftPlayer = null;
            RightPlayer = null;
        }

        public void Update(bool isFast, float pastRound, int round)
        {
            LeftPlayer.Update(isFast, pastRound, round);
            RightPlayer.Update(isFast, pastRound, round);
        }

        public void CheckRoundCard()
        {
            LeftPlayer.DrawNextNCard(null, 1, AddCardReasons.RoundCard);
            if (LeftPlayer.SpikeManager.HasSpike("doublecard"))
            {
                LeftPlayer.DrawNextNCard(null, 1, AddCardReasons.RoundCard);
            }
            RightPlayer.DrawNextNCard(null, 1, AddCardReasons.RoundCard);
            if (RightPlayer.SpikeManager.HasSpike("doublecard"))
            {
                RightPlayer.DrawNextNCard(null, 1, AddCardReasons.RoundCard);
            }
        }
    }
}

﻿using System.Drawing;
using TaleofMonsters.DataType.Others;
using TaleofMonsters.Forms;
using TaleofMonsters.MainItem.Quests.SceneQuests;

namespace TaleofMonsters.MainItem.Quests
{
    internal class TalkEventItemNpc : TalkEventItem
    {
        private bool autoClose;
        public TalkEventItemNpc(int evtId, int level, Rectangle r, SceneQuestEvent e)
            : base(evtId, level, r, e)
        {
            autoClose = true;
            if (e.ParamList[0] == "buypiece")
            {
                PanelManager.DealPanel(new BuyPieceForm());
            }
            else if (e.ParamList[0] == "changecard")
            {
                PanelManager.DealPanel(new ChangeCardForm());
            }
            else if (e.ParamList[0] == "changeres")
            {
                PanelManager.DealPanel(new ChangeResForm());
            }
            else if (e.ParamList[0] == "selectjob")
            {
                autoClose = false;
                PanelManager.DealPanel(new SelectJobForm());
            }
            else if (e.ParamList[0] == "merge")
            {
                PanelManager.DealPanel(new MergeWeaponForm());
            }
            else if (e.ParamList[0] == "farm")
            {
                PanelManager.DealPanel(new FarmForm());
            }
            else if (e.ParamList[0] == "bless")
            {
                PanelManager.DealPanel(new BlessForm());
            }
            else if (e.ParamList[0] == "shop")
            {
                var shop = new NpcShopForm();
                shop.ShopName = config.ShopName;
                PanelManager.DealPanel(shop);
            }
            else if (e.ParamList[0] == "wheel")
            {
                TreasureWheelBook.Show(int.Parse(e.ParamList[1]));
            }
        }

        public override void OnFrame(int tick)
        {
            RunningState = TalkEventState.Finish;
        }
        public override bool AutoClose()
        {
            return autoClose;
        }
    }
}


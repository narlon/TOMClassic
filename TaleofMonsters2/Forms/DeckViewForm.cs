﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ConfigDatas;
using ControlPlus;
using NarlonLib.Log;
using TaleofMonsters.Core;
using TaleofMonsters.Core.Config;
using TaleofMonsters.Core.Loader;
using TaleofMonsters.Datas;
using TaleofMonsters.Datas.Decks;
using TaleofMonsters.Datas.Effects;
using TaleofMonsters.Datas.Effects.Facts;
using TaleofMonsters.Datas.Others;
using TaleofMonsters.Datas.User;
using TaleofMonsters.Datas.User.Db;
using TaleofMonsters.Forms.CMain;
using TaleofMonsters.Forms.Items;
using TaleofMonsters.Forms.Items.Core;
using TaleofMonsters.Forms.Items.Regions;
using TaleofMonsters.Forms.Items.Regions.Decorators;
using TaleofMonsters.Forms.MagicBook;

namespace TaleofMonsters.Forms
{
    internal sealed partial class DeckViewForm : BasePanel
    {
        private class CompareDeckCardByType : IComparer<DeckCard>
        {
            #region IComparer<CardDescript> 成员

            public int Compare(DeckCard cx, DeckCard cy)
            {
                if (cx.CardId == cy.CardId && cy.CardId == 0)
                {
                    return 0;
                }
                if (cy.CardId == 0)
                {
                    return -1;
                }
                if (cx.CardId == 0)
                {
                    return 1;
                }
                int typex = CardConfigManager.GetCardConfig(cx.CardId).Attr;
                int typey = CardConfigManager.GetCardConfig(cy.CardId).Attr;
                if (typex != typey)
                {
                    return typex.CompareTo(typey);
                }

                return cx.CardId.CompareTo(cy.CardId);
            }

            #endregion
        }

        private VirtualRegion vRegion;
        private DeckCardRegion cardRegion;//卡盒区域
        private DeckSelectCardRegion selectRegion;//卡组区域
        private DeckCard targetCard;

        private const int yoff = 35;
        private bool show;
        private int floor;

        private bool isShowStatistic = false; //默认显示carddetail

        private CardDetail cardDetail;
        private CardDeckStatistic deckStatistic; 

        private PopMenuDeck popMenuDeck;
        private PoperContainer popContainer;

        public DeckViewForm()
        {
            InitializeComponent();
            vRegion = new VirtualRegion(this);
            SubVirtualRegion region;
            string[] txt = { "全部卡片", " 新卡片" };
            for (int i = 0; i < 2; i++)
            {
                region = new ButtonRegion(i + 1, 12+150+85*i, 40, 74, 24, "CommonButton1.JPG", "");
                region.AddDecorator(new RegionTextDecorator(8, 7, 10, Color.Black, txt[i]));
                vRegion.AddRegion(region);
            }

            vRegion.RegionClicked += OnVRegionClicked;
            vRegion.RegionEntered += OnVRegionEntered;
            vRegion.RegionLeft += OnVRegionLeft;

            this.bitmapButtonClose.ImageNormal = PicLoader.Read("Button.Panel", "CloseButton1.JPG");
            bitmapButtonClose.NoUseDrawNine = true;
            this.bitmapButtonNext.ImageNormal = PicLoader.Read("Button.Panel", "NextButton.JPG");
            bitmapButtonNext.NoUseDrawNine = true;
            this.bitmapButtonSwitch.ImageNormal = PicLoader.Read("Button.Panel", "ButtonBack2.PNG");
            bitmapButtonSwitch.Font = new Font("宋体", 8 * 1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
            bitmapButtonSwitch.ForeColor = Color.White;
            bitmapButtonSwitch.IconImage = PicLoader.Read("Icon", "rot4.PNG");
            bitmapButtonSwitch.IconSize = new Size(16, 16);
            bitmapButtonSwitch.IconXY = new Point(4, 4);
            bitmapButtonSwitch.TextOffX = 8;
            bitmapButtonSwitch.Text = "切换";
           // bitmapButtonNext.NoUseDrawNine = true;
            this.bitmapButtonPre.ImageNormal = PicLoader.Read("Button.Panel", "PreButton.JPG");
            bitmapButtonPre.NoUseDrawNine = true;
            this.bitmapButtonNextD.ImageNormal = PicLoader.Read("Button.Panel", "NextButton.JPG");
            bitmapButtonNextD.NoUseDrawNine = true;
            this.bitmapButtonPreD.ImageNormal = PicLoader.Read("Button.Panel", "PreButton.JPG");
            bitmapButtonPreD.NoUseDrawNine = true;
            this.bitmapButtonDel.ImageNormal = PicLoader.Read("Button.Panel", "DelButton.JPG");
            bitmapButtonDel.NoUseDrawNine = true;
          
            cardDetail = new CardDetail(this, 635, 35, 565);
            cardDetail.Enabled = true;
            cardDetail.Invalidate += DetailInvalidate;
            deckStatistic = new CardDeckStatistic(635, 35, 565);
            cardRegion = new DeckCardRegion(5+150, 35 + yoff, 480, 510);
            cardRegion.Invalidate += DeckInvalidate;
            selectRegion = new DeckSelectCardRegion(5, 35, 150, 540);
            selectRegion.Invalidate += SelectDeckInvalidate;

            popMenuDeck = new PopMenuDeck();
            popContainer = new PoperContainer(popMenuDeck);
            popMenuDeck.PoperContainer = popContainer;
            popMenuDeck.Form = this;
        }

        private void DeckInvalidate()
        {
            if (cardRegion != null)
                Invalidate(new Rectangle(cardRegion.X, cardRegion.Y, cardRegion.Width, cardRegion.Height));
        }

        private void SelectDeckInvalidate()
        {
            if (selectRegion != null)
                Invalidate(new Rectangle(selectRegion.X, selectRegion.Y, selectRegion.Width, selectRegion.Height));
        }

        private void DetailInvalidate()
        {
            if (cardDetail != null)
                Invalidate(new Rectangle(cardDetail.X, cardDetail.Y, cardDetail.Width, cardDetail.Height));
        }

        public override void Init(int width, int height)
        {
            base.Init(width, height);
            show = true;

            ChangeDeck(1);
            UpdateButtonState();
            UpdateDeckButtonState();

            SetBgm("TOM003.mp3");
        }

        public override void OnFrame(int tick, float timePass)
        {
            base.OnFrame(tick, timePass);

            cardDetail.OnFrame();
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void InstallDeckCard()
        {
            DeckCard[] dcards = null;
            DbDeckData dk = UserProfile.InfoCard.SelectedDeck;
            dcards = new DeckCard[GameConstants.DeckCardCount];
            for (int i = 0; i < GameConstants.DeckCardCount; i++)
            {
                int cid = dk.GetCardAt(i);
                dcards[i] = new DeckCard(UserProfile.InfoCard.GetDeckCardById(cid));
            }

            selectRegion.ChangeDeck(dcards);

            deckStatistic.Update(dcards);
        }

        private void ChangeDeck(int type)
        {
            InstallDeckCard();

            floor = type;
            DeckCard[] dcards = null;
            if (floor == 1)
            {
                dcards = new DeckCard[UserProfile.InfoCard.Cards.Count];
                int i = 0;
                foreach (var card in UserProfile.InfoCard.Cards.Values)
                    dcards[i++] = new DeckCard(card);
                Array.Sort(dcards, new CompareDeckCardByType());
            }
            else if (floor == 2)
            {
                dcards = new DeckCard[UserProfile.InfoCard.Newcards.Count];
                for (int i = 0; i < dcards.Length; i++)
                    dcards[i] = new DeckCard(UserProfile.InfoCard.GetDeckCardById(UserProfile.InfoCard.Newcards[i]));
            }

            cardRegion.ChangeDeck(dcards);
            SetTargetCard(dcards[0]);

            UpdateButtonState();
            UpdateDeckButtonState();
            for (int i = 0; i < 3; i++)
                vRegion.SetRegionEffect(i + 1, RegionEffect.Free);
            vRegion.SetRegionEffect(type, RegionEffect.Blacken);
            Invalidate();
        }

        private void SetTargetCard(DeckCard card)
        {
            targetCard = card;
            cardDetail.SetInfo(targetCard);
        }

        private void DeckViewForm_MouseMove(object sender, MouseEventArgs e)
        {
            cardRegion.CheckMouseMove(e.X, e.Y);
            selectRegion.CheckMouseMove(e.X, e.Y);
        }

        private void DeckViewForm_MouseClick(object sender, MouseEventArgs e)
        {
            bool cardFromRegion = true;
            var tCard = cardRegion.GetTargetCard();
            if (tCard == null)
            {
                tCard = selectRegion.GetTargetCard();
                if (tCard == null)
                    return;
                cardFromRegion = false;
            }

            SetTargetCard(tCard);
            if (e.Button == MouseButtons.Right)
            {
                popMenuDeck.Clear();

                if (cardFromRegion)
                {
                    var levelExpConfig = ConfigData.GetLevelExpConfig(targetCard.Level);
                    var cardConfig = CardConfigManager.GetCardConfig(targetCard.CardId);
                    var itemPrice = GameResourceBook.OutGemCardBuy((int)cardConfig.Quality)*5;//溢出价格
                    popMenuDeck.AddItem("activate", "添加到卡组");
                    popMenuDeck.AddItem("levelup", string.Format("升级({0})", levelExpConfig.CardExp), targetCard.Exp >= levelExpConfig.CardExp ? "white" : "gray", "oth10");
                    if (cardConfig.Type == CardTypes.Monster)
                        popMenuDeck.AddItem("levelup2", string.Format("升级({0})", (uint)levelExpConfig.CardExp * itemPrice), UserProfile.InfoBag.HasResource(GameResourceType.Carbuncle, (uint)levelExpConfig.CardExp* itemPrice) ? "white" : "gray", "res5");
                    else if (cardConfig.Type == CardTypes.Weapon)
                        popMenuDeck.AddItem("levelup2", string.Format("升级({0})", (uint)levelExpConfig.CardExp * itemPrice), UserProfile.InfoBag.HasResource(GameResourceType.Gem, (uint)levelExpConfig.CardExp * itemPrice) ? "white" : "gray", "res7");
                    else if (cardConfig.Type == CardTypes.Spell)
                        popMenuDeck.AddItem("levelup2", string.Format("升级({0})", (uint)levelExpConfig.CardExp * itemPrice), UserProfile.InfoBag.HasResource(GameResourceType.Mercury, (uint)levelExpConfig.CardExp * itemPrice) ? "white" : "gray", "res4");
                }
                else
                {
                    popMenuDeck.AddItem("remove", "从卡组移除");
                }

                popMenuDeck.AddItem("cancel", "取消");
                popMenuDeck.TargetCard = targetCard;
                popMenuDeck.AutoResize();
                popContainer.Show(this, e.Location.X, e.Location.Y);                
            }
        }

        public void MenuRefresh()
        {
            cardRegion.MenuRefresh();
            Invalidate();
        }

        public void ActivateCard()
        {
            InstallDeckCard();
            cardRegion.Invalidate();
            selectRegion.Invalidate();
        }

        public void OnCardLevelUp(int cardId, byte lv, ushort exp)
        {
            var card = cardRegion.GetCard(cardId);
            if (card != null)
            {
                card.Level = lv;
                card.Exp = exp;
            }

            card = selectRegion.GetCard(cardId);
            if (card != null)
            {
                card.Level = lv;
                card.Exp = exp;
            }

            card = cardRegion.GetTargetCard();
            if (card != null)
                SetTargetCard(card);

            var effect = new StaticUIEffect(EffectBook.GetEffect("yellowflash"), new Point(Width/2-50, Height/2-50), new Size(100, 100));
            effect.Repeat = false;
            AddEffect(effect);

            AddFlowCenter("升级卡牌成功", "Gold");
        }

        private void UpdateButtonState()
        {
            bitmapButtonPre.Enabled = cardRegion.Page > 0;
            bitmapButtonNext.Enabled = (cardRegion.Page + 1) * cardRegion.CardCount < cardRegion.CardTotalCount;
            bitmapButtonPre.Visible = bitmapButtonPre.Enabled || bitmapButtonNext.Enabled;
            bitmapButtonNext.Visible = bitmapButtonPre.Enabled || bitmapButtonNext.Enabled;
        }

        private void buttonPre_Click(object sender, EventArgs e)
        {
            if (cardRegion.PrePage())
            {
                var card = cardRegion.GetTargetCard();
                SetTargetCard(card);
                UpdateButtonState();
            }
        }

        private void buttonNext_Click(object sender, EventArgs e)
        {
            if (cardRegion.NextPage())
            {
                var card = cardRegion.GetTargetCard();
                SetTargetCard(card);
                UpdateButtonState();
            }
        }
        private void bitmapButtonSwitch_Click(object sender, EventArgs e)
        {
            isShowStatistic = !isShowStatistic;
            cardDetail.Enabled = !isShowStatistic;
            Invalidate(new Rectangle(cardDetail.X, cardDetail.Y, cardDetail.Width, cardDetail.Height));
        }

        private void UpdateDeckButtonState()
        {
            bitmapButtonPreD.Enabled = UserProfile.InfoCard.DeckId > 0;
            bitmapButtonNextD.Enabled = UserProfile.InfoCard.DeckId + 1 < GameConstants.PlayDeckCount;
        }

        private void buttonNextD_Click(object sender, EventArgs e)
        {
            int tmp = UserProfile.InfoCard.DeckId + 1;
            if (tmp >= GameConstants.PlayDeckCount)
                return;
            UserProfile.InfoCard.DeckId = tmp;
            InstallDeckCard();
            cardRegion.MenuRefresh();
            Invalidate();
            UpdateDeckButtonState();
        }

        private void buttonPreD_Click(object sender, EventArgs e)
        {
            int tmp = UserProfile.InfoCard.DeckId - 1;
            if (tmp < 0)
                return;
            UserProfile.InfoCard.DeckId = tmp;
            InstallDeckCard();
            cardRegion.MenuRefresh();
            Invalidate();
            UpdateDeckButtonState();
        }

        private void buttonDelD_Click(object sender, EventArgs e)
        {
            if (MessageBoxEx2.Show("确定要清除卡组内所有卡片？") == DialogResult.OK)
            {
                for (int i = 0; i < GameConstants.DeckCardCount; i++)
                    UserProfile.InfoCard.SelectedDeck.SetCardAt(i, -1);
            }
            InstallDeckCard();
            cardRegion.MenuRefresh();
            Invalidate();
        }

        private void OnVRegionClicked(int id, int x, int y, MouseButtons button)
        {
            if (button == MouseButtons.Left)
            {
                if (id > 0 && id < 10)
                {
                    ChangeDeck(id);
                }
                //else if (id == 11)//说明是升级
                //{
                //    int itemId = 32101;//todo 规则需要修改
                //    HItemConfig itemConfig = ConfigData.GetHItemConfig(itemId);
                //    if (UserProfile.Profile.InfoBag.GetItemCount(itemId) > 0)//给卡牌增加经验
                //    {
                //        UserProfile.Profile.InfoBag.DeleteItem(itemId, 1);
                //     //   UserProfile.InfoCard.AddCardExp(targetCard.Id, itemConfig.UseEffect);
                //    }
                //    cardRegion.MenuRefresh();
                //    Invalidate();
                //}
            }
        }

        private void OnVRegionEntered(int id, int x, int y, int key)
        {

        }

        private void OnVRegionLeft()
        {

        }

        private void DeckViewForm_Paint(object sender, PaintEventArgs e)
        {
            BorderPainter.Draw(e.Graphics, "", Width, Height);

            Font font = new Font("黑体", 12*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            e.Graphics.DrawString("我的卡片", font, Brushes.White, Width / 2 - 40, 8);
            font.Dispose();

            try
            {
                if (show)
                {
                    if (!isShowStatistic)
                        cardDetail.Draw(e.Graphics);
                    else
                        deckStatistic.Draw(e.Graphics);
                    cardRegion.Draw(e.Graphics);
                    selectRegion.Draw(e.Graphics);

                    vRegion.Draw(e.Graphics);

                    font = new Font("黑体", 12*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
                    e.Graphics.DrawString(UserProfile.InfoCard.SelectedDeck.Name, font, Brushes.White, 5, 580);
                    font.Dispose();
                }
            }
            catch (Exception ex)
            {
                NLog.Error("DeckViewForm_Paint" + ex);
            }
        }

    }
}
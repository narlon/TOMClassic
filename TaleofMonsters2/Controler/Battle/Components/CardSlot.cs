﻿using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ConfigDatas;
using TaleofMonsters.Controler.Battle.Data.MemCard;
using TaleofMonsters.Controler.Battle.Tool;
using TaleofMonsters.Core;
using TaleofMonsters.Core.Config;
using TaleofMonsters.Core.Loader;
using TaleofMonsters.Datas;
using TaleofMonsters.Datas.Cards;
using TaleofMonsters.Datas.Decks;

namespace TaleofMonsters.Controler.Battle.Components
{
    internal sealed class CardSlot
    {
        public bool MouseOn { get; set; }

        public ActiveCard ACard { get; private set; }
        private Card card;

        public Point Location;
        public Size Size;
        public bool Enabled = true;
        public Color BgColor = Color.DimGray;

        public bool IsSelected { get; set; }

        public CardSlot()
        {

        }

        public void SetSlotCard(ActiveCard tcard)
        {
            ACard = tcard;
            card = CardAssistant.GetCard(tcard.CardId);
            card.SetData(tcard);
        }

        public void CardSlot_Paint( PaintEventArgs e)
        {
            if (card != null)
            {
                CardMouseState state = !Enabled ? CardMouseState.Disable : (MouseOn ? CardMouseState.MouseOn : CardMouseState.Normal);
                Draw(e.Graphics, state);
            }
        }

        private void Draw(Graphics g, CardMouseState mouse)
        {
            if (card is SpecialCard)
            {
             //   Image img2 = Card.GetCardImage(120, 120);
             //   g.DrawImage(img2, new Rectangle(Location.X, 10, 120, 120), 0, 0, img2.Width, img2.YCount, GraphicsUnit.Pixel);
                return;
            }

            int x = Location.X;
            int y = Location.Y;
            Color bgColor = IsSelected ? Color.LightGreen : BgColor;
            SolidBrush sb = new SolidBrush(bgColor);
            g.FillRectangle(sb, new Rectangle(x, y, Size.Width, Size.Height));
            sb.Dispose();
            if (mouse != CardMouseState.MouseOn)
                y += 10;
            CardAssistant.DrawBase(g, card.CardId, x, y, Size.Width, 120);

            if (mouse == CardMouseState.Disable)
            {
                var sbrush = new SolidBrush(Color.FromArgb(150, Color.Black));
                g.FillRectangle(sbrush, x, y, Size.Width, 120);
                sbrush.Dispose();
            }

            var cardData = CardConfigManager.GetCardConfig(card.CardId);
            if (BattleManager.Instance.PlayerManager.LeftPlayer.Combo && cardData.Remark.Contains("连击"))
            {
                Image img = PicLoader.Read("System", "CardEff1.PNG");
                g.DrawImage(img, x + 2, y + 2, Size.Width - 4, 120 - 4);
                img.Dispose();
            }
            if (BattleManager.Instance.PlayerManager.LeftPlayer.IsLastSpellAttr(cardData.Attr) && cardData.Remark.Contains("元素"))
            {
                Image img = PicLoader.Read("System", "CardEff1.PNG"); //todo 先用这个
                g.DrawImage(img, x + 2, y + 2, Size.Width - 4, 120 - 4);
                img.Dispose();
            }

            Font font = new Font("Arial", 7*1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
            for (int i = 0; i < card.Star; i++)
            {
                g.DrawString("★", font, Brushes.Black, x + Size.Width - 16, y+2+i*10);
                g.DrawString("★", font, Brushes.Yellow, x + Size.Width - 15, y + 1 + i * 10);
            }
            font.Dispose();


            if (card.GetCardType() == CardTypes.Monster)
                g.DrawImage(HSIcons.GetIconsByEName("rac" + (int)cardData.TypeSub), x+ Size.Width/2-18, y+90, 16, 16);
            else if (card.GetCardType() == CardTypes.Weapon)
                g.DrawImage(HSIcons.GetIconsByEName("wep" + (int)(cardData.TypeSub-100+1)), x + Size.Width / 2 - 18, y + 90, 16, 16);
            else if (card.GetCardType() == CardTypes.Spell)
                g.DrawImage(HSIcons.GetIconsByEName("spl" + (int)(cardData.TypeSub-200+1)), x + Size.Width / 2 - 18, y + 90, 16, 16);
            g.DrawImage(HSIcons.GetIconsByEName("atr" + cardData.Attr), x + Size.Width / 2 + 2, y + 90, 16, 16);

            font = new Font("宋体", 9*1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
            var cardName = string.Format("{0}Lv{1}", card.Name, ACard.Level);
            var cardQual = CardConfigManager.GetCardConfig(card.CardId).Quality;
            var cardColor = Color.FromName(HSTypes.I2QualityColor((int)cardQual));
            var brush = new SolidBrush(cardColor);
            g.DrawString(cardName, font, Brushes.Black, x + 1, mouse != CardMouseState.MouseOn ? y + 107 : y+ 112);
            g.DrawString(cardName, font,brush, x, mouse != CardMouseState.MouseOn ? y+ 106 :y+ 111);
            font.Dispose();
            brush.Dispose();

            int index = 0;
            foreach (var manaType in GetCostList(ACard))
            {
                var imgIcon = HSIcons.GetIconsByEName("mix" + (int)manaType);
                g.DrawImage(imgIcon, x + index * 10, y + 1, 16, 16);
                index++;
            }
            if (index == 0)//无消耗
            {
                font = new Font("宋体", 9 * 1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
                g.DrawString("无消耗", font, Brushes.White, x + index * 10+2, y + 1+2);
                font.Dispose();
            }

        }

        private IEnumerable<PlayerManaTypes> GetCostList(ActiveCard card1)
        {
            List<PlayerManaTypes> l = new List<PlayerManaTypes>();
            for (int i = 0; i < card1.Lp; i++)
                l.Add(PlayerManaTypes.Lp);
            for (int i = 0; i < card1.Mp; i++)
                l.Add(PlayerManaTypes.Mp);
            for (int i = 0; i < card1.Pp; i++)
                l.Add(PlayerManaTypes.Pp);
            return l;
        }
    }
}

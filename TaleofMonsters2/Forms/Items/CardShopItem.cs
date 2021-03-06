﻿using System;
using System.Drawing;
using System.Windows.Forms;
using ConfigDatas;
using ControlPlus;
using NarlonLib.Math;
using TaleofMonsters.Core;
using TaleofMonsters.Core.Config;
using TaleofMonsters.Core.Loader;
using TaleofMonsters.Datas;
using TaleofMonsters.Datas.Cards;
using TaleofMonsters.Datas.Cards.Monsters;
using TaleofMonsters.Datas.Effects;
using TaleofMonsters.Datas.Effects.Facts;
using TaleofMonsters.Datas.Others;
using TaleofMonsters.Datas.User;
using TaleofMonsters.Datas.User.Db;
using TaleofMonsters.Forms.CMain;
using TaleofMonsters.Forms.Items.Regions;

namespace TaleofMonsters.Forms.Items
{
    internal class CardShopItem : ICellItem
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get { return 85; } }
        public int Height { get { return 125; } }

        private DbCardProduct product;
        private bool show;
        private ImageToolTip tooltip = SystemToolTip.Instance;
        private VirtualRegion vRegion;

        private BasePanel parent;
        private StaticUIEffect coverEffect;
        private Image backImg;
        private int renderIndex = 0;

        public CardShopItem(BasePanel prt)
        {
            parent = prt;
            backImg = PicLoader.Read("System", "CardBack2.JPG");
        }

        public void Init(int idx)
        {
            vRegion = new VirtualRegion(parent);
            vRegion.AddRegion(new SubVirtualRegion(1, X + 12, Y + 14, 64, 84));
            vRegion.AddRegion(new ButtonRegion(2, X + 55, Y + 102, 17, 17, "SBuyIcon.PNG", "SBuyIconOn.PNG"));
            vRegion.RegionEntered += new VirtualRegion.VRegionEnteredEventHandler(virtualRegion_RegionEntered);
            vRegion.RegionLeft += new VirtualRegion.VRegionLeftEventHandler(virtualRegion_RegionLeft);
            vRegion.RegionClicked += new VirtualRegion.VRegionClickEventHandler(virtualRegion_RegionClicked);
        }

        public void RefreshData(object data)
        {
            var pro = (DbCardProduct)data;
            show = pro.Id != 0;
            product = pro;
            if (product.Id != 0)
                vRegion.SetRegionKey(1, product.Cid);

            string effectName = "";
            var card =  CardAssistant.GetCard(product.Cid);
            if (card.GetCardType() == CardTypes.Monster)
            {
                MonsterConfig monsterConfig = ConfigData.GetMonsterConfig(product.Cid);
                foreach (var skill in MonsterBook.GetSkillList(monsterConfig.Id))
                {
                    int skillId = skill.Id;
                    SkillConfig skillConfig = ConfigData.GetSkillConfig(skillId);
                    if (skillConfig.Cover != null)
                        effectName = skillConfig.Cover;
                }
                if (monsterConfig.Cover != "")
                    effectName = monsterConfig.Cover;
            }

            string nowEffectName = "";
            if (coverEffect != null)
                nowEffectName = coverEffect.Name;

            if (effectName != nowEffectName)
            {
                if (effectName == "")
                {
                    coverEffect = null;
                }
                else
                {
                    coverEffect = new StaticUIEffect(EffectBook.GetEffect(effectName), new Point(X + 12, Y + 14), new Size(64, 84));
                    coverEffect.Repeat = true;
                }
            }

            parent.Invalidate(new Rectangle(X + 12, Y + 14, 64, 84));
        }
        
        public void OnFrame()
        {
            if (coverEffect != null)
            {
                renderIndex++;
                if (coverEffect.Next() & (renderIndex % 3) == 0) //降频，降低cpu开销
                    parent.Invalidate(new Rectangle(X + 12, Y + 14, 64, 84));
            }
        }

        private void virtualRegion_RegionEntered(int info, int mx, int my, int key)
        {
            if (info == 1 && product.Id > 0)
            {
                Image image = CardAssistant.GetCard(product.Cid).GetPreview(ShowSellInfo);
                tooltip.Show(image, parent, mx, my, product.Id);
            }
        }

        private void ShowSellInfo(TipImage tipData)
        {
            tipData.AddLine();
            tipData.AddTextNewLine("价格", "White");
            var resData = GetPrice().ToArray();
            for (int i = 0; i < 7; i++)
            {
                if (resData[i] > 0)
                {
                    tipData.AddText(" " + resData[i].ToString(), HSTypes.I2ResourceColor(i));
                    tipData.AddImage(HSIcons.GetIconsByEName("res" + (i + 1)));
                }
            }
        }

        private void virtualRegion_RegionLeft()
        {
            tooltip.Hide(parent, product.Id);
        }

        private void virtualRegion_RegionClicked(int info, int tx, int ty, MouseButtons button)
        {
            if (info == 2 && product.Id > 0)
            {
                GameResource res = GetPrice();
                if (UserProfile.InfoBag.CheckResource(res.ToArray()))
                {
                    UserProfile.InfoBag.SubResource(res.ToArray());
                    (parent as CardShopViewForm).OnSelect(product);
                }
                else
                {
                    parent.AddFlowCenter(HSErrors.GetDescript(ErrorConfig.Indexer.BagNotEnoughResource), "Red");
                }
            }
        }
        
        private GameResource GetPrice()
        {
            var cardData = CardConfigManager.GetCardConfig(product.Cid);

            GameResource res = new GameResource();
            res.Gold = (uint)cardData.Star * 30;
            var markType = (CardProductMarkTypes)product.Mark;
            if (markType == CardProductMarkTypes.Sale)
                res.Gold = (uint)MathTool.GetRound((int)res.Gold, 20);
            else if (markType == CardProductMarkTypes.Gold)
                res.Gold = res.Gold * 12 / 10;
            else if (markType == CardProductMarkTypes.Hot)
                res.Gold = res.Gold * 8 / 10;
            else if (markType == CardProductMarkTypes.Only)
                res.Gold = 300;

            if (cardData.Type == CardTypes.Monster)
                res.Add(GameResourceType.Carbuncle, GameResourceBook.OutCarbuncleCardBuy((int)cardData.Quality));
            else if (cardData.Type == CardTypes.Weapon)
                res.Add(GameResourceType.Gem, GameResourceBook.OutGemCardBuy((int)cardData.Quality));
            else if (cardData.Type == CardTypes.Spell)
                res.Add(GameResourceType.Mercury, GameResourceBook.OutMercuryCardBuy((int)cardData.Quality));
            return res;
        }

        public void Draw(Graphics g)
        {
            g.DrawImage(backImg, X, Y, Width - 1, Height - 1);

            if (show)
            {
             //   g.FillRectangle(PaintTool.GetBrushByAttribute(CardConfigManager.GetCardConfig(product.Cid).Attr), x + 10, y + 12, 70 - 2, 90 - 2);

                vRegion.Draw(g);

                CardAssistant.DrawBase(g, product.Cid, X + 12, Y + 14, 64, 84);
                
                if (coverEffect != null)
                    coverEffect.Draw(g);

                if ((CardProductMarkTypes)product.Mark != CardProductMarkTypes.Null)
                {
                    Image marker = PicLoader.Read("System", string.Format("Mark{0}.PNG", (int)product.Mark));
                    g.DrawImage(marker, X + 28, Y+12, 50, 51);
                    marker.Dispose();
                }

                var cardConfigData = CardConfigManager.GetCardConfig(product.Cid);
                var quality = cardConfigData.Quality + 1;
                g.DrawImage(HSIcons.GetIconsByEName("gem" + (int) quality), X + Width/2 - 8, Y + Height - 44, 16, 16);

            }
        }

        public void Dispose()
        {
            backImg.Dispose();
            backImg = null;
        }
    }
}

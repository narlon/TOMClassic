﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ControlPlus;
using NarlonLib.Core;
using NarlonLib.Math;
using TaleofMonsters.Core;
using TaleofMonsters.Forms.Items.Core;
using TaleofMonsters.Core.Config;
using TaleofMonsters.Core.Loader;
using TaleofMonsters.Datas;
using TaleofMonsters.Datas.Cards;
using TaleofMonsters.Datas.User;
using TaleofMonsters.Forms.CMain;
using TaleofMonsters.Forms.Items.Regions;

namespace TaleofMonsters.Forms
{
    internal sealed partial class SelectDungeonCard2Form : BasePanel
    {
        private ImageToolTip tooltip = SystemToolTip.Instance;
        private VirtualRegion vRegion;
        private List<NLPair<int, int>> cardIdList; //id,level

        public BasePanel Parent { get; set; }

        public SelectDungeonCard2Form()
        {
            InitializeComponent();

            bitmapButtonSelect.ImageNormal = PicLoader.Read("Button.Panel", "ButtonBack2.PNG");
            bitmapButtonSelect.Font = new Font("宋体", 8*1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
            bitmapButtonSelect.ForeColor = Color.White;
            bitmapButtonSelect.IconImage = TaleofMonsters.Core.HSIcons.GetIconsByEName("oth2");
            bitmapButtonSelect.IconSize = new Size(16, 16);
            bitmapButtonSelect.IconXY = new Point(8, 5);
            bitmapButtonSelect.TextOffX = 8;

            bitmapButtonSelect2.ImageNormal = PicLoader.Read("Button.Panel", "ButtonBack2.PNG");
            bitmapButtonSelect2.Font = new Font("宋体", 8 * 1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
            bitmapButtonSelect2.ForeColor = Color.White;
            bitmapButtonSelect2.IconImage = TaleofMonsters.Core.HSIcons.GetIconsByEName("oth2");
            bitmapButtonSelect2.IconSize = new Size(16, 16);
            bitmapButtonSelect2.IconXY = new Point(8, 5);
            bitmapButtonSelect2.TextOffX = 8;


            bitmapButtonSelect3.ImageNormal = PicLoader.Read("Button.Panel", "ButtonBack2.PNG");
            bitmapButtonSelect3.Font = new Font("宋体", 8 * 1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
            bitmapButtonSelect3.ForeColor = Color.White;
            bitmapButtonSelect3.IconImage = TaleofMonsters.Core.HSIcons.GetIconsByEName("oth2");
            bitmapButtonSelect3.IconSize = new Size(16, 16);
            bitmapButtonSelect3.IconXY = new Point(8, 5);
            bitmapButtonSelect3.TextOffX = 8;

            bitmapButtonGiveup.ImageNormal = PicLoader.Read("Button.Panel", "ButtonBack2.PNG");
            bitmapButtonGiveup.Font = new Font("宋体", 8 * 1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
            bitmapButtonGiveup.ForeColor = Color.White;
            bitmapButtonGiveup.IconImage = TaleofMonsters.Core.HSIcons.GetIconsByEName("rot4");
            bitmapButtonGiveup.IconSize = new Size(16, 16);
            bitmapButtonGiveup.IconXY = new Point(8, 5);
            bitmapButtonGiveup.TextOffX = 8;
        }

        public override void Init(int width, int height)
        {
            base.Init(width, height);
            cardIdList = new List<NLPair<int, int>>();
            for (int i = 0; i < 3; i++)
            {
                var rdCard = 0;
                if (MathTool.IsRandomInRange01(0.25f))
                    rdCard = CardConfigManager.GetRandomJobCard(UserProfile.InfoDungeon.JobId);
                else
                    rdCard = CardConfigManager.GetRandomJobCard(0);
                cardIdList.Add(new NLPair<int, int>(rdCard, MathTool.IsRandomInRange01(0.2f) ? 2 : 0));
            }

            vRegion = new VirtualRegion(this);
            for (int i = 0; i < 3; i++)
            {
                vRegion.AddRegion(new PictureAnimRegion(10+i, 20, 40 + i * 100, 80, 80, PictureRegionCellType.Card, cardIdList[i].Value1));
            }
        
            vRegion.RegionEntered += new VirtualRegion.VRegionEnteredEventHandler(virtualRegion_RegionEntered);
            vRegion.RegionLeft += new VirtualRegion.VRegionLeftEventHandler(virtualRegion_RegionLeft);
        }

        public override void RefreshInfo()
        {

        }

        private void virtualRegion_RegionEntered(int id, int x, int y, int key)
        {
            if (id > 0)
            {
                Image image = CardAssistant.GetCard(key).GetPreview(null);
                tooltip.Show(image, this, x, y, key);
            }
        }

        private void virtualRegion_RegionLeft()
        {
            tooltip.Hide(this);
        }

        private void SelectDungeonCard2Form_Paint(object sender, PaintEventArgs e)
        {
            BorderPainter.Draw(e.Graphics, "", Width, Height);

            Font font = new Font("黑体", 12*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            e.Graphics.DrawString("选择卡牌", font, Brushes.White, Width / 2 - 40, 8);
            font.Dispose();

            if(vRegion != null)
                vRegion.Draw(e.Graphics);

            Font fontTx = new Font("宋体", 11 * 1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            for (int i = 0; i < cardIdList.Count; i++)
            {
                var cardConfig = CardConfigManager.GetCardConfig(cardIdList[i].Value1);
                var brush = new SolidBrush(Color.FromName(HSTypes.I2QualityColor((int)cardConfig.Quality)));
                var lvStr = cardConfig.Name;
                if (cardIdList[i].Value2 > 0)
                    lvStr = string.Format("{0} Lv+{1}", cardConfig.Name, cardIdList[i].Value2);
                e.Graphics.DrawString(lvStr, fontTx, brush, 117, 52 + 100*i);
                brush.Dispose();
            }
            fontTx.Dispose();
        }

        private void bitmapButtonSelect_Click(object sender, EventArgs e)
        {
            AddDCard(0);
            Close();
        }

        private void AddDCard(int index)
        {
            var cardId = cardIdList[index].Value1;
            UserProfile.InfoCard.AddDungeonCard(cardId, cardIdList[index].Value2);
            if (Parent != null)
                Parent.AddFlowCenter("获得卡牌", "Lime", CardAssistant.GetCardImage(cardId, 40, 40));
        }

        private void bitmapButtonSelect2_Click(object sender, EventArgs e)
        {
            AddDCard(1);
            Close();
        }

        private void bitmapButtonSelect3_Click(object sender, EventArgs e)
        {
            AddDCard(2);
            Close();
        }

        private void bitmapButtonGiveup_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
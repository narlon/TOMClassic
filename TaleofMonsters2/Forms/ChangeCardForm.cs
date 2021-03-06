﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ConfigDatas;
using ControlPlus;
using NarlonLib.Math;
using TaleofMonsters.Core;
using TaleofMonsters.Core.Loader;
using TaleofMonsters.Datas;
using TaleofMonsters.Datas.Cards.Monsters;
using TaleofMonsters.Datas.Others;
using TaleofMonsters.Datas.User;
using TaleofMonsters.Forms.Items;
using TaleofMonsters.Forms.CMain;
using TaleofMonsters.Forms.Items.Core;

namespace TaleofMonsters.Forms
{
    internal sealed partial class ChangeCardForm : BasePanel
    {
        internal class ChangeCardData
        {
            public int Id1;
            public int Type1;
            public int Id2;
            public int Type2;
            public bool Used;

            public bool IsEmpty()
            {
                return Id1 == 0 && Id2 == 0;
            }
        }

        private List<ChangeCardData> changes;
        private CellItemBox itemBox;
        private ColorWordRegion colorWord;

        public ChangeCardForm()
        {
            InitializeComponent();
            this.bitmapButtonClose.ImageNormal = PicLoader.Read("Button.Panel", "CloseButton1.JPG");
            bitmapButtonClose.NoUseDrawNine = true;
            this.bitmapButtonRefresh.ImageNormal = PicLoader.Read("Button.Panel", "PlusButton.JPG");
            bitmapButtonRefresh.NoUseDrawNine = true;
            this.bitmapButtonFresh.ImageNormal = PicLoader.Read("Button.Panel", "FreshButton.JPG");
            bitmapButtonFresh.NoUseDrawNine = true;
            colorWord = new ColorWordRegion(12, 38, 384, new Font("微软雅黑", 11 * 1.33f, FontStyle.Bold, GraphicsUnit.Pixel), Color.White);
            Graphics g = this.CreateGraphics();
            colorWord.UpdateText("|交换公式随机出现，交换公式的|Lime|背景颜色||决定交换公式的最高品质。", g);
            g.Dispose();

            itemBox = new CellItemBox(8, 111, 193 * 2, 56 * 4);
        }

        public override void Init(int width, int height)
        {
            base.Init(width, height);

            for (int i = 0; i < 8; i++)
            {
                var item = new ChangeCardItem(this);
                itemBox.AddItem(item);
                item.Init(i);
            }
            GetChangeCardData();
            RefreshInfo();
            OnFrame(0, 0);
        }

        public override void RefreshInfo()
        {
            for (int i = 0; i < 8; i++)
                itemBox.Refresh(i,changes.Count > i ? changes[i] : new ChangeCardData());
            bitmapButtonRefresh.Visible = changes.Count < 8;
        }

        private void pictureBoxCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void bitmapButtonRefresh_Click(object sender, EventArgs e)
        {
            var cost = GameResourceBook.OutSulfurRefresh(0.5f);
            if (MessageBoxEx2.Show(string.Format("是否花{0}硫磺增加一条交换公式?", cost)) == DialogResult.OK)
            {
                if (UserProfile.InfoBag.HasResource(GameResourceType.Sulfur, cost))
                {
                    UserProfile.InfoBag.SubResource(GameResourceType.Sulfur, cost);
                    AddChangeCardData();
                    RefreshInfo();
                }
                else
                {
                    MainTipManager.AddTip(HSErrors.GetDescript(ErrorConfig.Indexer.BagNotEnoughResource), "Red");
                }
            }
        }

        private void bitmapButtonFresh_Click(object sender, EventArgs e)
        {
            var cost = GameResourceBook.OutSulfurRefresh(1);
            if (MessageBoxEx2.Show(string.Format("是否花{0}硫磺刷新所有交换公式?", cost)) == DialogResult.OK)
            {
                if (UserProfile.InfoBag.HasResource(GameResourceType.Sulfur, cost))
                {
                    UserProfile.InfoBag.SubResource(GameResourceType.Sulfur, cost);
                    RefreshAllChangeCardData();
                    RefreshInfo();
                }
                else
                {
                    MainTipManager.AddTip(HSErrors.GetDescript(ErrorConfig.Indexer.BagNotEnoughResource), "Red");
                }
            }
        }

        private void ChangeCardWindow_Paint(object sender, PaintEventArgs e)
        {
            BorderPainter.Draw(e.Graphics, "", Width, Height);

            Font font = new Font("黑体", 12*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            e.Graphics.DrawString("交换", font, Brushes.White, Width / 2 - 40, 8);
            font.Dispose();

            colorWord.Draw(e.Graphics);
            itemBox.Draw(e.Graphics);
        }

        public List<ChangeCardData> GetChangeCardData()
        {
            changes = new List<ChangeCardData>();
            for (int i = 0; i < 5; i++)
                changes.Add(CreateMethod(i));
            return changes;
        }

        private void AddChangeCardData()
        {
            if (changes.Count < 8)
                changes.Add(CreateMethod(changes.Count));
        }

        private void RefreshAllChangeCardData()
        {
            int count = changes.Count;
            changes.Clear();
            for (int i = 0; i < count; i++)
                changes.Add(CreateMethod(i));
        }
        
        private ChangeCardData CreateMethod(int index)
        {
            ChangeCardData chg = new ChangeCardData();
            int level = MathTool.GetRandom(Math.Max(index / 2, 1), index / 2 + 3);
            chg.Id1 = MonsterBook.GetRandStarMid(level);
            while (true)
            {
                chg.Id2 = MonsterBook.GetRandStarMid(level);
                if (chg.Id2 != chg.Id1)
                    break;
            }
            chg.Type1 = 1;
            chg.Type2 = 1;

            return chg;
        }

        public override void OnRemove()
        {
            base.OnRemove();

            itemBox.Dispose();
        }
    }
}
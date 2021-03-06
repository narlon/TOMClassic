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
using TaleofMonsters.Datas.Items;
using TaleofMonsters.Datas.Others;
using TaleofMonsters.Datas.User;
using TaleofMonsters.Forms.Items;
using TaleofMonsters.Forms.CMain;
using TaleofMonsters.Forms.Items.Core;

namespace TaleofMonsters.Forms
{
    internal sealed partial class BuyPieceForm : BasePanel
    {
        internal class BuyPieceData
        {
            public int Id;
            public int Count;
            public bool Used;

            public bool IsEmpty()
            {
                return Id == 0;
            }
        }

        private List<BuyPieceData> changes;
        private CellItemBox itemBox;
        private ColorWordRegion colorWord;

        public BuyPieceForm()
        {
            InitializeComponent();
            this.bitmapButtonClose.ImageNormal = PicLoader.Read("Button.Panel", "CloseButton1.JPG");
            bitmapButtonClose.NoUseDrawNine = true;
            this.bitmapButtonRefresh.ImageNormal = PicLoader.Read("Button.Panel", "PlusButton.JPG");
            bitmapButtonRefresh.NoUseDrawNine = true;
            this.bitmapButtonFresh.ImageNormal = PicLoader.Read("Button.Panel", "FreshButton.JPG");
            bitmapButtonFresh.NoUseDrawNine = true;
            this.bitmapButtonDouble.ImageNormal = PicLoader.Read("Button.Panel", "MoneyButton.JPG");
            bitmapButtonDouble.NoUseDrawNine = true;
            colorWord = new ColorWordRegion(12, 38, 384, new Font("微软雅黑", 11 * 1.33f, FontStyle.Bold, GraphicsUnit.Pixel), Color.White);

            Graphics g = this.CreateGraphics();
            colorWord.UpdateText("|所有素材随机出现，素材的|Lime|背景颜色||决定素材的最高品质。", g);
            g.Dispose();

            itemBox = new CellItemBox(8, 111, 193 * 2, 56 * 4);
        }

        public override void Init(int width, int height)
        {
            base.Init(width, height);

            for (int i = 0; i < 8; i++)
            {
                var item = new PieceItem(this);
                itemBox.AddItem(item);
                item.Init(i);
            }
            RemakePieceData();
            RefreshInfo();
            OnFrame(0, 0);
        }

        public override void RefreshInfo()
        {
            for (int i = 0; i < 8; i++)
                itemBox.Refresh(i, changes.Count > i ? changes[i] : new BuyPieceData());
            bitmapButtonRefresh.Visible = changes.Count < 8;
        }

        private void pictureBoxCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void bitmapButtonRefresh_Click(object sender, EventArgs e)
        {
            var cost = GameResourceBook.OutSulfurRefresh(0.5f);
            if (MessageBoxEx2.Show(string.Format("是否花{0}硫磺增加一条素材?", cost)) == DialogResult.OK)
            {
                if (UserProfile.InfoBag.HasResource(GameResourceType.Sulfur, cost))
                {
                    UserProfile.InfoBag.SubResource(GameResourceType.Sulfur, cost);
                    AddPieceData();
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
            if (MessageBoxEx2.Show(string.Format("是否花{0}硫磺刷新所有素材?", cost)) == DialogResult.OK)
            {
                if (UserProfile.InfoBag.HasResource(GameResourceType.Sulfur, cost))
                {
                    UserProfile.InfoBag.SubResource(GameResourceType.Sulfur, cost);
                    RefreshAllPieceData();
                    RefreshInfo();
                }
                else
                {
                    MainTipManager.AddTip(HSErrors.GetDescript(ErrorConfig.Indexer.BagNotEnoughResource), "Red");
                }
            }
        }
        private void bitmapButtonDouble_Click(object sender, EventArgs e)
        {
            var cost = GameResourceBook.OutSulfurRefresh(0.5f);
            if (MessageBoxEx2.Show(string.Format("是否花{0}硫磺翻倍所有素材数量?", cost)) == DialogResult.OK)
            {
                if (UserProfile.InfoBag.HasResource(GameResourceType.Sulfur, cost))
                {
                    UserProfile.InfoBag.SubResource(GameResourceType.Sulfur, cost);
                    DoubleAllPieceData();
                    RefreshInfo();
                }
                else
                {
                    MainTipManager.AddTip(HSErrors.GetDescript(ErrorConfig.Indexer.BagNotEnoughResource), "Red");
                }
            }
        }

        private void BuyPieceForm_Paint(object sender, PaintEventArgs e)
        {
            BorderPainter.Draw(e.Graphics, "", Width, Height);

            Font font = new Font("黑体", 12*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            e.Graphics.DrawString("素材收集人", font, Brushes.White, Width / 2 - 40, 8);
            font.Dispose();

            colorWord.Draw(e.Graphics);
            itemBox.Draw(e.Graphics);
        }

        private void RemakePieceData()
        {
            changes = new List<BuyPieceData>();
            for (int i = 0; i < 5; i++)
                changes.Add(CreatePieceMethod(i));
        }

        private void AddPieceData()
        {
            if (changes.Count < 8)
                changes.Add(CreatePieceMethod(changes.Count));
        }

        private void RefreshAllPieceData()
        {
            int count = changes.Count;
            changes.Clear();
            for (int i = 0; i < count; i++)
                changes.Add(CreatePieceMethod(i));
        }

        private void DoubleAllPieceData()
        {
            foreach (var memNpcPieceData in changes)
                memNpcPieceData.Count *= 2;
        }

        private BuyPieceData CreatePieceMethod(int index)
        {
            BuyPieceData piece = new BuyPieceData();
            int rare = MathTool.GetRandom(Math.Max(index / 2, 1), index / 2 + 3);
            piece.Id = HItemBook.GetRandRareItemIdWithGroup(HItemRandomGroups.Fight, rare);
            piece.Count = MathTool.GetRandom((8 - rare) / 2, 8 - rare);

            return piece;
        }

        public override void OnRemove()
        {
            base.OnRemove();
            itemBox.Dispose();
        }
    }
}
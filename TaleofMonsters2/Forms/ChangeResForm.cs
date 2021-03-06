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
using TaleofMonsters.Datas.Others;
using TaleofMonsters.Datas.User;
using TaleofMonsters.Forms.Items;
using TaleofMonsters.Forms.CMain;
using TaleofMonsters.Forms.Items.Core;

namespace TaleofMonsters.Forms
{
    internal sealed partial class ChangeResForm : BasePanel
    {
        internal class ChangeResData
        {
            public int Id1;
            public uint Count1;
            public int Id2;
            public uint Count2;
            public bool Used;

            public bool IsEmpty()
            {
                return Id1 == 0 && Id2 == 0;
            }
        }

        private List<ChangeResData> changes;
        private CellItemBox itemBox;
        private ColorWordRegion colorWord;

        public ChangeResForm()
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
            colorWord.UpdateText("|交易公式随机出现，交易公式的|Lime|背景颜色||决定交易公式的品质。品质越高交换性价比越高。", g);
            g.Dispose();
            itemBox = new CellItemBox(8, 111, 193 * 2, 56 * 4);
        }


        public override void Init(int width, int height)
        {
            base.Init(width, height);

            for (int i = 0; i < 8; i++)
            {
                var item = new ChangeResItem(this);
                itemBox.AddItem(item);
                item.Init(i);
            }
            GetChangeResData();
            RefreshInfo();
            OnFrame(0, 0);
        }

        public override void RefreshInfo()
        {
            for (int i = 0; i < 8; i++)
                itemBox.Refresh(i, changes.Count > i ? changes[i] : new ChangeResData());
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
                    RefreshAllChangeResData();
                    RefreshInfo();
                }
                else
                {
                    MainTipManager.AddTip(HSErrors.GetDescript(ErrorConfig.Indexer.BagNotEnoughResource), "Red");
                }
            }
        }

        private void ChangeResWindow_Paint(object sender, PaintEventArgs e)
        {
            BorderPainter.Draw(e.Graphics, "", Width, Height);

            Font font = new Font("黑体", 12*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            e.Graphics.DrawString("期货", font, Brushes.White, Width / 2 - 40, 8);
            font.Dispose();

            colorWord.Draw(e.Graphics);
            itemBox.Draw(e.Graphics);
        }

        public List<ChangeResData> GetChangeResData()
        {
            changes = new List<ChangeResData>();
            for (int i = 0; i < 5; i++)
                changes.Add(CreateMethod(i));
            return changes;
        }

        private void AddChangeCardData()
        {
            if (changes.Count < 8)
                changes.Add(CreateMethod(changes.Count));
        }

        private void RefreshAllChangeResData()
        {
            int count = changes.Count;
            changes.Clear();
            for (int i = 0; i < count; i++)
                changes.Add(CreateMethod(i));
        }
        
        private ChangeResData CreateMethod(int index)
        {
            ChangeResData chg = new ChangeResData();
            int floor = index / 2;
            float cutOff = 1;
            if (floor == 0)
                cutOff = (float)(MathTool.GetRandom(3) + 9) / 10;
            else if (floor == 1)
                cutOff = (float)(MathTool.GetRandom(2) + 9) / 10;
            else if (floor == 2)
                cutOff = (float)(MathTool.GetRandom(4) + 7) / 10;
            else if (floor == 3)
                cutOff = (float)(MathTool.GetRandom(6) + 5) / 10;

            chg.Id1 = MathTool.GetRandom(6) + 1;//1是木材
            while (true)
            {
                chg.Id2 = MathTool.GetRandom(6) + 1;//1是木材
                if(chg.Id1 != chg.Id2)    
                    break;
            }

            uint[] counts = new uint[] {10, 10, 10, 30, 30, 50, 50, 100, 200};
            chg.Count1 = counts[MathTool.GetRandom(counts.Length)];
            chg.Count2 = (uint)(chg.Count1/cutOff);

            if (chg.Id1 <= (int) GameResourceType.Stone && chg.Id2 > (int) GameResourceType.Stone) //汇率问题
                chg.Count2 /= 3;
            if (chg.Id1 > (int)GameResourceType.Stone && chg.Id2 <= (int)GameResourceType.Stone) //汇率问题
                chg.Count2 *= 3;

            return chg;
        }
    }
}
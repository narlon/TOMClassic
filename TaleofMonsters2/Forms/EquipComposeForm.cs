﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ConfigDatas;
using ControlPlus;
using TaleofMonsters.Core.Loader;
using TaleofMonsters.Datas.User;
using TaleofMonsters.Forms.Items;
using TaleofMonsters.Forms.Items.Core;

namespace TaleofMonsters.Forms
{
    internal sealed partial class EquipComposeForm : BasePanel
    {
        private CellItemBox itemBox;
        private int page;
        private List<int> equipIdList;
        private ControlPlus.NLPageSelector nlPageSelector1;

        public EquipComposeForm()
        {
            InitializeComponent();            
            this.bitmapButtonClose.ImageNormal = PicLoader.Read("Button.Panel", "CloseButton1.JPG");
            this.bitmapButton1.ImageNormal = PicLoader.Read("Button.Panel", "ButtonBack3.PNG");
            this.bitmapButton2.ImageNormal = PicLoader.Read("Button.Panel", "ButtonBack1.PNG");
            this.bitmapButton3.ImageNormal = PicLoader.Read("Button.Panel", "ButtonBack1.PNG");
            this.bitmapButton4.ImageNormal = PicLoader.Read("Button.Panel", "ButtonBack1.PNG");
            this.bitmapButton5.ImageNormal = PicLoader.Read("Button.Panel", "ButtonBack1.PNG");

            this.nlPageSelector1 = new ControlPlus.NLPageSelector(this, 398, 310, 150);
            nlPageSelector1.PageChange += nlPageSelector1_PageChange;

            itemBox = new CellItemBox(10, 63, 180 * 3, 82 * 3);
        }

        public override void Init(int width, int height)
        {
            base.Init(width, height);

            for (int i = 0; i < 9; i++)
            {
                var item = new EquipComposeItem(this);
                itemBox.AddItem(item);
                item.Init(i);
            }
            InitEquips(1);
        }

        private void InitEquips(int pos)
        {
            equipIdList = new List<int>();
            Dictionary<int, bool> hasEquip = new Dictionary<int, bool>();
            foreach (var eData in UserProfile.InfoCastle.EquipAvail)//先显示有的
            {
                var equipConfig = ConfigData.GetEquipConfig(eData.BaseId);
                if (equipConfig.Id > 0 && equipConfig.Position == pos)
                {
                    equipIdList.Add(equipConfig.Id);
                    hasEquip[equipConfig.Id] = true;
                }
            }
            foreach (var eData in ConfigData.EquipDict.Values)//再显示没有的
            {
                var equipConfig = ConfigData.GetEquipConfig(eData.Id);
                if (equipConfig.Id > 0 && equipConfig.Position == pos && !hasEquip.ContainsKey(eData.Id))
                    equipIdList.Add(equipConfig.Id);
            }
            page = 0;
            nlPageSelector1.TotalPage = (equipIdList.Count - 1) / 9 + 1;
            RefreshInfo();
        }

        public override void RefreshInfo()
        {
            for (int i = 0; i < 9; i++)
                itemBox.Refresh(i, (page*9 + i < equipIdList.Count) ? equipIdList[page*9 + i] : 0);
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void EquipComposeForm_Paint(object sender, PaintEventArgs e)
        {
            BorderPainter.Draw(e.Graphics, "", Width, Height);

            Font font = new Font("黑体", 12*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            e.Graphics.DrawString(" 建造 ", font, Brushes.White, Width / 2 - 40, 8);
            font.Dispose();

            itemBox.Draw(e.Graphics);
        }

        private void nlPageSelector1_PageChange(int pg)
        {
            page = pg;
            RefreshInfo();
        }

        private void bitmapButton1_Click(object sender, EventArgs e)
        {
            InitEquips(int.Parse((sender as Control).Tag.ToString()));
        }

        public override void OnRemove()
        {
            base.OnRemove();
            itemBox.Dispose();
        }
    }
}
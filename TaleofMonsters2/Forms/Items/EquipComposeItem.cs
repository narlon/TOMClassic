﻿using System;
using System.Drawing;
using ConfigDatas;
using ControlPlus;
using TaleofMonsters.Core;
using TaleofMonsters.Core.Loader;
using TaleofMonsters.Datas.Equips;
using TaleofMonsters.Datas.Others;
using TaleofMonsters.Datas.User;
using TaleofMonsters.Forms.CMain;
using TaleofMonsters.Forms.Items.Regions;
using TaleofMonsters.Forms.Items.Regions.Decorators;
using TaleofMonsters.Forms.Pops;

namespace TaleofMonsters.Forms.Items
{
    internal class EquipComposeItem : ICellItem
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get { return 180; } }
        public int Height { get { return 82; } }

        private int index;
        private int equipId;
        private bool hasEquip; //是否已经拥有
        private bool show;
        private ImageToolTip tooltip = SystemToolTip.Instance;
        private VirtualRegion vRegion;

        private BasePanel parent;
        private BitmapButton bitmapButtonBuy;

        public EquipComposeItem(BasePanel prt)
        {
            parent = prt;
        }

        public void Init(int idx)
        {
            index = idx;

            vRegion = new VirtualRegion(parent);
            vRegion.AddRegion(new PictureRegion(1, X + 3 + 6, Y + 3 + 6,  76 - 12, 75 - 12, PictureRegionCellType.Equip, 0));
            vRegion.RegionEntered += new VirtualRegion.VRegionEnteredEventHandler(virtualRegion_RegionEntered);
            vRegion.RegionLeft += new VirtualRegion.VRegionLeftEventHandler(virtualRegion_RegionLeft);

            this.bitmapButtonBuy = new BitmapButton();
            bitmapButtonBuy.Location = new Point(X + 122, Y + 50);
            bitmapButtonBuy.Size = new Size(50, 24);
            this.bitmapButtonBuy.Click += new System.EventHandler(this.pictureBoxBuy_Click);
            this.bitmapButtonBuy.ImageNormal = PicLoader.Read("Button.Panel", "ButtonBack2.PNG");
            bitmapButtonBuy.Font = new Font("宋体", 8 * 1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
            bitmapButtonBuy.ForeColor = Color.White;
            bitmapButtonBuy.IconImage = TaleofMonsters.Core.HSIcons.GetIconsByEName("hatt2");
            bitmapButtonBuy.IconSize = new Size(16, 16);
            bitmapButtonBuy.IconXY = new Point(4, 4);
            bitmapButtonBuy.TextOffX = 8;
            this.bitmapButtonBuy.Text = @"改造";
            parent.Controls.Add(bitmapButtonBuy);
        }

        public void RefreshData(object data)
        {
            var eid = (int)data;
            equipId = eid;
            hasEquip = UserProfile.InfoCastle.HasEquip(eid);
            if (eid > 0)
            {
                bitmapButtonBuy.Visible = hasEquip;
                vRegion.SetRegionKey(1, eid);
                if (hasEquip)
                {
                    vRegion.SetRegionEnable(1, true);
                    vRegion.SetRegionDecorator(1, 0, null);
                    vRegion.SetRegionDecorator(1, 1, null);
                    var equipConfig = ConfigData.GetEquipConfig(eid);
                    var equipInfo = UserProfile.InfoCastle.GetEquipById(eid);
                    bitmapButtonBuy.Visible = equipInfo.Level < equipConfig.MaxLevel;
                }
                else
                {
                    vRegion.SetRegionEnable(1, false);
                    vRegion.SetRegionDecorator(1, 0, new RegionCoverDecorator(Color.FromArgb(180, Color.Black)));
                    vRegion.SetRegionDecorator(1, 1, new RegionImageDecorator(HSIcons.GetIconsByEName("oth3"), 24));
                }
                show = true;
            }
            else
            {
                vRegion.SetRegionKey(1, 0);
            //    vRegion.SetRegionKey(2, 0);
                bitmapButtonBuy.Visible = false;
                show = false;
            }

            parent.Invalidate(new Rectangle(X, Y, Width, Height));
        }

        private void virtualRegion_RegionEntered(int info, int mx, int my, int key)
        {
            if (info == 1 && key > 0)
            {
                Equip equipD = new Equip(key);

                var eData = UserProfile.InfoCastle.GetEquipById(key);
                if (eData != null && eData.Level > 1)
                    equipD.UpgradeToLevel(eData.Level);
                Image image = equipD.GetPreview();
                tooltip.Show(image, parent, mx, my, equipId);
            }
        }

        private void virtualRegion_RegionLeft()
        {
            tooltip.Hide(parent, equipId);
        }

        private void pictureBoxBuy_Click(object sender, EventArgs e)
        {
            PopBuildUpgrade.Show(equipId, parent);
        }

        public void Draw(Graphics g)
        {
            SolidBrush sb = new SolidBrush(Color.FromArgb(20, 20, 20));
            g.FillRectangle(sb, X + 2, Y + 2, Width - 4, Height - 4);
            sb.Dispose();
            g.DrawRectangle(Pens.Teal, X + 2, Y + 2, Width - 4, Height - 4);

            if (show)
            {
                var back = PicLoader.Read("System", "MapBack.JPG");
                g.DrawImage(back, X + 2, Y + 2, Width - 4, Height - 4);
                back.Dispose();

                vRegion.Draw(g);

                var equipConfig = ConfigData.GetEquipConfig(equipId);

                var textBack = PicLoader.Read("System", "TipBack.PNG");
                g.DrawImage(textBack, X + 82, Y + 8, 90, 16);
                textBack.Dispose();
                Font ft = new Font("宋体", 10*1.33f, FontStyle.Regular, GraphicsUnit.Pixel);

                if (hasEquip)
                {
                    var equipInfo = UserProfile.InfoCastle.GetEquipById(equipId);
                    Brush b = new SolidBrush(Color.FromName(HSTypes.I2QualityColor(equipConfig.Quality)));
                    g.DrawString(string.Format("{0}v{1}", equipConfig.Name, equipInfo.Level), ft, b, X + 82, Y + 10);
                    b.Dispose();

                    if (equipInfo.Level < equipConfig.MaxLevel)
                    {
                        string expstr = string.Format("{0}/{1}", equipInfo.Exp, ExpTree.GetNextRequiredEquip(equipInfo.Level));
                        g.DrawString(expstr, ft, Brushes.AliceBlue, X + 102, Y + 27);
                        g.FillRectangle(Brushes.DimGray, X + 82, Y + 42, 80, 4);
                        g.FillRectangle(Brushes.DodgerBlue, X + 82, Y + 42, Math.Min(equipInfo.Exp*79/ExpTree.GetNextRequiredEquip(equipInfo.Level) + 1, 80), 2);
                    }
                }
                else
                {
                    g.DrawString(equipConfig.Name, ft, Brushes.Gray, X + 82, Y + 10);
                }
                ft.Dispose();
            }
        }

        public void OnFrame()
        {
        }

        public void Dispose()
        {
        }
    }
}

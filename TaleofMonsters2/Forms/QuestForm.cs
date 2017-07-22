﻿using System;
using System.Drawing;
using System.Windows.Forms;
using ConfigDatas;
using NarlonLib.Control;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.Core;
using TaleofMonsters.DataType.Others;
using TaleofMonsters.DataType.User;
using TaleofMonsters.Forms.Items.Core;
using TaleofMonsters.Forms.Items.Regions;
using TaleofMonsters.Forms.Items.Regions.Decorators;

namespace TaleofMonsters.Forms
{
    internal sealed partial class QuestForm : BasePanel
    {
        private ImageToolTip tooltip = MainItem.SystemToolTip.Instance;
        private VirtualRegion virtualRegion;
        private ColorWordRegion colorWord;
        private int regionId;

        public QuestForm()
        {
            InitializeComponent();
            this.bitmapButtonClose.ImageNormal = PicLoader.Read("Button.Panel", "CloseButton1.JPG");
            regionId = 1;
            InitTasks();
        }

        public void InitTasks()
        {
            virtualRegion = new VirtualRegion(this);
            virtualRegion.RegionEntered += new VirtualRegion.VRegionEnteredEventHandler(virtualRegion_RegionEntered);
            virtualRegion.RegionLeft += new VirtualRegion.VRegionLeftEventHandler(virtualRegion_RegionLeft);
            RefreshQuests();

            colorWord = new ColorWordRegion(190, 84, 440, "微软雅黑", 11, Color.White);
        }

        public override void Init(int width, int height)
        {
            base.Init(width, height);

            comboBoxType.SelectedIndex = 0;
        }

        private void RefreshQuests()
        {
            virtualRegion.ClearRegion();
            int index = 0;
            foreach (var questData in ConfigData.QuestDict.Values)
            {
                if (questData.RegionId != regionId)
                {
                    continue;
                }
                Image img = null;
                Color borderColor = Color.Lime;
                if (UserProfile.InfoQuest.IsQuestNotReceive(questData.Id))
                {
                    img = HSIcons.GetIconsByEName("npc1");
                    borderColor = Color.Black;
                }
                else if (UserProfile.InfoQuest.IsQuestFinish(questData.Id))
                {
                    img = HSIcons.GetIconsByEName("npc4");
                    borderColor = Color.Goldenrod;
                }
                else if (UserProfile.InfoQuest.IsQuestCanReward(questData.Id))
                    img = HSIcons.GetIconsByEName("npc3");
                else
                    img = HSIcons.GetIconsByEName("npc2");
                var region = new ImageRegion(index++, 30 + questData.X * 24, 75 + questData.Y * 24, 20, 20, ImageRegionCellType.None, img);
                region.SetKeyValue(questData.Id);
                region.AddDecorator(new RegionBorderDecorator(borderColor));
                virtualRegion.AddRegion(region);
            }
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void virtualRegion_RegionEntered(int id, int x, int y, int key)
        {
            var questConfig = ConfigData.GetQuestConfig(key);
            if (UserProfile.InfoQuest.IsQuestNotReceive(questConfig.Id))
            {
                return;
            }
            Image image = QuestBook.GetPreview(questConfig.Id);
            tooltip.Show(image, this, x, y);
        }

        private void virtualRegion_RegionLeft()
        {
            tooltip.Hide(this);
        }

        private void TaskForm_Paint(object sender, PaintEventArgs e)
        {
            BorderPainter.Draw(e.Graphics, "", Width, Height);

            Font font = new Font("黑体", 12*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            e.Graphics.DrawString(" 任务 ", font, Brushes.White, Width / 2 - 40, 8);
            font.Dispose();

            colorWord.Draw(e.Graphics);
            
            font = new Font("黑体", 11 * 1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
            Pen darkPen = new Pen(Color.FromArgb(30,30,30));
            for (int i = 1; i <= 16; i++)
            {
                e.Graphics.DrawLine(darkPen, 40+i*24, 85, 40 + i * 24, 369);
                e.Graphics.DrawString((i).ToString(), font, Brushes.DimGray, 30 + i * 24, 67);
            }
            for (int i = 1; i <= 11; i++)
            {
                e.Graphics.DrawLine(darkPen, 40, 85 + i * 24, 448, 85 + i * 24);
                e.Graphics.DrawString(((char)('A'+i-1)).ToString(), font, Brushes.DimGray, 20, 77 + i * 24);
            }
            darkPen.Dispose();
            font.Dispose();
            e.Graphics.DrawRectangle(Pens.DimGray, 40, 85, 408, 284);
            virtualRegion.Draw(e.Graphics);
        }

        private void comboBoxType_SelectedIndexChanged(object sender, EventArgs e)
        {
            regionId = comboBoxType.SelectedIndex + 1;
            RefreshQuests();
            Invalidate();
        }

    }
}
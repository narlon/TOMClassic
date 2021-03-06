﻿using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using ConfigDatas;
using ControlPlus;
using NarlonLib.Core;
using TaleofMonsters.Core.Loader;
using TaleofMonsters.Datas;
using TaleofMonsters.Datas.CardPieces;
using TaleofMonsters.Datas.Cards;
using TaleofMonsters.Datas.Effects;
using TaleofMonsters.Datas.Effects.Facts;
using TaleofMonsters.Datas.Items;
using TaleofMonsters.Datas.User;
using TaleofMonsters.Forms.CMain;
using TaleofMonsters.Forms.Items.Core;
using TaleofMonsters.Forms.Items.Regions;
using TaleofMonsters.Rpc;

namespace TaleofMonsters.Forms.VBuilds
{
    internal sealed partial class HuntForm : BasePanel
    {
        private bool showImage;
        private ImageToolTip tooltip = SystemToolTip.Instance;
        private VirtualRegion vRegion;
        private VirtualRegionMoveMediator moveMediator;

        public HuntForm()
        {
            InitializeComponent();
            this.bitmapButtonClose.ImageNormal = PicLoader.Read("Button.Panel", "CloseButton1.JPG");
            this.bitmapButtonHelp.ImageNormal = PicLoader.Read("Button.Panel", "LearnButton.JPG");

            this.bitmapButtonC1.ImageNormal = PicLoader.Read("Button.Panel", "ButtonBack2.PNG");
            bitmapButtonC1.IconImage = TaleofMonsters.Core.HSIcons.GetIconsByEName("tsk1");
            this.bitmapButtonC2.ImageNormal = PicLoader.Read("Button.Panel", "ButtonBack2.PNG");
            bitmapButtonC2.IconImage = TaleofMonsters.Core.HSIcons.GetIconsByEName("tsk6");
        }

        public override void Init(int width, int height)
        {
            base.Init(width, height);

            showImage = true;
            vRegion = new VirtualRegion(this);
            
            vRegion.AddRegion(new PictureAnimRegion(10, 210, 100, 160, 160, PictureRegionCellType.Card, 0));
            for (int i = 0; i < 9; i++)
                vRegion.AddRegion(new PictureRegion(i+1, 36 + (i%3)*48, 90 + (i/ 3) * 48, 40, 40, PictureRegionCellType.Item, 0));

            UserProfile.InfoCastle.RefreshHuntMonster(false);
            UpdateMonsterInfo();

            moveMediator = new VirtualRegionMoveMediator(vRegion);

            vRegion.RegionEntered += new VirtualRegion.VRegionEnteredEventHandler(virtualRegion_RegionEntered);
            vRegion.RegionLeft += new VirtualRegion.VRegionLeftEventHandler(virtualRegion_RegionLeft);
        }

        public override void OnFrame(int tick, float timePass)
        {
            base.OnFrame(tick, timePass);
        }

        private void virtualRegion_RegionEntered(int id, int x, int y, int key)
        {
            if (id == 10)
            {
                Image image = CardAssistant.GetCard(key).GetPreview(null);
                tooltip.Show(image, this, x, y, key);
            }
            else if (key > 0)
            {
                Image image = HItemBook.GetPreview(key);
                tooltip.Show(image, this, x, y);
            }

        }

        private void virtualRegion_RegionLeft()
        {
            tooltip.Hide(this);
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void HuntForm_Paint(object sender, PaintEventArgs e)
        {
            BorderPainter.Draw(e.Graphics, "", Width, Height);

            Font font = new Font("黑体", 12*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            e.Graphics.DrawString("猎场", font, Brushes.White, Width / 2 - 40, 8);
            font.Dispose();

            if (!showImage)
                return;

            Image back = PicLoader.Read("Build", "hunt.JPG");
            e.Graphics.DrawImage(back, 15, 40, 572, 352);
            back.Dispose();

            vRegion.Draw(e.Graphics);

            font = new Font("宋体", 9 * 1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
         
            var hpTotal = ConfigData.GetMonsterConfig(UserProfile.InfoCastle.HuntMonsterId).Quality * 5 + 5;
            var hpLeft = UserProfile.InfoCastle.HuntHpLeft;
            e.Graphics.FillRectangle(Brushes.Red, 210, 88, 160, 12);
            e.Graphics.FillRectangle(Brushes.Lime, 210, 88, 160*hpLeft/hpTotal, 12);
            e.Graphics.DrawString(string.Format("血量 {0}/{1}", hpLeft, hpTotal), font, Brushes.Brown, 210+50, 88);

            Brush b = new SolidBrush(Color.FromArgb(200, Color.Black));
            e.Graphics.FillRectangle(b, 30, 57, 70, 20);
            e.Graphics.DrawString("掉落列表", font, Brushes.White, 33, 60);
            e.Graphics.FillRectangle(b, 30, 350, 100, 20);
            e.Graphics.DrawString(string.Format("可狩猎次数 {0}", UserProfile.InfoCastle.HuntPoint), font, Brushes.White, 33, 353);
            font.Dispose();
            b.Dispose();
        }

        private void bitmapButtonC1_Click(object sender, EventArgs e)
        {
            if (UserProfile.InfoCastle.HuntHpLeft <= 0)
                return;

            if (UserProfile.InfoCastle.HuntPoint <= 0)
            {
                AddFlowCenter("狩猎次数不足", "Red");
                return;
            }

            var effect = new StaticUIEffect(EffectBook.GetEffect("hit1"), new Point(210+30, 100+30), new Size(100, 100));
            effect.Repeat = false;
            AddEffect(effect);
            moveMediator.FireShake(10);

            UserProfile.InfoCastle.HuntHpLeft--;
            AddFlowCenter("-1", "Red"); //生命-1
            int itemId = CardPieceBook.CheckPieceDrop(UserProfile.InfoCastle.HuntMonsterId, 0);
            if (itemId > 0)
            {
                AddFlowCenter("+1", "Lime", HItemBook.GetHItemImage(itemId));
                UserProfile.InfoBag.AddItem(itemId, 1);
            }

            if (UserProfile.InfoCastle.HuntHpLeft == 0)
            {
                moveMediator.FireFadeOut(10);
                TalePlayer.Start(DelayRevive());
            }

            UserProfile.InfoCastle.HuntPoint--;
            Invalidate();
        }

        private IEnumerator DelayRevive()
        {
            yield return new NLWaitForSeconds(1f);
            UserProfile.InfoCastle.RefreshHuntMonster(false);
            UpdateMonsterInfo();
            vRegion.SetRegionDecorator(10, 0, null);
            Invalidate();
        }

        private void bitmapButtonC2_Click(object sender, EventArgs e)
        {
            if (ControlPlus.MessageBoxEx2.Show("是否花费5点狩猎点刷新怪物？") == DialogResult.Cancel)
            {
                return;
            }
            if (UserProfile.InfoCastle.HuntPoint < 5)
            {
                AddFlowCenter("狩猎次数不足", "Red");
                return;
            }
            UserProfile.InfoCastle.HuntPoint-=5;
            UserProfile.InfoCastle.RefreshHuntMonster(true);
            UpdateMonsterInfo();
            Invalidate();
        }

        private void bitmapButtonHelp_Click(object sender, EventArgs e)
        {
            MessageBoxEx.Show("通过完成事件和战斗可以累积能量");
        }

        private void UpdateMonsterInfo()
        {
            vRegion.SetRegionKey(10, UserProfile.InfoCastle.HuntMonsterId);
            for (int i = 0; i < 9; i++)
                vRegion.SetRegionKey(i + 1, 0);
            var itemList = CardPieceBook.GetDropListByCardId(UserProfile.InfoCastle.HuntMonsterId);
            for (int i = 0; i < itemList.Count; i++)
                vRegion.SetRegionKey(i + 1, itemList[i].ItemId);
        }
    }
}
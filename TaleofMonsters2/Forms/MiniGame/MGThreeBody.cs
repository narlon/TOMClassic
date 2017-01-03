﻿using System;
using System.Drawing;
using System.Windows.Forms;
using NarlonLib.Math;
using ControlPlus;
using TaleofMonsters.DataType.User;
using TaleofMonsters.Forms.Items.Core;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.Forms.Items.Regions.Decorators;
using TaleofMonsters.MainItem;
using TaleofMonsters.Forms.Items.Regions;

namespace TaleofMonsters.Forms.MiniGame
{
    internal partial class MGThreeBody : BasePanel
    {
        private const int WorkWater = 1;
        private const int WorkDry = 2;
        private const int WorkPopu = 3;
        private const int WorkFood = 4;
        private const int WorkSci = 5;

        private VirtualRegion virtualRegion;

        private bool show;
        private Image backImage;
        private int type;

        private int eraGoodBad;
        private int eraBadGood;
        private bool isGoodEra;
        private int population;
        private int populationDry;
        private int food;
        private int sci;

        private int selectWork;
        private bool isFail;

        private const int xoff = 11;
        private const int yoff = 129;

        public MGThreeBody()
        {
            type = (int)SystemMenuIds.GameThreeBody;
            InitializeComponent();

            virtualRegion = new VirtualRegion(this);
            for (int i = 0; i < 5; i++)
            {
                ButtonRegion region = new ButtonRegion(i + 1, 40 + 55 * i, 310, 50, 50, string.Format("GameSanTi{0}.PNG", i + 1), string.Format("GameSanTi{0}On.PNG", i + 1));
                region.AddDecorator(new RegionTextDecorator(10, 30, 10));
                virtualRegion.AddRegion(region);
            }
            virtualRegion.SetRegionDecorator(1, 0, "浸泡");
            virtualRegion.SetRegionDecorator(2, 0, "脱水");
            virtualRegion.SetRegionDecorator(3, 0, "医疗");
            virtualRegion.SetRegionDecorator(4, 0, "农耕");
            virtualRegion.SetRegionDecorator(5, 0, "科研");
            
            virtualRegion.RegionClicked += new VirtualRegion.VRegionClickEventHandler(virtualRegion_RegionClicked);
        }

        internal override void Init(int width, int height)
        {
            base.Init(width, height);
            this.bitmapButtonC1.ImageNormal = PicLoader.Read("ButtonBitmap", "ButtonBack2.PNG");
            bitmapButtonC1.Font = new Font("宋体", 8 * 1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
            bitmapButtonC1.ForeColor = Color.White;
            bitmapButtonC1.IconImage = TaleofMonsters.Core.HSIcons.GetIconsByEName("rot3");
            bitmapButtonC1.IconSize = new Size(16, 16);
            bitmapButtonC1.IconXY = new Point(4, 5);
            bitmapButtonC1.TextOffX = 8;

            this.bitmapButtonClose.ImageNormal = PicLoader.Read("ButtonBitmap", "CloseButton1.JPG");

            backImage = PicLoader.Read("MiniGame", "t5.JPG");
            show = true;

            RestartGame();
        }

        public void RestartGame()
        {
            eraGoodBad = MathTool.GetRandom(6, 15);
            eraBadGood = MathTool.GetRandom(6, 15);
            population = 50;
            populationDry = 0;
            food = 250;
            sci = 0;
            isGoodEra = true;
            isFail = false;
            ChangeWork(WorkPopu);            
        }

        public void EndGame()
        {
            if (!UserProfile.Profile.InfoMinigame.Has(type))
            {
                UserProfile.Profile.InfoMinigame.Add(type);
            }

            string hint;
            if (!isFail && sci >= 5000)
            {
                hint = "获得了游戏胜利";
                UserProfile.InfoBag.AddDiamond(10);
            }
            else
            {
                hint = "你输了";
            }

            if (MessageBoxEx2.Show(hint + ",是否花5钻石再试一次?") == DialogResult.OK)
            {
                if (UserProfile.InfoBag.PayDiamond(5))
                {
                    RestartGame();
                    return;
                }
            }

            Close();
        }

        private void ChangeWork(int id)
        {
            selectWork = id;
            for (int i = 0; i < 5; i++)
            {
                virtualRegion.SetRegionState(i + 1, RegionState.Free);
            }
            virtualRegion.SetRegionState(id, RegionState.Rectangled);
            Invalidate(new Rectangle(xoff, yoff, 324, 244));
        }

        private string GetState()
        {
            if (isGoodEra)
            {
                return eraGoodBad < 8 ? "平静" : eraGoodBad < 20 ? "普通" : "凶险";
            }
            return eraBadGood < 8 ? "黑暗" : eraBadGood < 20 ? "阴森" : "希望";
        }

        private void bitmapButtonC1_Click(object sender, EventArgs e)
        {
            int resetDice = MathTool.GetRandom(3);
            if (isGoodEra)
            {
                isGoodEra = MathTool.GetRandom(100) > eraGoodBad;
                if (!isGoodEra)
                {
                    AddFlowCenter("进入乱纪元", "Red");
                }
                if (resetDice == 0)
                {
                    eraGoodBad = MathTool.GetRandom(3, 30);
                }
            }
            else
            {
                isGoodEra = MathTool.GetRandom(100) <= eraBadGood;
                if (isGoodEra)
                {
                    AddFlowCenter("进入乱纪元", "Lime");
                }
                if (resetDice == 0)
                {
                    eraBadGood = MathTool.GetRandom(3, 30);
                }
            }

            int mood = Math.Max(30 - MathTool.GetRandom(eraGoodBad), 0);
            if (selectWork == WorkWater)
            {
                if (populationDry > 0)
                {
                    int change = Math.Min(Math.Max(MathTool.GetRandom(populationDry / 3, populationDry / 2), 10), populationDry);
                    population += change;
                    populationDry -= change;
                }
            }
            else if (selectWork == WorkDry)
            {
                if (population > 0)
                {
                    int change = Math.Min(Math.Max(MathTool.GetRandom(population / 6, population / 4), 20), population);
                    population -= change;
                    populationDry += change;
                }
            }
            else if (selectWork == WorkFood && isGoodEra && population > 0)
            {
                food += population * 4 + population * mood / 20;
            }
            else if (selectWork == WorkPopu && isGoodEra)
            {
                population += MathTool.GetRandom(5, 15)+population*(20+mood)/2000;
            }
            else if (selectWork == WorkSci && isGoodEra && population>0)
            {
                sci += population + population * mood / 50;
            }

            food -= population;
            if (food<0)
            {
                populationDry += food;
                if (populationDry<0)
                {
                    population += populationDry;
                    populationDry = 0;
                }
                food = 0;
            }

            if (isGoodEra)
            {
                populationDry = populationDry*14/15;
            }
            else
            {
                food -= Math.Max(10, food/10);
                population = population*Math.Min(MathTool.GetRandom(eraBadGood) + 60, 99)/100;
            }
            if ((population + populationDry) <= 0)
            {
                isFail = true;
                EndGame();
            }

            Invalidate(new Rectangle(xoff, yoff, 324, 244));
        }

        void virtualRegion_RegionClicked(int id, int x, int y, MouseButtons button)
        {
            if (button == MouseButtons.Left)
            {
                ChangeWork(id);
            }
        }

        private void bitmapButtonClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void DrawShadeText(Graphics g, string text, Font font, Brush Brush, int x, int y)
        {
            g.DrawString(text, font, Brushes.Black, x + 1, y + 1);
            g.DrawString(text, font, Brush, x, y);
        }

        private void MGUpToNumber_Paint(object sender, PaintEventArgs e)
        {
            BorderPainter.Draw(e.Graphics, "", Width, Height);

            Font font = new Font("黑体", 12*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            e.Graphics.DrawString("三体", font, Brushes.White, 150, 8);
            font.Dispose();

            if (!show)
                return;

            e.Graphics.DrawImage(backImage, xoff, yoff, 324, 244);

            Image img = PicLoader.Read("MiniGame.Planet", string.Format("star{0}.PNG", isGoodEra ? 1 : 2));
            e.Graphics.DrawImage(img, 50 + xoff, 50 + yoff, 100, 100);
            img.Dispose();

            virtualRegion.Draw(e.Graphics);

            font = new Font("宋体", 12*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            DrawShadeText(e.Graphics, string.Format("{0}({1})", isGoodEra ? "恒纪元" : "乱纪元", GetState()), font, isGoodEra?Brushes.Lime: Brushes.OrangeRed, xoff + 165, 35 + yoff);
            DrawShadeText(e.Graphics, string.Format("人口 {0}({1})", population, populationDry), font, Brushes.White, xoff+165, 65+yoff);
            DrawShadeText(e.Graphics, string.Format("食物 {0}", food), font, Brushes.White, xoff + 165, 95 + yoff);
            DrawShadeText(e.Graphics, string.Format("科技 {0}/5000", sci), font, Brushes.White, xoff + 165, 125 + yoff);            
            font.Dispose();
        }
    }
}

﻿using System;
using System.Drawing;
using NarlonLib.Math;
using TaleofMonsters.DataType;
using TaleofMonsters.DataType.Others;
using TaleofMonsters.DataType.User;
using TaleofMonsters.Forms.CMain.Blesses;
using TaleofMonsters.Forms.CMain.Quests.SceneQuests;

namespace TaleofMonsters.Forms.CMain.Quests
{
    internal class TalkEventItemRoll : TalkEventItem
    {
        private int rollItemX;
        private int rollItemSpeedX;

        private const int FrameOff = 10; //第一个柱子相叫最左边的偏移

        public TalkEventItemRoll(int evtId, int level, Rectangle r, SceneQuestEvent e)
            : base(evtId, level, r, e)
        {
            rollItemX = MathTool.GetRandom(0, pos.Width);
            rollItemSpeedX = MathTool.GetRandom(20, 40);
        }

        public override void Init()
        {
            if (BlessManager.RollAlwaysFailBig)
            {
                bool hasFailBig = evt.ParamList.Contains("大失败");
                if (hasFailBig)
                {
                    for (int i = 0; i < evt.ParamList.Count; i++)
                    {
                        if (evt.ParamList[i] == "失败")
                            evt.ParamList[i] = "大失败";
                    }
                }
            }
            if (BlessManager.RollAlwaysWinBig)
            {
                bool hasWinBig = evt.ParamList.Contains("大成功");
                if (hasWinBig)
                {
                    for (int i = 0; i < evt.ParamList.Count; i++)
                    {
                        if (evt.ParamList[i] == "成功")
                            evt.ParamList[i] = "大成功";
                    }
                }
            }
        }

        public override void OnFrame(int tick)
        {
            rollItemX += rollItemSpeedX;
            if (rollItemX < 0)
            {
                rollItemX = 0;
                rollItemSpeedX *= -1;
            }
            if (rollItemX > pos.Width- FrameOff*2)
            {
                rollItemX = pos.Width- FrameOff * 2;
                rollItemSpeedX *= -1;
            }

            if (MathTool.GetRandom(10) < 2)
            {
                if (rollItemSpeedX > 0)
                    rollItemSpeedX = rollItemSpeedX - MathTool.GetRandom(1, 3);
                else
                    rollItemSpeedX = rollItemSpeedX + MathTool.GetRandom(1, 3);
                if (Math.Abs(rollItemSpeedX) <= 1)
                    OnStop();
            }
        }

        private void OnStop()
        {
            if (result == null)
            {
                RunningState = TalkEventState.Finish;
                int frameSize = (pos.Width - FrameOff*2)/evt.ParamList.Count;
                result = evt.ChooseTarget(rollItemX/frameSize);

                if (BlessManager.RollFailSubHealth > 0 && evt.ParamList[rollItemX/frameSize].Contains("失败"))
                {
                    var healthSub = GameResourceBook.OutHealthSceneQuest(BlessManager.RollFailSubHealth*100);
                    if (healthSub > 0)
                        UserProfile.Profile.InfoBasic.SubHealth(healthSub);
                } 
                if (BlessManager.RollWinAddGold > 0 && evt.ParamList[rollItemX / frameSize].Contains("成功"))
                {
                    var goldAdd = GameResourceBook.InGoldSceneQuest(level, BlessManager.RollWinAddGold * 100);
                    if (goldAdd > 0)
                        UserProfile.Profile.InfoBag.AddResource(GameResourceType.Gold, goldAdd);
                }
            }
        }

        public override void Draw(Graphics g)
        {
           // g.DrawRectangle(Pens.White, pos);

            Font font = new Font("宋体", 11 * 1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
            g.DrawString("请等待", font, Brushes.White, pos.X + 3, pos.Y + 3);
            font.Dispose();

            g.DrawLine(Pens.Wheat, pos.X + 3, pos.Y + 3 + 20, pos.X + 3 + 400, pos.Y + 3 + 20);

            int frameSize = (pos.Width - FrameOff * 2) / evt.ParamList.Count;
            font = new Font("宋体", 11 * 1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
            for (int i = 0; i < evt.ParamList.Count; i++)
            {
                Brush b;
                switch (evt.ParamList[i])
                {
                    case "成功": b = Brushes.Lime;break;
                    case "大成功": b = Brushes.Green; break;
                    case "失败": b = Brushes.Orange; break;
                    case "大失败": b = Brushes.Red; break;
                    default: b = Brushes.Wheat; break;
                }
                g.FillRectangle(b, pos.X + i * frameSize + FrameOff, pos.Y + 25 + 30, frameSize - 2, 5);
                g.DrawString(evt.ParamList[i], font, b, pos.X + i * frameSize + FrameOff + frameSize / 2 - 20, pos.Y + 25 + 10);
            }
            g.FillEllipse(Brushes.Yellow, new Rectangle(pos.X + rollItemX + FrameOff - 6, pos.Y + 25 + 40, 12, 12));
            g.FillEllipse(Brushes.OrangeRed, new Rectangle(pos.X + rollItemX + FrameOff - 1, pos.Y + 25 + 40 + 5, 3, 3));
            font.Dispose();
        }
    }
}
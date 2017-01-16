﻿using System.Drawing;
using System.Windows.Forms;
using NarlonLib.Control;
using NarlonLib.Drawing;
using TaleofMonsters.DataType;
using TaleofMonsters.DataType.Drops;
using TaleofMonsters.DataType.Items;
using TaleofMonsters.DataType.Others;
using TaleofMonsters.DataType.User;
using TaleofMonsters.Forms.Items.Regions;
using TaleofMonsters.MainItem.Quests.SceneQuests;

namespace TaleofMonsters.MainItem.Quests
{
    internal class TalkEventItemReward : TalkEventItem
    {
        private Control parent;
        private VirtualRegion vRegion; 
        private ImageToolTip tooltip = MainItem.SystemToolTip.Instance;

        public TalkEventItemReward(int evtId, int level, Control c, Rectangle r, SceneQuestEvent e)
            : base(evtId, level, r, e)
        {
            parent = c;
            vRegion = new VirtualRegion(parent);
            vRegion.RegionEntered += virtualRegion_RegionEntered;
            vRegion.RegionLeft += virtualRegion_RegionLeft;

            DoReward();
        }

        private void DoReward()
        {
            int index = 1;
            if (IsBonusAvail("gold"))
            {
                var goldGet = GameResourceBook.InGoldSceneQuest(level, config.RewardGold);
                if (goldGet > 0)
                {
                    UserProfile.Profile.InfoBag.AddResource(GameResourceType.Gold, goldGet);
                    var pictureRegion = ComplexRegion.GetSceneDataRegion(index,
                        new Point(pos.X + 3 + 20 + (index - 1)*70, pos.Y + 3 + 25), 60, ImageRegionCellType.Gold,
                        (int) goldGet);
                    vRegion.AddRegion(pictureRegion);
                    index++;
                }
            }
            if (IsBonusAvail("food"))
            {
                var foodGet = GameResourceBook.InFoodSceneQuest(config.RewardFood);
                if (foodGet > 0)
                {
                    UserProfile.Profile.InfoBasic.AddFood(foodGet);
                    var pictureRegion = ComplexRegion.GetSceneDataRegion(index,
                        new Point(pos.X + 3 + 20 + (index - 1)*70, pos.Y + 3 + 25), 60, ImageRegionCellType.Food,
                        (int) foodGet);
                    vRegion.AddRegion(pictureRegion);
                    index++;
                }
            }
            if (IsBonusAvail("health"))
            {
                var healthGet = GameResourceBook.InHealthSceneQuest(config.RewardHealth);
                if (healthGet > 0)
                {
                    UserProfile.Profile.InfoBasic.AddHealth(healthGet);
                    var pictureRegion = ComplexRegion.GetSceneDataRegion(index,
                        new Point(pos.X + 3 + 20 + (index - 1)*70, pos.Y + 3 + 25), 60, ImageRegionCellType.Health,
                        (int) healthGet);
                    vRegion.AddRegion(pictureRegion);
                    index++;
                }
            }
            if (IsBonusAvail("mental"))
            {
                var mentalGet = GameResourceBook.InMentalSceneQuest(config.RewardMental);
                if (mentalGet > 0)
                {
                    UserProfile.Profile.InfoBasic.AddMental(mentalGet);
                    var pictureRegion = ComplexRegion.GetSceneDataRegion(index,
                        new Point(pos.X + 3 + 20 + (index - 1)*70, pos.Y + 3 + 25), 60, ImageRegionCellType.Mental,
                        (int) mentalGet);
                    vRegion.AddRegion(pictureRegion);
                    index++;
                }
            }
            if (IsBonusAvail("exp"))
            {
                var expGet = GameResourceBook.InExpSceneQuest(level, config.RewardExp);
                if (expGet > 0)
                {
                    UserProfile.Profile.InfoBasic.AddExp((int) expGet);
                    var pictureRegion = ComplexRegion.GetSceneDataRegion(index,
                        new Point(pos.X + 3 + 20 + (index - 1)*70, pos.Y + 3 + 25), 60, ImageRegionCellType.Exp,
                        (int) expGet);
                    vRegion.AddRegion(pictureRegion);
                    index++;
                }
            }
            if (IsBonusAvail("item"))
            {
                if (config.RewardItem > 0)
                {
                    UserProfile.InfoBag.AddItem(config.RewardItem, 1);
                    vRegion.AddRegion(new PictureRegion(index, pos.X + 3 + 20 + (index - 1)*70, pos.Y + 3 + 25, 60, 60,
                        PictureRegionCellType.Item, config.RewardItem));
                    index++;
                }
                if (config.RewardDrop > 0)
                {
                    var itemList = DropBook.GetDropItemList(config.RewardDrop);
                    foreach (var itemId in itemList)
                    {
                        UserProfile.InfoBag.AddItem(itemId, 1);
                        vRegion.AddRegion(new PictureRegion(index, pos.X + 3 + 20 + (index - 1) * 70, pos.Y + 3 + 25, 60, 60,
                            PictureRegionCellType.Item, itemId));
                        index++;
                    }
                }
            }
        }

        private bool IsBonusAvail(string tp)
        {
            if (evt.ParamList.Count == 0)
            {
                return true;
            }

            foreach (var item in evt.ParamList)
            {
                if (item == tp)
                {
                    return true;
                }
            }
            return false;
        }

        private void virtualRegion_RegionEntered(int id, int x, int y, int key)
        {
            {
                var region = vRegion.GetRegion(id) as PictureRegion;
                if (region != null)
                {
                    var regionType = region.GetVType();
                    if (regionType == PictureRegionCellType.Item)
                    {
                        Image image = HItemBook.GetPreview(key);
                        tooltip.Show(image, parent, x, y);
                    }
                }
            }
            {
                var region = vRegion.GetRegion(id) as ImageRegion;
                if (region != null)
                {
                    var regionType = region.GetVType();
                    if (regionType == ImageRegionCellType.Gold)
                    {
                        string resStr = string.Format("黄金:{0}", region.Parm);
                        Image image = DrawTool.GetImageByString(resStr, 100);
                        tooltip.Show(image, parent, x, y);
                    }
                    else if (regionType == ImageRegionCellType.Food)
                    {
                        string resStr = string.Format("食物:{0}", region.Parm);
                        Image image = DrawTool.GetImageByString(resStr, 100);
                        tooltip.Show(image, parent, x, y);
                    }
                    else if (regionType == ImageRegionCellType.Health)
                    {
                        string resStr = string.Format("生命:{0}", region.Parm);
                        Image image = DrawTool.GetImageByString(resStr, 100);
                        tooltip.Show(image, parent, x, y);
                    }
                    else if (regionType == ImageRegionCellType.Mental)
                    {
                        string resStr = string.Format("精神:{0}", region.Parm);
                        Image image = DrawTool.GetImageByString(resStr, 100);
                        tooltip.Show(image, parent, x, y);
                    }
                    else if (regionType == ImageRegionCellType.Exp)
                    {
                        string resStr = string.Format("经验值:{0}", region.Parm);
                        Image image = DrawTool.GetImageByString(resStr, 100);
                        tooltip.Show(image, parent, x, y);
                    }
                }
            }
        }

        private void virtualRegion_RegionLeft()
        {
            tooltip.Hide(parent);
        }

        public override void OnFrame(int tick)
        {
            RunningState = TalkEventState.Finish;
        }
        public override void Draw(Graphics g)
        {
           // g.DrawRectangle(Pens.White, pos);

            Font font = new Font("宋体", 11 * 1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
            g.DrawString("奖励", font, Brushes.White, pos.X + 3, pos.Y + 3);
            font.Dispose();

            g.DrawLine(Pens.Wheat, pos.X + 3, pos.Y + 3 + 20, pos.X + 3+400, pos.Y + 3 + 20);

            vRegion.Draw(g);
        }
    }
}

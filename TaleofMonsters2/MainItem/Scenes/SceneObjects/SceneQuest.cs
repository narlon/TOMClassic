using System.Drawing;
using ConfigDatas;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.Core;
using TaleofMonsters.DataType.Scenes;
using TaleofMonsters.Forms;

namespace TaleofMonsters.MainItem.Scenes.SceneObjects
{ 
    internal class SceneQuest : SceneObject
    {
        #region ί��
        delegate void CloseFormCallback(NpcTalkForm f);
        private void WiseOpen(NpcTalkForm f)
        {
            if (MainForm.Instance.InvokeRequired)
            {
                CloseFormCallback d = MainForm.Instance.DealPanel;
                MainForm.Instance.Invoke(d, new object[] {f });
            }
            else
            {
                MainForm.Instance.DealPanel(f);
            }
        }
        #endregion

        public int EventId { get; private set; }
        public SceneQuest(int wid, int wx, int wy, int wwidth, int wheight, int info)
            :base(wid,wx,wy,wwidth,wheight)
        {
            EventId = info;
        }

        public override bool OnClick()
        {
            if (!base.OnClick())
            {
                return false;
            }
            
            return true;
        }

        public override void MoveEnd()
        {
            base.MoveEnd();

            if (!Disabled)
            {
                BeginEvent();

                var config = ConfigData.GetSceneQuestConfig(EventId);
                if (!config.TriggerMulti)
                {
                    SetEnable(false);
                }
            }
        }

        private void BeginEvent()
        {
            if (EventId == 0)
            {
                return;
            }

            NpcTalkForm sw = new NpcTalkForm {EventId = EventId};
            WiseOpen(sw);
        }

        public override void Draw(Graphics g, int target)
        {
            base.Draw(g, target);

            if (Disabled && EventId == 0)
            {
                return;
            }

            Image markQuest = PicLoader.Read("Map", MapSetting ? "SymEvent.PNG": "SymQuest.PNG");
            int drawWidth = markQuest.Width * Width / GameConstants.SceneTileStandardWidth;
            int drawHeight = markQuest.Height * Height / GameConstants.SceneTileStandardHeight;
            var destRect = new Rectangle(X - drawWidth / 2 + Width / 8, Y - drawHeight / 2, drawWidth, drawHeight);
            if (Disabled)
            {
                g.DrawImage(markQuest, destRect, 0, 0, markQuest.Width, markQuest.Height, GraphicsUnit.Pixel, HSImageAttributes.ToGray);
                var config = ConfigData.GetSceneQuestConfig(EventId);
                g.DrawImage(SceneBook.GetSceneQuestImage(config.Id), new Rectangle(X, Y - Width / 2 + Height / 2, Width / 2, Width / 2), 0, 0, 180, 180, GraphicsUnit.Pixel, HSImageAttributes.ToGray);
            }
            else
            {
                g.DrawImage(markQuest, destRect, 0, 0, markQuest.Width, markQuest.Height, GraphicsUnit.Pixel);
                if (MapSetting)
                {
                    var config = ConfigData.GetSceneQuestConfig(EventId);
                    g.DrawImage(SceneBook.GetSceneQuestImage(config.Id), new Rectangle(X, Y - Width / 2 + Height / 2, Width / 2, Width / 2), 0, 0, 180, 180, GraphicsUnit.Pixel);
                }
            }

            markQuest.Dispose();
        }
    }
}


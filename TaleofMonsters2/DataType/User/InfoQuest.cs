using System.Collections.Generic;
using ConfigDatas;
using NarlonLib.Log;
using TaleofMonsters.Core;
using TaleofMonsters.DataType.User.Db;
using TaleofMonsters.MainItem;
using TaleofMonsters.MainItem.Scenes;

namespace TaleofMonsters.DataType.User
{
    public class InfoQuest
    {
        [FieldIndex(Index = 1)]
        public List<DbQuestData> QuestRunning; //目前进行到的任务id
        [FieldIndex(Index = 2)]
        public List<int> QuestFinish;

        public InfoQuest()
        {
            QuestRunning = new List<DbQuestData>();
            QuestFinish = new List<int>();
        }

        public bool IsQuestNotReceive(int qid)
        {
            var questRun = QuestRunning.Find(q => q.QuestId == qid);
            var questFin = QuestFinish.Find(q => q == qid);
            return questFin <= 0 && questRun == null;
        }

        public bool IsQuestCanReceive(int qid)
        {
            if (QuestFinish.Contains(qid))
            {
                return false;
            }
            if (QuestRunning.Find(q => q.QuestId == qid) != null)
            {
                return false;
            }
            var questConfig = ConfigData.GetQuestConfig(qid);
            if (questConfig.Former > 0 && !IsQuestFinish(questConfig.Former))
                return false;
            return true;
        }

        public bool IsQuestCanProgress(int qid)
        {
            var questData = QuestRunning.Find(q => q.QuestId == qid);
            if (questData != null)
            {
                return questData.State == (int)QuestStates.Receive && questData.Progress < 10;
            }
            return false;
        }

        public int GetQuestProgress(int qid)
        {
            var questData = QuestRunning.Find(q => q.QuestId == qid);
            if (questData != null && questData.State == (int)QuestStates.Receive)
            {
                return questData.Progress;
            }
            return 0;
        }

        public bool IsQuestCanReward(int qid)
        {
            var questData = QuestRunning.Find(q => q.QuestId == qid);
            if (questData != null)
            {
                return questData.State == (int)QuestStates.Accomplish;
            }
            return false;
        }

        public bool IsQuestFinish(int qid)
        {
            return QuestFinish.Contains(qid);
        }

        public void SetQuestState(int qid, QuestStates state)
        {
            if (qid <= 0)
            {
                NLog.Warn("SetQuestState state qid==0");
                return;
            }

            var questRun = QuestRunning.Find(q => q.QuestId == qid);
            var questFin = QuestFinish.Find(q => q == qid);
            if (state == QuestStates.Receive)
            {
                if (questRun == null && questFin <= 0)
                {
                    var questData = new DbQuestData
                    {
                        QuestId = qid,
                        State = (byte) QuestStates.Receive
                    };
                    QuestRunning.Add(questData);

                    OnReceiveQuest(qid);
                }
            }
            else if (state == QuestStates.Finish)
            {
                if (questRun != null && questRun.State == (byte)QuestStates.Accomplish && questFin == 0)
                {
                    QuestRunning.Remove(questRun);
                    QuestFinish.Add(qid);

                    OnFinishQuest(qid);
                }
            }
            else
            {
                if (questRun != null && questRun.State < (byte)state)
                {
                    questRun.State = (byte) state;
                }
            }
        }

        public void AddQuestProgress(int qid, byte progress)
        {
            var questRun = QuestRunning.Find(q => q.QuestId == qid);
            if (questRun.State == (int)QuestStates.Receive)
            {
                questRun.Progress += progress;
                var questConfig = ConfigData.GetQuestConfig(qid);
                if (questRun.Progress >= 10)
                {
                    questRun.Progress = 10;
                    questRun.State = (int) QuestStates.Accomplish;
                    MainTipManager.AddTip(string.Format("任务达成-{0}", questConfig.Name), "White");
                }
                else
                {
                    MainTipManager.AddTip(string.Format("任务进度-{0} {1}/10", questConfig.Name, questRun.Progress), "White");
                }
            }
        }

        private void OnReceiveQuest(int qid)
        {
            var questConfig = ConfigData.GetQuestConfig(qid);
            if (!string.IsNullOrEmpty(questConfig.TriggerSceneQuest))
            {
                Scene.Instance.QuestNext(questConfig.TriggerSceneQuest);
            }

            MainTipManager.AddTip(string.Format("接受任务-{0}", questConfig.Name), "White");
            SoundManager.Play("System", "QuestActivateWhat1.wav");
        }

        private void OnFinishQuest(int qid)
        {
            var questConfig = ConfigData.GetQuestConfig(qid);
            MainTipManager.AddTip(string.Format("完成任务-{0}", questConfig.Name), "White");
            SoundManager.Play("System", "QuestCompleted.wav");
        }

        public void OnSceneQuestSuccess(string questName, bool partial)
        {
            if (partial)
            {
                return;
            }

            foreach (var runQuest in QuestRunning)
            {
                var config = ConfigData.GetQuestConfig(runQuest.QuestId);
                if (IsQuestCanProgress(runQuest.QuestId) && config.SuccessSceneQuest == questName)
                {
                    AddQuestProgress(runQuest.QuestId, (byte) config.ProgressAdd);
                }
            }
        }

        public void OnSwitchScene(bool isWarp)
        {
            if (isWarp)
                ResetQuest();
        }

        public void OnLogout()
        {
          //  ResetQuest();
        }

        private void ResetQuest()
        {
            var resetList = new List<int>();
            foreach (var dbQuestData in QuestRunning)
            {
                var questConfig = ConfigData.GetQuestConfig(dbQuestData.QuestId);
                if (questConfig.ResetOnLeave)
                {
                    resetList.Add(questConfig.Id);
                }
            }
            foreach (var questId in resetList)
            {
                QuestRunning.RemoveAll(quest => questId == quest.QuestId);
            }
        }
    }
}

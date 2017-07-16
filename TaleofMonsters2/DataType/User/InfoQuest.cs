using System.Collections.Generic;
using TaleofMonsters.Core;

namespace TaleofMonsters.DataType.User
{
    public class InfoQuest
    {
        [FieldIndex(Index = 1)]
        public int QuestId; //Ŀǰ���е�������id

        public InfoQuest()
        {
        }

        public bool IsQuestFinish(int qid)
        {
            return qid <= QuestId;
        }

        public void SetQuest(int qid)
        {
            if (qid <= QuestId)
            {
                return;
            }
            QuestId = qid;
        }
    }
}

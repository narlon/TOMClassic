﻿using System;
using System.Collections.Generic;
using ConfigDatas;
using NarlonLib.Log;
using TaleofMonsters.Core;
using TaleofMonsters.DataType.User.Db;

namespace TaleofMonsters.DataType.User
{
    public class InfoDungeon
    {
        [FieldIndex(Index = 1)] public int DungeonId; //副本id

        [FieldIndex(Index = 2)] public int Str; //力量
        [FieldIndex(Index = 3)] public int Agi; //敏捷
        [FieldIndex(Index = 4)] public int Intl; //智慧
        [FieldIndex(Index = 5)] public int Perc; //感知
        [FieldIndex(Index = 6)] public int Endu; //耐力

        [FieldIndex(Index = 7)] public List<DbGismoState> EventList; //完成的任务列表，如果0，表示任务失败
        [FieldIndex(Index = 11)] public int FightWin;
        [FieldIndex(Index = 12)] public int FightLoss;

        [FieldIndex(Index = 22)] public int StrAddon; //力量改变值
        [FieldIndex(Index = 23)] public int AgiAddon; //敏捷改变值
        [FieldIndex(Index = 24)] public int IntlAddon; //智慧改变值
        [FieldIndex(Index = 25)] public int PercAddon; //感知改变值
        [FieldIndex(Index = 26)] public int EnduAddon; //耐力改变值

        public InfoDungeon()
        {
            EventList = new List<DbGismoState>();
        }

        public void Enter(int dungeonId) //进入副本需要初始化
        {
            DungeonId = dungeonId;

            EventList = new List<DbGismoState>();
            FightWin = 0;
            FightLoss = 0;

            StrAddon = 0;
            AgiAddon = 0;
            IntlAddon = 0;
            PercAddon = 0;
            EnduAddon = 0;

            RecalculateAttr();
        }

        public void Leave()
        {
            DungeonId = 0;
        }

        public int Step { get { return EventList.Count; } }

        public void OnEventEnd(int id, string type)
        {
            if (DungeonId > 0)
            {
                EventList.Add(new DbGismoState(id, type));
                UserProfile.InfoGismo.CheckEventList();
                NLog.Debug("OnEventEnd " +id.ToString() + " " + type);
            }
        }

        public bool CheckQuestCount(int qid, string state, int countNeed, bool needContinue)
        {
            if (EventList.Count == 0)
                return false;

            if (EventList[EventList.Count - 1].BaseId != qid)
                return false;

            int count = 0;
            for (int i = EventList.Count-1; i >=0 ; i--)
            {
                var checkData = EventList[i];
                if (checkData.BaseId == qid)
                {
                    if (state != "" && state != checkData.ResultName)
                        break;

                    count++;
                }
                else
                {
                    if(needContinue)
                        break;
                }
            }

            return count >= countNeed;
        }

        public void ChangeAttr(int strC, int agiC, int intlC, int percC, int enduC)
        {
            if (strC != 0)
                StrAddon += strC;
            if (agiC != 0)
                AgiAddon += agiC;
            if (intlC != 0)
                IntlAddon += intlC;
            if (percC != 0)
                PercAddon += percC;
            if (enduC != 0)
                EnduAddon += enduC;

            RecalculateAttr();
        }

        public void RecalculateAttr()
        {
            if (DungeonId <= 0)
                return;

            var dungeonConfig = ConfigData.GetDungeonConfig(DungeonId);
            Str = dungeonConfig.Str;
            Agi = dungeonConfig.Agi;
            Intl = dungeonConfig.Intl;
            Perc = dungeonConfig.Perc;
            Endu = dungeonConfig.Endu;

            Str += StrAddon; //加成属性，一般来自sq
            Agi += AgiAddon;
            Intl += IntlAddon;
            Perc += PercAddon;
            Endu += EnduAddon;

            foreach (var dbEquip in UserProfile.InfoEquip.Equipon) //装备的属性修改
            {
                if (dbEquip.BaseId > 0)
                {
                    var equipConfig = ConfigData.GetEquipConfig(dbEquip.BaseId);
                    if (equipConfig.DungeonAttrs != null && equipConfig.DungeonAttrs.Length > 0)
                    {
                        if (dungeonConfig.Str >= 0)
                            Str += equipConfig.DungeonAttrs[0];
                        if (dungeonConfig.Agi >= 0)
                            Agi += equipConfig.DungeonAttrs[1];
                        if (dungeonConfig.Intl >= 0)
                            Intl += equipConfig.DungeonAttrs[2];
                        if (dungeonConfig.Perc >= 0)
                            Perc += equipConfig.DungeonAttrs[3];
                        if (dungeonConfig.Endu >= 0)
                            Endu += equipConfig.DungeonAttrs[4];
                    }
                }
            }

            if (dungeonConfig.Str >= 0)
                Str = Math.Max(1, Str);
            if (dungeonConfig.Agi >= 0)
                Agi = Math.Max(1, Agi);
            if (dungeonConfig.Intl >= 0)
                Intl = Math.Max(1, Intl);
            if (dungeonConfig.Perc >= 0)
                Perc = Math.Max(1, Perc);
            if (dungeonConfig.Endu >= 0)
                Endu = Math.Max(1, Endu);
        }

        public int GetAttrByStr(string type)
        {
            switch (type)
            {
                case "str": return Str;
                case "agi": return Agi;
                case "intl": return Intl;
                case "perc": return Perc;
                case "endu": return Endu;
            }
            return -1;
        }

        public int GetRequireAttrByStr(string type, int biasData)
        {
            var dungeonConfig = ConfigData.GetDungeonConfig(DungeonId);
            switch (type)
            {
                case "str": return dungeonConfig.Str + biasData;
                case "agi": return dungeonConfig.Agi + biasData;
                case "intl": return dungeonConfig.Intl + biasData;
                case "perc": return dungeonConfig.Perc + biasData;
                case "endu": return dungeonConfig.Endu + biasData;
            }
            return 1;
        }
    }
}

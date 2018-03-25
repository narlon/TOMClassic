﻿using System;
using System.Collections.Generic;
using ConfigDatas;
using TaleofMonsters.Core;
using TaleofMonsters.Datas.Equips;
using TaleofMonsters.Datas.Others;
using TaleofMonsters.Datas.User.Db;
using TaleofMonsters.Forms.CMain;

namespace TaleofMonsters.Datas.User
{
    public class InfoEquip
    {
        [FieldIndex(Index = 6)] public DbEquip[] Equipon;
        [FieldIndex(Index = 7)] public List<DbEquip> EquipAvail;

        private const int MainHouseIndex = 5;

        public InfoEquip()
        {
            Equipon = new DbEquip[GameConstants.EquipOnCount+1];
            EquipAvail = new List<DbEquip>();
            for (int i = 0; i < Equipon.Length; i++)
                Equipon[i] = new DbEquip();
        }

        public void AddEquip(int eid)
        {
            EquipConfig equipConfig = ConfigData.GetEquipConfig(eid);

            var equip = EquipAvail.Find(edata => edata.BaseId == eid);
            if (equip != null)
            {
                //todo 升级
            }
            else
            {
                EquipAvail.Add(new DbEquip { BaseId = eid, Level = 1 });
                MainTipManager.AddTip(string.Format("|获得装备-|{0}|{1}", HSTypes.I2QualityColor(equipConfig.Quality), equipConfig.Name), "White");
                UserProfile.InfoRecord.AddRecordById((int)MemPlayerRecordTypes.EquipGet, 1);
            }
        }

        public bool CanEquip(int equipId, int slotId)
        {
            //先判定slot可用性
            EquipSlotConfig slotConfig = ConfigData.GetEquipSlotConfig(slotId);
            if (slotId != MainHouseIndex)//主楼格永远可以装备
            {
                if (Equipon[MainHouseIndex].BaseId == 0)
                    return false;
                EquipConfig equipConfig = ConfigData.GetEquipConfig(Equipon[MainHouseIndex].BaseId);
                if (equipConfig.SlotId != null && Array.IndexOf(equipConfig.SlotId, slotId) < 0)
                    return false;
            }

            if (equipId > 0)
            {
                EquipConfig equipConfig = ConfigData.GetEquipConfig(equipId);
                return equipConfig.Position == slotConfig.Type;
            }
            return true;
        }

        public void DoEquip(int equipPos, int equipId)
        {
            if (equipPos == MainHouseIndex) //如果主楼，移除所有其他建筑
            {
                for (int i = 0; i < Equipon.Length; i++)
                    Equipon[i] = new DbEquip();
            }
            UserProfile.InfoEquip.Equipon[equipPos] = GetEquipById(equipId);
            UserProfile.InfoDungeon.RecalculateAttr(); //会影响力量啥的属性
        }

        public List<Equip> GetValidEquipsList()
        {
            List<Equip> equips = new List<Equip>();

            for (int i = 0; i < GameConstants.EquipOnCount; i++)
            {
                var equip = GetEquipOn(i+1);
                if (equip.BaseId == 0)
                    continue;

                if (CanEquip(equip.BaseId, i + 1))
                {
                    var equipD = new Equip(equip.BaseId);
                    if (equip.Level > 1)
                        equipD.UpgradeToLevel(equip.Level);
                    equips.Add(equipD);
                }
            }
            return equips;
        }

        public bool HasEquip(int id)
        {
            return EquipAvail.Find(eq => eq.BaseId == id) != null;
        }
        public bool HasEquipOn(int id)
        {
            return Array.Find(Equipon, eq => eq.BaseId == id) != null;
        }
        public DbEquip GetEquipById(int id)
        {
            return EquipAvail.Find(eq => eq.BaseId == id);
        }
        public DbEquip GetEquipOn(int id)
        {
            return Equipon[id];
        }
        public List<DbEquip> GetEquipList(int type)
        {
            var list = new List<DbEquip>();
            foreach (var dbEquip in EquipAvail)
            {
                var equipConfig = ConfigData.GetEquipConfig(dbEquip.BaseId);
                if(equipConfig.Position == type)
                    list.Add(dbEquip);
            }
            return list;
        }

        public void AddExp(int id, int exp)
        {
            var equip = GetEquipById(id);
            if (equip != null)
            {
                equip.Exp += exp;
                var expNeed = ExpTree.GetNextRequiredEquip(equip.Level);
                if (equip.Exp >= expNeed)
                {
                    equip.Exp -= expNeed;
                    equip.Level++;
                }

                var equipOn = GetEquipOn(id);
                equipOn.Exp += exp;
                if (equipOn.Exp >= expNeed)
                {
                    equipOn.Exp -= expNeed;
                    equipOn.Level++;
                }
            }
        }
    }
}

﻿using CodeInject.MemoryTools;
using CodeInject.Party;
using CodeInject.PickupFilters;
using CodeInject.WebServ.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace CodeInject.Actors
{
    public unsafe class Player : IObject, IPlayer
    {
        #region Properties
        public long ObjectPointer { get; set; }
        public ushort ID { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public int MaxHp { get; set; }
        public int Hp { get; set; }
        public int MaxMp { get; set; }
        public int Mp { get; set; }
        public string Name { get; set; } = "";
        public short BuffCount { get; set; }
        #endregion

        public Player(long* Entry)
        {
            ObjectPointer = (long)(long*)*Entry;

            X = *(float*)(*Entry + 0x10);
            Y = *(float*)(*Entry + 0x14);
            Z = *(float*)(*Entry + 0x18);
            ID = *(ushort*)(*((long*)(*Entry + 0x20)));
            Hp = *(int*)(*Entry + 0x3980);
            MaxHp = *(int*)(*Entry + 0x3B2C);
            Mp = *(int*)(*Entry + 0x3984);
            MaxMp = *(int*)(*Entry + 0x3B38);
            BuffCount = *(short*)(*Entry + 0x648);
            Name = Marshal.PtrToStringAnsi(new IntPtr((*Entry + 0x9A8)));
        }

        #region Actions
        public void CastSkill(IObject target, int skillIndex)
        {
            GameHackFunc.Game.Actions.CastSpell(target,skillIndex);
        }
        public void CastSkill(int skillIndex)
        {
            GameHackFunc.Game.Actions.CastSpell(skillIndex);
        }
        public void CastSkill(IObject target)
        {
            GameHackFunc.Game.Actions.Attack(target.ID);
        }
        public void UseItem(InvItem item)
        {
            item.UseItem();
        }
        public void WalkToPoint(Vector2 position)
        {
            GameHackFunc.Game.Actions.MoveToPoint(position);
        }
        #endregion
        #region CharacterData
        public static Player GetPlayer
        {
            get
            {
                return GameHackFunc.Game.ClientData.GetPlayer();
            }
        }
        public List<ushort> GetBuffsIdList()
        {

            List<ushort> list = new List<ushort>();

            long baseBuffAddres = ObjectPointer + 0x410 + 0x230;

            long* currentBuff = (long*)(*((long*)baseBuffAddres));
            while (*currentBuff != *(long*)baseBuffAddres)
            {
                long* detailsPointer = (long*)(*currentBuff + 0x18);

                ushort* buffID = (ushort*)(*detailsPointer + 0x18);
                list.Add(*buffID);
                //  MessageBox.Show((*buffID).ToString("X"));
                currentBuff = (long*)*currentBuff;
            }

            return list;
        }
        public List<Skills> GetSkillsList()
        {
            return GameHackFunc.Game.ClientData.GetPlayerSkills();
        }
        public List<InvItem> GetInventoryItemsList(List<InvItem> currentList =null)
        {
            return GameHackFunc.Game.ClientData.GetAllItemsFromInventory(currentList);
        }
        public List<InvItem> GetConsumableItemsFromInventory(List<InvItem> currentList = null)
        {
            return GameHackFunc.Game.ClientData.GetConsumableItemsFromInventory(currentList);
        }
        public List<PartyMember> GetPartyMembersList()
        {
            return GameHackFunc.Game.ClientData.GetPartyMembersList();
        }
        public List<IObject> GetAllItemsAroundPlayerList()
        {
            return GameHackFunc.Game.ClientData.GetItemsAroundPlayerV2();
        }
        public List<IObject> GetItemsAroundPlayerFilteredList(IFilter filter)
        {
            return GetAllItemsAroundPlayerList().Where(x => filter.CanPickup(x) && x.CalcDistance(this) < 20).OrderBy(x => x.CalcDistance(this)).ToList();
        }
        #endregion


        public double CalcDistance(IObject targerObject)
        {
            return Math.Sqrt(Math.Pow((targerObject.X / 100) - (this.X / 100), 2) + Math.Pow((targerObject.Y / 100) - (this.Y / 100), 2) + Math.Pow((targerObject.Z / 100) - (this.Z / 100), 2));
        }
        public double CalcDistance(float x, float y, float z)
        {
            return Math.Sqrt(
                  Math.Pow((x / 100) - (this.X / 100), 2)
                + Math.Pow((y / 100) - (this.Y / 100), 2)
                + Math.Pow((z / 100) - (this.Z / 100), 2));
        }
        public override string ToString()
        {
            return $"[{(ID).ToString("X")}] {ObjectPointer.ToString("X")} {Name} {BuffCount}";
        }
        public string ToWSObject()
        {
            string playerJson =
                JsonConvert.SerializeObject(new PlayerInfoModel()
                {
                    Hp = Hp,
                    MaxHp = MaxHp,
                    Mp = Mp,
                    MaxMp = MaxMp,
                    X = X,
                    Y = Y,
                    Z = Z,
                    BuffCount = BuffCount,
                    Name = Name
                }, Formatting.Indented);

            return playerJson;
        }
    }
}

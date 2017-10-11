using Destiny.Maple.Characters;
using Destiny.Network;
using Destiny.Data;
using Destiny.Maple.Shops;
using System.Collections.Generic;
using Destiny.Maple.Scripting;
using System;

namespace Destiny.Maple.Life
{
    public class Npc : LifeObject, ISpawnable, IControllable
    {
        public Npc(Datum datum)
            : base(datum)
        {
            this.Scripts = new Dictionary<Character, NpcScript>();
        }

        public Character Controller { get; set; }

        public Shop Shop { get; set; }
        public int StorageCost { get; set; }
        public Dictionary<Character, NpcScript> Scripts { get; private set; }

        public void Move(Packet iPacket)
        {
            byte action1 = iPacket.ReadByte();
            byte action2 = iPacket.ReadByte();

            Movements movements = null;

            if (iPacket.Remaining > 0)
            {
                movements = Movements.Decode(iPacket);
            }

            using (Packet oPacket = new Packet(ServerOperationCode.NpcMove))
            {
                oPacket
                    .WriteInt(this.ObjectID)
                    .WriteByte(action1)
                    .WriteByte(action2);

                if (movements != null)
                {
                    oPacket.WriteBytes(movements.ToByteArray());
                }

                this.Map.Broadcast(oPacket);
            }
        }

        public void Converse(Character talker)
        {
            if (this.Shop != null)
            {
                this.Shop.Show(talker);
            }
            else if (this.StorageCost > 0)
            {
                talker.Storage.Show(this);
            }
            else
            {
                var script = new NpcScript(this, talker);

                this.Scripts[talker] = script;

                try
                {
                    script.Execute();
                }
                catch (Exception ex)
                {

                }
            }
        }

        public void Handle(Character talker, Packet iPacket)
        {
            if (talker.LastNpc == null)
            {
                return;
            }

            NpcMessageType lastMessageType = (NpcMessageType)iPacket.ReadByte();
            byte action = iPacket.ReadByte();

            // TODO: Validate last message type.

            int selection = -1;

            byte endTalkByte;

            switch (lastMessageType)
            {
                case NpcMessageType.RequestText:
                case NpcMessageType.RequestNumber:
                case NpcMessageType.RequestStyle:
                case NpcMessageType.Choice:
                    endTalkByte = 0;
                    break;

                default:
                    endTalkByte = byte.MaxValue;
                    break;
            }

            if (action != endTalkByte)
            {
                if (iPacket.Remaining >= 4)
                {
                    selection = iPacket.ReadInt();
                }
                else if (iPacket.Remaining > 0)
                {
                    selection = iPacket.ReadByte();
                }

                if (lastMessageType == NpcMessageType.RequestStyle)
                {
                    //selection = this.StyleSelectionHelpers[talker][selection];
                }

                if (selection != -1)
                {
                    this.Scripts[talker].SetResult(selection);
                }
                else
                {
                    this.Scripts[talker].SetResult(action);
                }
            }
            else
            {
                talker.LastNpc = null;
            }
        }

        public void AssignController()
        {
            if (this.Controller == null)
            {
                int leastControlled = int.MaxValue;
                Character newController = null;

                lock (this.Map.Characters)
                {
                    foreach (Character character in this.Map.Characters)
                    {
                        if (character.ControlledNpcs.Count < leastControlled)
                        {
                            leastControlled = character.ControlledNpcs.Count;
                            newController = character;
                        }
                    }
                }

                if (newController != null)
                {
                    newController.ControlledNpcs.Add(this);
                }
            }
        }

        public Packet GetCreatePacket()
        {
            return this.GetSpawnPacket();
        }

        public Packet GetSpawnPacket()
        {
            return this.GetInternalPacket(false);
        }

        public Packet GetControlRequestPacket()
        {
            return this.GetInternalPacket(true);
        }

        private Packet GetInternalPacket(bool requestControl)
        {
            Packet oPacket = new Packet(requestControl ? ServerOperationCode.NpcChangeController : ServerOperationCode.NpcEnterField);

            if (requestControl)
            {
                oPacket.WriteBool(true);
            }

            oPacket
                .WriteInt(this.ObjectID)
                .WriteInt(this.MapleID)
                .WriteShort(this.Position.X)
                .WriteShort(this.Position.Y)
                .WriteBool(!this.FacesLeft)
                .WriteShort(this.Foothold)
                .WriteShort(this.MinimumClickX)
                .WriteShort(this.MaximumClickX)
                .WriteBool(true); // NOTE: Hide.

            return oPacket;
        }

        public Packet GetControlCancelPacket()
        {
            Packet oPacket = new Packet(ServerOperationCode.NpcChangeController);

            oPacket
                .WriteBool(false)
                .WriteInt(this.ObjectID);

            return oPacket;
        }

        public Packet GetDialogPacket(string text, NpcMessageType messageType, params byte[] footer)
        {
            Packet oPacket = new Packet(ServerOperationCode.ScriptMessage);

            oPacket
                .WriteByte(4) // NOTE: Unknown.
                .WriteInt(this.MapleID)
                .WriteByte((byte)messageType)
                .WriteByte() // NOTE: Speaker.
                .WriteString(text)
                .WriteBytes(footer);

            return oPacket;
        }

        public Packet GetDestroyPacket()
        {
            Packet oPacket = new Packet(ServerOperationCode.NpcLeaveField);

            oPacket.WriteInt(this.ObjectID);

            return oPacket;
        }
    }
}

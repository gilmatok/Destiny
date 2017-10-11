using Destiny.Network;
using Destiny.Data;
using Destiny.Maple.Maps;
using System.Collections.Generic;
using Destiny.IO;
using Destiny.Maple.Data;

namespace Destiny.Maple.Characters
{
    public sealed class CharacterTrocks
    {
        public Character Parent { get; private set; }

        public List<int> Regular { get; private set; }
        public List<int> VIP { get; private set; }

        public CharacterTrocks(Character parent)
        {
            this.Parent = parent;

            this.Regular = new List<int>();
            this.VIP = new List<int>();
        }

        public void Load()
        {
            foreach (Datum datum in new Datums("trocks").Populate("CharacterID = {0}", this.Parent.ID))
            {
                byte index = (byte)datum["Index"];
                int map = (int)datum["Map"];

                if (index >= 5)
                {
                    this.VIP.Add(map);
                }
                else
                {
                    this.Regular.Add(map);
                }
            }
        }

        public void Save()
        {
            Database.Delete("trocks", "CharacterID = {0}", this.Parent.ID);

            byte index = 0;

            foreach (int map in this.Regular)
            {
                Datum datum = new Datum("trocks");

                datum["CharacterID"] = this.Parent.ID;
                datum["Index"] = index++;
                datum["Map"] = map;

                datum.Insert();
            }

            index = 5;

            foreach (int map in this.VIP)
            {
                Datum datum = new Datum("trocks");

                datum["CharacterID"] = this.Parent.ID;
                datum["Index"] = index++;
                datum["Map"] = map;

                datum.Insert();
            }
        }

        public bool Contains(int mapID)
        {
            foreach (int map in this.Regular)
            {
                if (map == mapID)
                {
                    return true;
                }
            }

            foreach (int map in this.VIP)
            {
                if (map == mapID)
                {
                    return true;
                }
            }

            return false;
        }

        public void Update(Packet iPacket)
        {
            TrockAction action = (TrockAction)iPacket.ReadByte();
            TrockType type = (TrockType)iPacket.ReadByte();

            switch (action)
            {
                case TrockAction.Remove:
                    {
                        int mapID = iPacket.ReadInt();

                        if (type == TrockType.Regular)
                        {
                            if (!this.Regular.Contains(mapID))
                            {
                                return;
                            }

                            this.Regular.Remove(mapID);
                        }
                        else if (type == TrockType.VIP)
                        {
                            if (!this.VIP.Contains(mapID))
                            {
                                return;
                            }

                            this.VIP.Remove(mapID);
                        }
                    }
                    break;

                case TrockAction.Add:
                    {
                        int mapID = this.Parent.Map.MapleID;

                        // TODO: Check if the map field limits allow trocks (e.g. Maple Island is forbidden).

                        if (true)
                        {
                            if (type == TrockType.Regular)
                            {
                                this.Regular.Add(mapID);
                            }
                            else if (type == TrockType.VIP)
                            {
                                this.VIP.Add(mapID);
                            }
                        }
                        else
                        {
                            return;
                        }
                    }
                    break;
            }

            using (Packet oPacket = new Packet(ServerOperationCode.MapTransferResult))
            {
                oPacket
                    .WriteByte((byte)(action == TrockAction.Remove ? 2 : 3))
                    .WriteByte((byte)type)
                    .WriteBytes(type == TrockType.Regular ? this.RegularToByteArray() : this.VIPToByteArray());

                this.Parent.Client.Send(oPacket);
            }
        }

        public bool Use(int itemID, Packet iPacket)
        {
            bool used = false;
            byte action = iPacket.ReadByte();

            TrockType type = itemID == 5040000 ? TrockType.Regular : TrockType.VIP;

            int destinationMapID = -1;
            TrockResult result = TrockResult.Success;

            if (action == 0) // NOTE: Preset map.
            {
                int mapID = iPacket.ReadInt();

                if (!this.Parent.Trocks.Contains(mapID))
                {
                    result = TrockResult.CannotGo;
                }

                destinationMapID = mapID;
            }
            else if (action == 1) // NOTE: IGN.
            {
                string targetName = iPacket.ReadString();

                Character target = null;// this.Parent.Client.Channel.Characters.GetCharacter(targetName);

                if (target == null)
                {
                    result = TrockResult.DifficultToLocate;
                }
                else
                {
                    destinationMapID = target.Map.MapleID;
                }
            }

            iPacket.ReadInt(); // NOTE: Ticks.

            if (destinationMapID != -1)
            {
                Map originMap = this.Parent.Map;
                Map destinationMap = DataProvider.Maps[destinationMapID];

                if (false) // TODO: Field limit check.
                {
                    result = TrockResult.CannotGo;
                }
                else if (false) // TODO: Origin map field limit check.
                {
                    result = TrockResult.CannotGo;
                }
                else if (originMap.MapleID == destinationMap.MapleID)
                {
                    result = TrockResult.AlreadyThere;
                }
                else if (false) // TODO: Continent check.
                {
                    result = TrockResult.CannotGo;
                }
            }
            
            if (result == TrockResult.Success)
            {
                this.Parent.ChangeMap(destinationMapID);

                used = true;
            }
            else
            {
                using (Packet oPacket = new Packet(ServerOperationCode.MapTransferResult))
                {
                    oPacket
                        .WriteByte((byte)result)
                        .WriteByte((byte)type);

                    this.Parent.Client.Send(oPacket);
                }
            }

            return used;
        }

        public byte[] RegularToByteArray()
        {
            using (ByteBuffer oPacket = new ByteBuffer())
            {
                int remaining = 1;

                while (remaining <= this.Regular.Count)
                {
                    oPacket.WriteInt(this.Regular[remaining - 1]);

                    remaining++;
                }

                while (remaining <= 5)
                {
                    oPacket.WriteInt(999999999);

                    remaining++;
                }

                oPacket.Flip();
                return oPacket.GetContent();
            }
        }

        public byte[] VIPToByteArray()
        {
            using (ByteBuffer oPacket = new ByteBuffer())
            {
                int remaining = 1;

                while (remaining <= this.VIP.Count)
                {
                    oPacket.WriteInt(this.VIP[remaining - 1]);

                    remaining++;
                }

                while (remaining <= 10)
                {
                    oPacket.WriteInt(999999999);

                    remaining++;
                }

                oPacket.Flip();
                return oPacket.GetContent();
            }
        }
    }
}

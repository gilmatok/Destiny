using Destiny.Core.IO;
using Destiny.Game.Characters;
using Destiny.Game.Data;
using Destiny.Server;
using System;

namespace Destiny.Game.Maps
{
    public sealed class Map
    {
        public int MapleID { get; private set; }
        public MapData Data { get; private set; }

        public MapCharacters Characters { get; private set; }
        public MapMobs Mobs { get; private set; }
        public MapNpcs Npcs { get; private set; }
        public MapPortals Portals { get; private set; }
        
        public Map(int mapleID)
        {
            this.MapleID = mapleID;
            this.Data = MasterServer.Instance.Data.Maps[this.MapleID];

            this.Characters = new MapCharacters(this);
            this.Mobs = new MapMobs(this);
            this.Npcs = new MapNpcs(this);
            this.Portals = new MapPortals(this);
        }

        public void Broadcast(byte[] buffer)
        {
            foreach (Character character in this.Characters)
            {
                character.Client.Send(buffer);
            }
        }

        public void Broadcast(byte[] buffer, Character ignored)
        {
            foreach (Character character in this.Characters)
            {
                if (character != ignored)
                {
                    character.Client.Send(buffer);
                }
            }
        }

        public void DecodeMovePath(IMoveable moveable, InPacket iPacket)
        {
            byte stance = 0;
            short foothold = 0;
            Point position = null;

            byte count = iPacket.ReadByte();

            while (count-- > 0)
            {
                byte type = iPacket.ReadByte();

                switch (type)
                {
                    case 0:
                    case 5:
                    case 17:
                        position = iPacket.ReadPoint();
                        iPacket.Skip(4);
                        iPacket.Skip(2);
                        stance = iPacket.ReadByte();
                        iPacket.Skip(2);
                        break;

                    case 1:
                    case 2:
                    case 6:
                    case 12:
                    case 13:
                    case 16:
                        iPacket.Skip(4);
                        stance = iPacket.ReadByte();
                        iPacket.Skip(2);
                        break;

                    case 3:
                    case 4:
                    case 7:
                    case 8:
                    case 9:
                    case 14:
                        position = iPacket.ReadPoint();
                        iPacket.Skip(4);
                        stance = iPacket.ReadByte();
                        break;

                    case 10:
                        iPacket.Skip(1);
                        break;

                    case 11:
                        break;

                    case 15:
                        position = iPacket.ReadPoint();
                        iPacket.Skip(4);
                        iPacket.Skip(2);
                        foothold = iPacket.ReadShort();
                        stance = iPacket.ReadByte();
                        iPacket.Skip(2);
                        break;

                    case 21:
                        iPacket.Skip(3);
                        break;
                }
            }

            if (position != null)
            {
                moveable.Stance = stance;
                moveable.Foothold = foothold;
                moveable.Position = position;
            }
        }

        // TODO: Refactor this.

        private int mNpcObjectIDs = 0;
        private int mMobObjectIDs = 0;
        private int mReactorObjectIDs = 0;

        public int AssignObjectID(MapObjectType type)
        {
            switch (type)
            {
                case MapObjectType.Npc: return ++mNpcObjectIDs;
                case MapObjectType.Mob: return ++mMobObjectIDs;
                case MapObjectType.Reactor: return ++mReactorObjectIDs;
                default: throw new ArgumentException(type.ToString());
            }
        }
    }
}

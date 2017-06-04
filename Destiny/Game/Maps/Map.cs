using Destiny.Core.IO;
using Destiny.Game.Characters;
using Destiny.Server.Data;
using Destiny.Server;
using System;

namespace Destiny.Game.Maps
{
    public sealed class Map
    {
        public int MapleID { get; private set; }
        public MapData Data { get; private set; }

        public byte World { get; private set; }
        public byte Channel { get; private set; }

        public MapCharacters Characters { get; private set; }
        public MapMobs Mobs { get; private set; }
        public MapNpcs Npcs { get; private set; }
        public MapPortals Portals { get; private set; }
        
        public Map(int mapleID, byte world, byte channel)
        {
            this.MapleID = mapleID;
            this.Data = MasterServer.Instance.Data.Maps.GetMapData(this.MapleID);

            this.World = world;
            this.Channel = channel;

            this.Characters = new MapCharacters(this);
            this.Mobs = new MapMobs(this);
            this.Npcs = new MapNpcs(this);
            this.Portals = new MapPortals(this);
        }

        public void Broadcast(OutPacket oPacket)
        {
            foreach (Character character in this.Characters)
            {
                character.Client.Send(oPacket);
            }
        }

        public void Broadcast(OutPacket oPacket, Character ignored)
        {
            foreach (Character character in this.Characters)
            {
                if (character != ignored)
                {
                    character.Client.Send(oPacket);
                }
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

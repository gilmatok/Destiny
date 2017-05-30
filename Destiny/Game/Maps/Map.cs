using Destiny.Game.Characters;
using Destiny.Server;
using System;
using System.IO;

namespace Destiny.Game.Maps
{
    public sealed class Map
    {
        public int MapleID { get; private set; }
        public int ReturnMapID { get; private set; }
        public int ForcedReturnMapID { get; private set; }

        public MapCharacters Characters { get; private set; }
        public MapMobs Mobs { get; private set; }
        public MapNpcs Npcs { get; private set; }
        public MapPortals Portals { get; private set; }

        public Map CachedReference
        {
            get
            {
                return MasterServer.Instance.Data.Maps[this.MapleID];
            }
        }

        public Map(BinaryReader reader)
        {
            this.MapleID = reader.ReadInt32();
            this.ReturnMapID = reader.ReadInt32();
            this.ForcedReturnMapID = reader.ReadInt32();

            this.Characters = new MapCharacters(this);
            this.Mobs = new MapMobs(this);
            this.Npcs = new MapNpcs(this);
            this.Portals = new MapPortals(this);
        }

        public Map(int mapleID)
        {
            this.MapleID = mapleID;
            this.ReturnMapID = this.CachedReference.ReturnMapID;
            this.ForcedReturnMapID = this.CachedReference.ForcedReturnMapID;

            this.Characters = this.CachedReference.Characters;
            this.Mobs = this.CachedReference.Mobs;
            this.Npcs = this.CachedReference.Npcs;
            this.Portals = this.CachedReference.Portals;
        }

        public void Broadcast(byte[] buffer)
        {
            foreach (Character character in this.Characters)
            {
                character.Client.Send(buffer);
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

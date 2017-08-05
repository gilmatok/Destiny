using Destiny.Core.IO;
using Destiny.Data;
using Destiny.Maple.Characters;
using Destiny.Maple.Data;
using Destiny.Utility;

namespace Destiny.Maple.Maps
{
    public sealed class Map
    {
        public int MapleID { get; private set; }
        public byte Channel { get; private set; }
        public int ReturnMapID { get; private set; }
        public int ForcedReturnMapID { get; private set; }
        public byte RegenerationRate { get; private set; }
        public byte DecreaseHP { get; private set; }
        public ushort DamagePerSecond { get; private set; }
        public int ProtectorItemID { get; private set; }
        public bool HasShip { get; private set; }
        public byte RequiredLevel { get; private set; }
        public int TimeLimit { get; private set; }
        public double SpawnRate { get; private set; }
        public bool IsTown { get; private set; }
        public bool HasClock { get; private set; }
        public bool IsEverlasting { get; private set; }
        public bool DisablesTownScroll { get; private set; }
        public bool IsSwim { get; private set; }
        public bool ShufflesReactors { get; private set; }
        public string UniqueShuffledReactor { get; private set; }
        public bool IsShop { get; private set; }
        public bool NoPartyLeaderPass { get; private set; }

        public MapCharacters Characters { get; private set; }
        public MapDrops Drops { get; private set; }
        public MapMobs Mobs { get; private set; }
        public MapNpcs Npcs { get; private set; }
        public MapSeats Seats { get; private set; }
        public MapPortals Portals { get; private set; }
        public MapSpawnPoints SpawnPoints { get; private set; }

        public Map CachedReference
        {
            get
            {
                return DataProvider.CachedMaps[this.MapleID];
            }
        }

        public Map(int mapleID, byte channel)
        {
            this.MapleID = mapleID;
            this.Channel = channel;
            this.ReturnMapID = this.CachedReference.ReturnMapID;
            this.ForcedReturnMapID = this.CachedReference.ForcedReturnMapID;

            this.Characters = this.CachedReference.Characters;
            this.Drops = this.CachedReference.Drops;
            this.Mobs = this.CachedReference.Mobs;
            this.Npcs = this.CachedReference.Npcs;
            this.Seats = this.CachedReference.Seats;
            this.Portals = this.CachedReference.Portals;
            this.SpawnPoints = this.CachedReference.SpawnPoints;
        }

        public Map(Datum datum)
        {
            this.MapleID = (int)datum["mapid"];
            this.ReturnMapID = (int)datum["return_map"];
            this.ForcedReturnMapID = (int)datum["forced_return_map"];

            this.Characters = new MapCharacters(this);
            this.Drops = new MapDrops(this);
            this.Mobs = new MapMobs(this);
            this.Npcs = new MapNpcs(this);
            this.Seats = new MapSeats(this);
            this.Portals = new MapPortals(this);
            this.SpawnPoints = new MapSpawnPoints(this);
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

        private int mObjectIDs = 0;

        public int AssignObjectID()
        {
            return ++mObjectIDs;
        }
    }
}

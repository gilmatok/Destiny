using Destiny.Data;
using Destiny.Maple.Characters;
using Destiny.Network;

namespace Destiny.Maple.Maps
{
    public sealed class Map
    {
        public int MapleID { get; private set; }
        public int ReturnMapID { get; private set; }
        public int ForcedReturnMapID { get; private set; }
        public sbyte RegenerationRate { get; private set; }
        public byte DecreaseHP { get; private set; }
        public ushort DamagePerSecond { get; private set; }
        public int ProtectorItemID { get; private set; }
        public sbyte ShipKind { get; private set; }
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
        public MapReactors Reactors { get; private set; }
        public MapFootholds Footholds { get; private set; }
        public MapSeats Seats { get; private set; }
        public MapPortals Portals { get; private set; }
        public MapSpawnPoints SpawnPoints { get; private set; }
        public MapPlayerShops PlayerShops { get; private set; }

        public Map(Datum datum)
        {
            this.MapleID = (int)datum["mapid"];
            this.ReturnMapID = (int)datum["return_map"];
            this.ForcedReturnMapID = (int)datum["forced_return_map"];

            this.RegenerationRate = (sbyte)datum["regen_rate"];
            this.DecreaseHP = (byte)datum["decrease_hp"];
            this.DamagePerSecond = (ushort)datum["damage_per_second"];
            this.ProtectorItemID = (int)datum["protect_item"];
            this.ShipKind = (sbyte)datum["ship_kind"];
            this.SpawnRate = (double)datum["mob_rate"];
            this.RequiredLevel = (byte)datum["min_level_limit"];
            this.TimeLimit = (int)datum["time_limit"];

            this.IsTown = ((string)datum["flags"]).Contains("town");
            this.HasClock = ((string)datum["flags"]).Contains("clock");
            this.IsEverlasting = ((string)datum["flags"]).Contains("everlast");
            this.DisablesTownScroll = ((string)datum["flags"]).Contains("scroll_disable");
            this.IsSwim = ((string)datum["flags"]).Contains("swim");
            this.ShufflesReactors = ((string)datum["flags"]).Contains("shuffle_reactors");
            this.UniqueShuffledReactor = (string)datum["shuffle_name"];
            this.IsShop = ((string)datum["flags"]).Contains("shop");
            this.NoPartyLeaderPass = ((string)datum["flags"]).Contains("no_party_leader_pass");

            this.Characters = new MapCharacters(this);
            this.Drops = new MapDrops(this);
            this.Mobs = new MapMobs(this);
            this.Npcs = new MapNpcs(this);
            this.Footholds = new MapFootholds(this);
            this.Seats = new MapSeats(this);
            this.Reactors = new MapReactors(this);
            this.Portals = new MapPortals(this);
            this.SpawnPoints = new MapSpawnPoints(this);
            this.PlayerShops = new MapPlayerShops(this);
        }

        public void Broadcast(Packet oPacket, Character ignored = null)
        {
            foreach (Character character in this.Characters)
            {
                if (character != ignored)
                {
                    character.Client.Send(oPacket);
                }
            }
        }

        public void Notify(string text, NoticeType type = NoticeType.Popup)
        {
            foreach (Character character in this.Characters)
            {
                character.Notify(text, type);
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

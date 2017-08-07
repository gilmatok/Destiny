using Destiny.Core.IO;
using Destiny.Core.Network;
using Destiny.Data;
using Destiny.Maple.Characters;
using Destiny.Maple.Data;
using Destiny.Maple.Maps;
using Destiny.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;

namespace Destiny.Maple.Life.Reactors
{
    public sealed class Reactor : LifeObject, ISpawnable
    {
        public bool IsAlive { get; private set; }

        public int RespawnTime { get; private set; }
        public Timer RespawnTimer { get; private set; }
        public string Label { get; private set; }

        public byte MaxStates { get; private set; }
        public int Link { get; private set; }
        public bool ActivateByTouch { get; private set; }
        public bool RemoveInFieldSet { get; private set; }
        
        private ReactorEvent[] Events { get; set; }
        private List<ReactorDrop> Drops { get; set; }
        private byte _state;

        public Reactor CachedReference
        {
            get
            {
                return DataProvider.CachedMaps?[this.Map.MapleID].Reactors[this.ObjectID];
            }
        }

        public List<Reactor> LinkedReactors
        {
            get
            {
                return DataProvider.CachedMaps?[this.Map.MapleID].Reactors.Where(x => x.Link == this.MapleID).ToList();
            }
        }

        public byte Flags
        {
            get
            {
                byte flags = 0;

                if (this.FacesLeft) flags |= (byte)ReactorFlags.FacesLeft;
                if (this.ActivateByTouch) flags |= (byte)ReactorFlags.ActivateByTouch;
                if (this.RemoveInFieldSet) flags |= (byte)ReactorFlags.RemoveInFieldSet;

                return flags;
            }
        }

        public byte State
        {
            get
            {
                return _state;
            }
            set
            {
                this._state = value;

                using (OutPacket oPacket = new OutPacket(ServerOperationCode.ReactorChangeState))
                {
                    oPacket
                        .WriteInt(this.ObjectID)
                        .WriteByte(this.State)
                        .WritePoint(this.Position)
                        .WriteShort() //NOTE: Action delay
                        .WriteByte() //NOTE: Event index
                        .WriteByte(4); //NOTE: Delay
                }
            }
        }

        public Reactor(Datum datum) : base(datum)
        {
            this.RespawnTime = (int)datum["respawn_time"];
            //Set up the timer, but don't start it yet
            RespawnTimer = new Timer();
            RespawnTimer.Elapsed += new ElapsedEventHandler(this.Spawn);
            RespawnTimer.AutoReset = false;

            this.Label = (string)datum["life_name"] ?? string.Empty;

            Datum reactorDatum = new Datum("reactor_data").Populate("reactorid = '{0}'", this.MapleID);
            
            this.MaxStates = (byte)(sbyte)reactorDatum["max_states"];
            this.Link = (int)reactorDatum["link"];
            this.ActivateByTouch = reactorDatum["flags"].ToString().Contains("activate_by_touch");
            this.RemoveInFieldSet = reactorDatum["flags"].ToString().Contains("remove_in_field_set");
            
            Datums events = new Datums("reactor_events").Populate("reactorid = '{0}'", this.MapleID);
            this.Events = new ReactorEvent[events.Count()];
            foreach (Datum reactorEvent in events)
            {
                ReactorEvent newEvent = new ReactorEvent(this, reactorEvent);
                this.Events[newEvent.State] = newEvent;
            }
            
            this.Drops = new List<ReactorDrop>();
            foreach (Datum drop in new Datums("drop_data").Populate("dropperid = '{0}'", this.MapleID))
            {
                this.Drops.Add(new ReactorDrop(drop));
            }

            this.IsAlive = true;
        }

        public OutPacket GetCreatePacket()
        {
            return this.GetSpawnPacket();
        }

        public OutPacket GetSpawnPacket()
        {
            OutPacket oPacket = new OutPacket(ServerOperationCode.ReactorEnterField);

            oPacket
                .WriteInt(this.ObjectID)
                .WriteInt(this.MapleID)
                .WriteByte(this.State)
                .WritePoint(this.Position)
                .WriteShort(this.Flags)
                .WriteBool() //NOTE: Unknown
                .WriteString(this.Label);

            return oPacket;
        }

        public OutPacket GetDestroyPacket()
        {
            OutPacket oPacket = new OutPacket(ServerOperationCode.ReactorLeaveField);

            oPacket
                .WriteInt(this.ObjectID)
                .WriteByte(this.State)
                .WritePoint(this.Position);

            return oPacket;
        }

        public void Hit(short actionDelay, int skillID, Character character)
        {
            if (!this.IsAlive)
                return;

            bool activated = false;

            ReactorEvent ev = this.Events[this.State];
            switch (ev.EventType)
            {
                case ReactorEventType.PlainAdvanceState:
                    activated = true;
                    break;
                case ReactorEventType.HitBySkill:
                    //TODO: Position validation
                    activated = ev.Triggers.Any(x => x.Item1 == this.State && x.Item2 == skillID);
                    break;
                case ReactorEventType.HitByItem:
                case ReactorEventType.HitFromLeft:
                case ReactorEventType.HitFromRight:
                case ReactorEventType.NoClue:
                case ReactorEventType.NoClue2:
                case ReactorEventType.Timeout:
                    //TODO
                    break;
            }
            
            if (activated)
            {
                //After reactor has reached MaxState, the next state is always 0, so a 0 indicates the reactor should be destroyed
                if (ev.NextState == 0 && this.State != 0 && ev.EventType == ReactorEventType.PlainAdvanceState)
                {
                    using (OutPacket oPacket = this.GetDestroyPacket())
                    {
                        this.Map.Broadcast(oPacket);
                    }
                    this.IsAlive = false;

                    this.SpawnDrops(character);

                    if (RespawnTime == 0)
                    {
                        Spawn();
                    }
                    else if (RespawnTime > 0)
                    {
                        RespawnTimer.Interval = RespawnTime * 1000;
                        RespawnTimer.Enabled = true;
                    }
                }

                this.State = ev.NextState != this.State ? ev.NextState : (byte)(this.State + 1);
            }
        }

        public void Spawn(object caller = null, ElapsedEventArgs args = null)
        {
            if (!this.IsAlive)
            {
                using (OutPacket oPacket = this.GetSpawnPacket())
                {
                    this.Map.Broadcast(oPacket);
                }
            }

            RespawnTimer.Enabled = false;
            this.IsAlive = true;
        }

        public void SpawnDrops(Character owner)
        {
            Random rand = new Random();
            foreach (var drop in this.Drops)
            {
                if (rand.Next(1000000) >= 1000000 - (drop.Chance * MasterServer.World.DropRate)
                    && (drop.QuestID <= 0 || owner.Quests.Started.ContainsKey(drop.QuestID)))
                {
                    short quantity = (short)rand.Next(drop.MinQuantity, drop.MaxQuantity);
                    Item item = new Item(drop.ItemID, quantity);
                    item.Dropper = this;
                    item.Position = this.Position; //TODO: Set this better
                    this.Map.Drops.Add(item);
                }
            }
        }
    }
}

using Destiny.Core.IO;
using Destiny.Core.Network;
using Destiny.Data;
using Destiny.Maple.Data;
using Destiny.Maple.Maps;
using System.Collections.Generic;
using System.Linq;

namespace Destiny.Maple.Life.Reactors
{
    public sealed class Reactor : LifeObject, ISpawnable
    {
        public Map Parent { get; private set; }
        public byte State { get; private set; }

        public int RespawnTime { get; private set; }
        public string Label { get; private set; }

        public byte MaxStates { get; private set; }
        public int Link { get; private set; }
        public bool ActivateByTouch { get; private set; }
        public bool RemoveInFieldSet { get; private set; }

        private List<ReactorEvent> Events { get; set; }

        public Reactor CachedReference
        {
            get
            {
                return DataProvider.CachedMaps[this.Parent.MapleID].Reactors[this.MapleID];
            }
        }

        public List<Reactor> LinkedReactors
        {
            get
            {
                return DataProvider.CachedMaps[this.Parent.MapleID].Reactors.Where(x => x.Link == this.MapleID).ToList();
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

        public Reactor(Map parent, Datum datum) : base(datum)
        {
            this.Parent = parent;
            this.RespawnTime = (int)datum["respawn_time"];
            this.Label = (string)datum["life_name"];

            Datum reactorDatum = new Datum("reactor_data").Populate("reactorid = '{0}'", this.MapleID);
            
            this.MaxStates = (byte)(sbyte)reactorDatum["max_states"];
            this.Link = (int)reactorDatum["link"];
            this.ActivateByTouch = reactorDatum["flags"].ToString().Contains("activate_by_touch");
            this.RemoveInFieldSet = reactorDatum["flags"].ToString().Contains("remove_in_field_set");

            this.Events = new List<ReactorEvent>();
            foreach (Datum reactorEvent in new Datums("reactor_events").Populate("reactorid = '{0}'", this.MapleID))
            {
                ReactorEvent newEvent = new ReactorEvent(this, reactorEvent);
                this.Events.Add(newEvent);
            }
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
                .WriteShort() //NOTE: Unknown
                .WriteByte() //NOTE: Unknown
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
    }
}

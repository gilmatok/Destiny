using Destiny.Data;
using System;
using System.Collections.Generic;

namespace Destiny.Maple.Life.Reactors
{
    public sealed class ReactorEvent
    {
        public Reactor Parent { get; private set; }

        public byte State { get; private set; }
        public byte NextState { get; private set; }
        public ReactorEventType EventType { get; private set; }
        public int Timeout { get; private set; }
        public int ItemId { get; private set; }
        public short Quantity { get; private set; }
        public Rectangle Boundaries { get; private set; }

        public List<Tuple<byte, int>> Triggers { get; private set; }

        public ReactorEvent(Reactor parent, Datum datum)
        {
            this.Parent = parent;

            this.State = (byte)(sbyte)datum["state"];
            this.NextState = (byte)(sbyte)datum["next_state"];
            this.EventType = (ReactorEventType)Enum.Parse(typeof(ReactorEventType), datum["event_type"].ToString().ToCamel().Replace("_", ""));
            this.Timeout = (int)datum["timeout"];
            this.ItemId = (int)datum["itemid"];
            this.Quantity = (short)datum["quantity"];
            this.Boundaries = new Rectangle(new Point((short)datum["ltx"], (short)datum["lty"]), new Point((short)datum["ltx"], (short)datum["lty"]));

            Triggers = new List<Tuple<byte, int>>();
            foreach(Datum eventTrigger in new Datums("reactor_event_trigger_skills").Populate("reactorid = '{0}'", this.Parent.MapleID))
            {
                Triggers.Add(new Tuple<byte, int>((byte)eventTrigger["state"], (int)eventTrigger["skillid"]));
            }
        }
    }
}

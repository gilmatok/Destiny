using Destiny.Data;
using System;

namespace Destiny.Maple.Life
{
    public sealed class ReactorState
    {
        public ReactorEventType Type { get; private set; }
        public byte State { get; private set; }
        public byte NextState { get; private set; }
        public int Timeout { get; private set; }
        public int ItemId { get; private set; }
        public short Quantity { get; private set; }
        public Rectangle Boundaries { get; private set; }

        public ReactorState(Datum datum)
        {
            this.Type = this.Timeout > 0 ? ReactorEventType.Timeout : (ReactorEventType)Enum.Parse(typeof(ReactorEventType), datum["event_type"].ToString().ToCamel().Replace("_", ""));
            this.State = (byte)(sbyte)datum["state"];
            this.NextState = (byte)(sbyte)datum["next_state"];
            this.Timeout = (int)datum["timeout"];
            this.ItemId = (int)datum["itemid"];
            this.Quantity = (short)datum["quantity"];
            this.Boundaries = new Rectangle(new Point((short)datum["ltx"], (short)datum["lty"]), new Point((short)datum["ltx"], (short)datum["lty"]));
        }
    }
}

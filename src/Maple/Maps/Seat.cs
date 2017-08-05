using Destiny.Data;

namespace Destiny.Maple.Maps
{
    public sealed class Seat : MapObject
    {
        public short ID { get; private set; }

        public Seat(Datum datum)
            : base()
        {
            this.ID = (short)datum["seatid"];
            this.Position = new Point((short)datum["x_pos"], (short)datum["y_pos"]);
        }
    }
}

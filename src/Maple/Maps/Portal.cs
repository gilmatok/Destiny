using Destiny.Server;
using Destiny.Utility;

namespace Destiny.Maple.Maps
{
    public sealed class Portal : MapObject
    {
        public byte ID { get; private set; }
        public string Label { get; private set; }
        public int DestinationMap { get; private set; }
        public string DestinationLabel { get; private set; }
        public string Script { get; private set; }

        public Portal Link
        {
            get
            {
                return MasterServer.Channels[this.Map.Channel].Maps[this.DestinationMap].Portals[this.DestinationLabel];
            }
        }

        public Portal(DatabaseQuery query)
        {
            this.ID = (byte)query.GetInt("id");
            this.Label = query.GetString("label");
            this.Position = new Point(query.GetShort("x_pos"), query.GetShort("y_pos"));
            this.DestinationMap = query.GetInt("destination");
            this.DestinationLabel = query.GetString("destination_label");
            this.Script = query.GetString("script");
        }
    }
}

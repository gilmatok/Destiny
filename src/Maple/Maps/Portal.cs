using Destiny.Data;
using Destiny.Network;

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

        public string ScriptPath
        {
            get
            {
                return string.Format(@"..\..\scripts\portals\{0}.js", this.Script);
            }
        }

        public Portal(Datum datum)
        {
            this.ID = (byte)(int)datum["id"];
            this.Label = (string)datum["label"];
            this.Position = new Point((short)datum["x_pos"], (short)datum["y_pos"]);
            this.DestinationMap = (int)datum["destination"];
            this.DestinationLabel = (string)datum["destination_label"];
            this.Script = (string)datum["script"];
        }
    }
}

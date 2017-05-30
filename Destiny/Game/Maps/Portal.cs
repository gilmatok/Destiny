using System.IO;

namespace Destiny.Game.Maps
{
    public sealed class Portal : MapObject
    {
        public byte ID { get; private set; }
        public string Label { get; private set; }
        public int DestinationID { get; private set; }
        public string DestinationLabel { get; private set; }
        public string Script { get; private set; }

        public override MapObjectType Type
        {
            get
            {
                return MapObjectType.Portal;
            }
        }

        public Portal(BinaryReader reader)
        {
            this.ID = reader.ReadByte();
            this.Label = reader.ReadString();
            this.DestinationID = reader.ReadInt32();
            this.DestinationLabel = reader.ReadString();
            this.Script = reader.ReadString();
            this.Position = new Point(reader.ReadInt16(), reader.ReadInt16());
        }
    }
}

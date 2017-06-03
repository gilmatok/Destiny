using Destiny.Game.Data;
using Destiny.Server;

namespace Destiny.Game.Maps
{
    public sealed class Portal : MapObject
    {
        public byte ID { get; private set; }
        public MapPortalData Data { get; private set; }

        public override MapObjectType Type
        {
            get
            {
                return MapObjectType.Portal;
            }
        }

        public Portal Link
        {
            get
            {
                return MasterServer.Instance.Worlds[this.Map.World].Channels[this.Map.Channel].Maps[this.Data.DestinationMap].Portals[this.Data.DestinationLabel];
            }
        }

        public Portal(MapPortalData data)
        {
            this.ID = data.ID;
            this.Data = data;
        }
    }
}

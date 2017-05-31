using Destiny.Game.Data;

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

        public Portal(MapPortalData data)
        {
            this.ID = data.ID;
            this.Data = data;
        }
    }
}
